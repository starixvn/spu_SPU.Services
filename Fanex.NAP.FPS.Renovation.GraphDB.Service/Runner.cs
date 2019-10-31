using Fanex.Graph.Data;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.NAP.FPS.Renovation.GraphDB.Service.Services.Implement;
using Fanex.NAP.FPS.Renovation.GraphDB.Service.Services.Interface;
using Fanex.NAP.FPS.Renovation.Model;
using Fanex.NAP.FPS.Renovation.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;

namespace Fanex.NAP.FPS.Renovation.GraphDB.Service
{
    public partial class Runner : ServiceBase
    {
        private EasyNetQConnector easyNetQService;
        private IDataCenterService dataCenterService;
        private List<IsSynAssociationModel> associations;
        public Runner()
        {
            InitializeComponent();

            dataCenterService = new DataCenterService();

            associations = new List<IsSynAssociationModel>();
        }

        protected override void OnStart(string[] args)
        {
            easyNetQService = new EasyNetQConnector(AppSettings.RabbitMQ_Host,
                AppSettings.RabbitMQ_VirtualHost,
                Convert.ToInt32(AppSettings.RabbitMQ_PrefetchCount),
                AppSettings.RabbitMQ_Username,
                AppSettings.RabbitMQ_Password).CreateBus();

            easyNetQService.DeclareQueue(AppSettings.RabbitMQ_Queue)
                            .DeclareExchange(AppSettings.RabbitMQ_Exchange)
                            .DeclareRoutingKey(AppSettings.RabbitMQ_RoutingKey)
                            .Bind();

            AddHandlerForGetMassage(InsertCustomer);
        }

        protected override void OnStop()
        {
            easyNetQService.Disposed();
        }

        public void AddHandlerForGetMassage(Action<AssociationModel> handler)
        {
            easyNetQService.AdvanceBus.Consume(easyNetQService.Queue,
                registration => registration.Add<AssociationModel>((message, info) =>
                {
                    handler(message.Body);
                }));
        }

        private void UpdateData(string key, AssociationModel criteria)
        {
            var result = DbSessionFactory.Execute(key, criteria);

            if (result != null)
            {
                if(associations.Count < Core.AppSettings.TransBatch)
                {
                    associations.Add(new IsSynAssociationModel
                    {
                        AssociationId = criteria.AssociationId,
                        SyncGraphDB = 2
                    });
                }
                else
                {
                    var xmlData = Core.Utilities.XMLConvert(associations);

                    dataCenterService.UpdateIsSyncByAssociationId(xmlData);

                    associations.Clear();
                }                
            }
        }

        private void InsertCustomer(AssociationModel criteria)
        {
            // ko luu SMA, chi luu member, roleid = 1
            if (criteria.RoleId == 1)
            {
                // remove test account
                if (!criteria.Username.Contains("test") || criteria.Username2 == null || !criteria.Username2.Contains("test"))
                {
                    try
                    {
                        var custId = DbSessionFactory.Query<int>(FPS.GetCustomerById, new { criteria.CustId });
                        var deviceId = DbSessionFactory.Query<int>(FPS.GetDeviceById, new { criteria.DeviceId });
                        var association = DbSessionFactory.Query<int>(FPS.CheckExistedAssociation, new { criteria.CustId, criteria.DeviceId });

                        if (criteria.DeviceId == 0)
                        {
                            if (!custId.Any())
                            {
                                UpdateData(FPS.AddNewCustomer, criteria);
                            }
                        }
                        else
                        {
                            UpdateData(FPS.AddAssociation, criteria);

                            if (custId.Any())
                            {
                                if (deviceId.Any())
                                {
                                    // case acc cu, device cu
                                    if (association.Any())
                                    {
                                        UpdateData(FPS.UpdateCustomerUsername2, criteria);
                                    }
                                    else
                                    {
                                        UpdateData(FPS.MergeGroup, criteria);
                                    }
                                }
                            }
                            else
                            {
                                if (deviceId.Any())
                                {
                                    // case acc moi, device cu
                                    UpdateData(FPS.AddCustomerGroupRelationship, criteria);
                                }
                                else
                                {
                                    // case acc moi, device moi
                                    UpdateData(FPS.CreateNewGroup, criteria);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var logMessage = new LogMessage(ex.Message)
                         .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                         .ToString();

                        LogService.Error(logMessage);
                    }
                }
            }
        }
    }
}
