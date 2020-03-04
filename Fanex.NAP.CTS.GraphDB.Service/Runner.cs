using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.NAP.CTS.GraphDB.Service.Implement;
using Fanex.NAP.CTS.GraphDB.Service.Interface;
using Fanex.NAP.CTS.Model.GraphDB;
using Fanex.NAP.SPU.Graph.Core;
using Fanex.NAP.SPU.Graph.Core.Neo4j;
using Fanex.NAP.SPU.Queue;
using SharpRaven;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.GraphDB.Service
{
    public partial class Runner : ServiceBase
    {
        private static EasyNetQConnector easyNetQService;
        private static IDataCenterService dataCenterService;

        public Runner()
        {
            InitializeComponent();

            dataCenterService = new DataCenterService();

            easyNetQService = new EasyNetQConnector(AppSettings.RabbitMQ_Host,
                AppSettings.RabbitMQ_VirtualHost,
                Convert.ToInt32(AppSettings.RabbitMQ_PrefetchCount),
                AppSettings.RabbitMQ_Username,
                AppSettings.RabbitMQ_Password).CreateBus();

            easyNetQService.DeclareQueue(AppSettings.RabbitMQ_Queue)
                            .DeclareExchange(AppSettings.RabbitMQ_Exchange)
                            .DeclareRoutingKey(AppSettings.RabbitMQ_RoutingKey)
                            .Bind();
        }

        protected override void OnStart(string[] args)
        {
            AddHandlerForGetMassage(TransformFromCTS);
        }

        protected override void OnStop()
        {
        }

        private static void TransformFromCTS(AssociationModelForGraph criteria)
        {
            if (criteria.CustId == 0 && !string.IsNullOrEmpty(criteria.Username2) && criteria.SubscriberId > 0)
            {
                DbSessionFactory.Execute(CTS.TransformAccountFromDCS, criteria);
            }
            else if (criteria.DeviceId > 0)
            {
                DbSessionFactory.Execute(CTS.TransformAssociation, criteria);
            }
            else if (criteria.AssociationId > 0)
            {
                DbSessionFactory.Execute(CTS.TransformAssociation, criteria);
            }
            else
            {
                //DbSessionFactory.Execute(CTS.TransformFromDCS, criteria);
            }
        }

        private static void AddHandlerForGetMassage(Action<AssociationModelForGraph> handler)
        {
            easyNetQService.AdvanceBus.Consume(easyNetQService.Queue,
                registration => registration.Add<AssociationModelForGraph>((message, info) =>
                {
                    handler(message.Body);
                }));
        }
    }
}
