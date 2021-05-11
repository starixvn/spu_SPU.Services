namespace Fanex.NAP.CTS.CTSCustomer.Service
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.CTS.Services.Interface;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.SPU.Core;
    using Fanex.NAP.SPU.Model;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using static Fanex.NAP.SPU.Core.Constants;

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
                var result = JsonConvert.DeserializeObject<List<UpdateCustomerModel>>(jsonContent).ToList();

                if(result != null && result.Any())
                { 
                    return InsertNewCustomer(result[0].CustomerInfos, result[0].LastInsertCustID);
                }               
            }
                
            LogService.Error($"Insert New Customer - Request content is empty. {jsonContent}");

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
                    var dataTranfrom = JsonConvert.DeserializeObject<List<TranformCustModel>>(jsonContent).ToList();

                    if (dataTranfrom == null || !dataTranfrom.Any())
                    {
                        return true;
                    }

                    var custFromCusInfo = dataTranfrom[0];
                    var custFromCustomer = dataTranfrom[1];

                    UpdateCustomer(custFromCusInfo, (int)Constants.UPDATECUSTOMER.FROM_CUSTINFO);
                    UpdateCustomer(custFromCustomer, (int)Constants.UPDATECUSTOMER.FROM_CUSTOMER);

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
                        _dataCenterService.UpdateUsername2(customers);

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

        private bool InsertNewCustomer(List<CustomerMainDBModel> customers, long? lastCustID)
        {
            try
            {
                var isWriteLogInfo = AppSettings.IsWriteLogInfo;

                if (isWriteLogInfo)
                    LogService.Info($"CTSCustomer.Service - InsertNewCustomer start with {JsonConvert.SerializeObject(customers)} - LastCustId = {lastCustID}");

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
                                        LogService.Info($"CTSCustomer.Service - InsertNewCustomer - RescanCustomerClassify done with data: {JsonConvert.SerializeObject(custToRescan)}");
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
                                    _dataCenterService.ReTransformAccount(JsonConvert.SerializeObject(custIdList), lastCustID);

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

                LogService.Error($"CTSCustomer.Service - InsertNewCustomer throws exception {logMessage} with listCust: {JsonConvert.SerializeObject(customers)}");
            }

            return false;
        }

        private void UpdateCustomer(TranformCustModel custs, int sourceType)
        {
            try
            {
                if (custs == null || custs.LastCustId == null || custs.LastUpdatedTime == null)
                {
                    return;
                }

                var updateCustomerType = (UPDATECUSTOMER)sourceType;

                var custInfoToUpdate = GetCustInfoToUpdate(custs, updateCustomerType);

                _dataCenterService.UpdateCustomerComplete(custInfoToUpdate, custs.LastUpdatedTime, custs.LastCustId, updateCustomerType);

                if (AppSettings.IsWriteLogInfo)
                    LogService.Info($"Healing - done");

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

        private string GetCustInfoToUpdate(TranformCustModel custs, UPDATECUSTOMER sourceType)
        {
            var custInfoToUpdate = JsonConvert.SerializeObject(string.Empty);

            if (!string.IsNullOrWhiteSpace(custs.CustIdList))
            {
                var custInfoFromMainDB = _dataCenterService.GetCustomerFromMainDB(custs.CustIdList, sourceType);
                var custInfoFromCTS = _dataCenterService.GetCustomerFromCTS(custs.CustIdList, sourceType);

                if (AppSettings.IsWriteLogInfo)
                {
                    LogService.Info($"Healing - Get {((sourceType == UPDATECUSTOMER.FROM_CUSTOMER) ? "Customer History" : "CustInfo")} from MainDB: {JsonConvert.SerializeObject(custInfoFromMainDB)}");
                    LogService.Info($"Healing - Get {((sourceType == UPDATECUSTOMER.FROM_CUSTOMER) ? "Customer History" : "CustInfo")} from CTS: {JsonConvert.SerializeObject(custInfoFromCTS)}");
                }

                if(custInfoFromMainDB != null && custInfoFromCTS != null)
                {
                    custInfoToUpdate = CompareCustomerInfo(custInfoFromMainDB, custInfoFromCTS, sourceType);
                }
            }

            if (AppSettings.IsWriteLogInfo)
                LogService.Info($"Healing - Cust to update data: {custInfoToUpdate}");

            return custInfoToUpdate;
        }

        private string CompareCustomerInfo(IEnumerable<CustomerMainDBModel> custInfoFromMainDB, IEnumerable<CustomerMainDBModel> custInfoFromCTS, UPDATECUSTOMER sourceType)
        {
            if (sourceType == UPDATECUSTOMER.FROM_CUSTINFO)
            {
                var customerInfo = custInfoFromMainDB
                    .Join(custInfoFromCTS, custMain => custMain.CustID,
                                           custCTS => custCTS.CustID,
                                           (custMain, custCTS) => custMain)
                    .Where(c => !custInfoFromCTS.Contains(c, new CustomerInfoEqualityComparer()))
                    .Select(c => new CustInfoModel
                    {
                        CustID = c.CustID,
                        CustStatusID = c.CustStatusID,
                        Danger1 = c.Danger1,
                        Danger2 = c.Danger2,
                        Danger3 = c.Danger3
                    }).ToList();

                return JsonConvert.SerializeObject(customerInfo);
            }
            else
            {
                var customerInfo = custInfoFromMainDB
                    .Join(custInfoFromCTS, custMain => custMain.CustID,
                                           custCTS => custCTS.CustID,
                                           (custMain, custCTS) => custMain)
                    .Where(c => !custInfoFromCTS.Contains(c, new CustomerEqualityComparer()))
                    .Select(c => new CustomerInfoModel
                    {
                        CustID = c.CustID,
                        UserName = c.UserName,
                        UserName2 = c.UserName2,
                        SiteID = c.SiteID,
                        Site = c.Site,
                        RoleID = c.RoleID,
                        SRecommend = c.SRecommend,
                        MRecommend = c.MRecommend,
                        Recommend = c.Recommend,
                        Currency = c.Currency,
                        CurrencyID = c.CurrencyID,
                        UpdateType = (custInfoFromCTS.Any(f => f.CustID == c.CustID &&
                                                (!string.Equals(f.UserName, c.UserName)
                                             || !string.Equals(f.UserName2, c.UserName2)
                                             || f.SiteID != c.SiteID)) == true) ? 1 : 0
                    }).ToList();   

                return JsonConvert.SerializeObject(customerInfo);
            }
        }
    }
}
