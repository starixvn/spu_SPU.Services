namespace Fanex.NAP.CTS.CTSCustomer.Service
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.CTS.Services.Interface;
    using Fanex.NAP.SPU.Core;
    using Fanex.NAP.SPU.Model;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using Fanex.NAP.FS.Core;
    using System.Threading.Tasks;

    public class ActiveController : ApiController
    {
        private readonly IDataCenterService _dataCenterService;
        private readonly ICustomerClassifyService _customerClassificationService;

        public ActiveController()
        {
            _dataCenterService = new DataCenterService();
            _customerClassificationService = new CustomerClassifyService();
        }

        public string Index()
        {
            return "ahihi";
        }

        [HttpPost]
        public bool InsertCustomer()
        {
            var requestContent = Request.Content;
            var jsonContent = requestContent.ReadAsStringAsync().Result;

            if (!string.IsNullOrWhiteSpace(jsonContent))
            {
                var result = JsonConvert.DeserializeObject<List<CustomerMainDbModel>>(jsonContent);

                return InsertNewCustomer(result);
            }
            else
            {
                LogService.Error("Request content is empty.");
            }

            return false;
        }

        [HttpPost]
        public bool InsertMissingCustomer()
        {
            var requestContent = Request.Content;
            var jsonContent = requestContent.ReadAsStringAsync().Result;

            if (!string.IsNullOrWhiteSpace(jsonContent))
            {
                try
                {
                    var tranformCusts = JsonConvert.DeserializeObject<List<TranformCustModel>>(jsonContent).FirstOrDefault();

                    if (tranformCusts != null )
                    {
                        var custInfoFromMainDB =  _dataCenterService.GetCustomerFromMainDB(tranformCusts.CustIdList);
                        var custInfoFromCTS = _dataCenterService.GetCustomerFromCTS(tranformCusts.CustIdList);
                        var custToUpdateInfo = new List<CustomerMainDbModel>();

                        if (custInfoFromMainDB == null || !custInfoFromMainDB.Any())
                        {
                            return true;
                        }

                        custToUpdateInfo = custInfoFromMainDB.Where(custsDB => !custInfoFromCTS.Contains(custsDB)).ToList();
                        
                        if(custToUpdateInfo != null && custToUpdateInfo.Any())
                        {
                            _dataCenterService.UpdateCustomerInfo(JsonConvert.SerializeObject(custToUpdateInfo), tranformCusts.LastUpdatedTime, tranformCusts.LastCustId);
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                    LogService.Error(logMessage);
                }
            }
            else
            {
                LogService.Error("Request content is empty.");
            }

            return false;
        }

        [HttpPost]
        public bool UpdateUsername2()
        {
            var requestContent = Request.Content;
            var jsonContent = requestContent.ReadAsStringAsync().Result;

            if (!string.IsNullOrWhiteSpace(jsonContent))
            {
                var customers = JsonConvert.DeserializeObject<List<GetCustIdModel>>(jsonContent);
                try
                {
                    if (customers != null && customers.Any())
                    {
                        _dataCenterService.UpdateUsername2(customers, AppSettings.UpdateUserName2Batch);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                          .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                          .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                          .ToString();

                    LogService.Error(logMessage);
                }
            }

            return false;
        }

        private bool InsertNewCustomer(List<CustomerMainDbModel> customers)
        {
            try
            {
                var isWriteLogInfo = AppSettings.IsWriteLogInfo;

                if (isWriteLogInfo)
                    LogService.Info($"CTSCustomer.Service - InsertNewCustomer start with {JsonConvert.SerializeObject(customers)}");

                if (customers != null && customers.Any())
                {
                    var customersChunk = customers.Distinct().Chunk(AppSettings.InternalChunkSize);

                    foreach (var custs in customersChunk)
                    {
                        var custIdList = _dataCenterService.TransformCustomerFromMainDB(custs.ToList());

                        if (isWriteLogInfo)                  
                            LogService.Info($"CTSCustomer.Service - Insert: {JsonConvert.SerializeObject(custs.ToList())}");

                        if (custIdList != null && custIdList.Any())
                        {
                            var tasks = new List<Task>();

                            tasks.Add(Task.Run(() =>
                            {
                                try
                                {
                                    var custToRescan = custIdList.Where(c => !c.IsExisting).Select(c => c.CustID).Distinct().ToList();
                                    _customerClassificationService.RescanCustomerClassify(custToRescan, true, true, Constants.FLOWTYPERESCANCC.NEWMEMBER);

                                    if (isWriteLogInfo)
                                        LogService.Info($"CTSCustomer.Service - InsertNewCustomer - RescanCustomerClassify done with data: {string.Join(",", custIdList)}");
                                }
                                catch (Exception ex)
                                {
                                    var logMessage = new LogMessage(ex.Message).AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex)).AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();
                                    LogService.Error($"CTSCustomer.Service - InsertNewCustomer throws exception {logMessage} with listCust: {JsonConvert.SerializeObject(custIdList)}");
                                }
                            }));

                            tasks.Add(Task.Run(() =>
                            {
                                try
                                {
                                    _dataCenterService.ReTransformAccount(JsonConvert.SerializeObject(custIdList));

                                    if (isWriteLogInfo)
                                        LogService.Info($"CTSCustomer.Service - InsertNewCustomer - Insert complete: {JsonConvert.SerializeObject(custIdList)}");
                                }
                                catch (Exception ex)
                                {
                                    var logMessage = new LogMessage(ex.Message).AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex)).AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();
                                    LogService.Error($"CTSCustomer.Service - InsertNewCustomer - ReTransformAccount throws exception {logMessage} with listCust: {JsonConvert.SerializeObject(custIdList)}");
                                }
                            }));

                            Task.WhenAll(tasks.ToArray()).Wait();
                        }              
                    }
                }

                if (isWriteLogInfo)
                    LogService.Info($"CTSCustomer.Service - InsertNewCustomer done");

                return true;
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                LogService.Error( $"CTSCustomer.Service - InsertNewCustomer throws exception {logMessage} with listCust: {JsonConvert.SerializeObject(customers)}");
            }

            return false;
        }
    }
}
