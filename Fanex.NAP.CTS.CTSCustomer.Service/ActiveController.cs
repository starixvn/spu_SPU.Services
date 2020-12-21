namespace Fanex.NAP.CTS.CTSCustomer.Service
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.CTS.Services.Helper;
    using Fanex.NAP.CTS.Services.Interface;
    using Fanex.NAP.SPU.Core;
    using Fanex.NAP.SPU.Model;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    public class ActiveController : ApiController
    {
        private readonly IDataCenterService _dataCenterService;
        private readonly IRescanCustomerClassifyService _rescanCustomerClassificationService;

        public ActiveController()
        {
            _dataCenterService = new DataCenterService();
            _rescanCustomerClassificationService = new RescanCustomerClassifyService();
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
                    var customers = JsonConvert.DeserializeObject<List<CustomerMainDbModel>>(jsonContent);

                    if (customers != null && customers.Any())
                    {
                        _dataCenterService.FillUpCustomerFromMainDB(customers);
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
                var customers = JsonConvert.DeserializeObject<List<CustomerMainDbModel>>(jsonContent);
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

        private bool InsertNewCustomer(List<CustomerMainDbModel> customers)
        {
            try
            {
                if (customers != null && customers.Any())
                {
                    _dataCenterService.TransformCustomerFromMainDB(customers);

                    var custIdList = customers.Where(c => c.IsDangerous == false).Select(c => c.CustId).Distinct().ToList();

                    _rescanCustomerClassificationService.RescanCustomerClassification(custIdList);

                    return true;
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                LogService.Error( $"InsertNewCustomer - exception {logMessage} with listCust: {JsonConvert.SerializeObject(customers)}");
            }

            return false;
        }
    }
}
