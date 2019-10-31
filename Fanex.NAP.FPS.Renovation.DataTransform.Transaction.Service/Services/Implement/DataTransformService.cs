namespace Fanex.NAP.FPS.Renovation.DataTransform.Transaction.Service.Services.Implement
{
    using Fanex.NAP.FPS.Renovation.Core;
    using Fanex.NAP.FPS.Renovation.DataAccess;
    using Fanex.NAP.FPS.Renovation.DataTransform.Transaction.Service.Services.Interface;
    using Fanex.NAP.FPS.Renovation.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    class DataTransformService : IDataTransformService
    {
        private readonly DataAccess.DataCenterDataAccess dataCenter;

        private int fromId;
        private int toId;

        public DataTransformService()
        {
            dataCenter = new DataCenterDataAccess();
        }

        public IDataTransformService Initial(int fromId, int toId)
        {
            this.fromId = fromId;
            this.toId = toId;

            System.Console.WriteLine($"1.Initial Min Id: {fromId} - Max Id: {toId}");

            return this;
        }

        public IDataTransformService AccountProcessing()
        {
            System.Console.WriteLine("4.Account Process");

            var existingAccounts = AccountProcess(this.fromId, this.toId);

            if (existingAccounts.Any())
            {
                var customerInfo = UpdateCustomerInfo(existingAccounts);

                var newCustomerInfo = customerInfo.Select(x =>
                {
                    x.Site = x.Site ?? string.Empty;
                    x.Username = x.Username ?? string.Empty;
                    x.Username2 = x.Username2 ?? string.Empty;
                    x.CurrencyName = x.CurrencyName ?? string.Empty;

                    return x;
                }).ToList();

                var customerInfoMapping = existingAccounts
                    .Where(x => !newCustomerInfo.Any(a => a.LoginName == x.LoginName || a.LoginNameWithPrefix == x.LoginNameWithPrefix))
                    .Select(s => new CustomerInfoModel
                    {
                        AccountId = s.AccountID,
                        CreatedDate = s.CreatedDate,
                        CurrencyId = 0,
                        CurrencyName = string.Empty,
                        CustID = 0,
                        IsDeposit = s.IsDeposit,
                        LoginName = s.LoginName,
                        LoginNameWithPrefix = s.LoginNameWithPrefix,
                        RoleID = 0,
                        Site = string.Empty,
                        SubAccId = 0,
                        SubscriberId = s.SubscriberID,
                        SubscriberName = s.SubscriberName,
                        Username = string.Empty,
                        Username2 = string.Empty
                    })
                    .ToList();

                var latestCustomerInfo = newCustomerInfo.Union(customerInfoMapping).ToList();                              

                InsertRawAccount(latestCustomerInfo);                
            }

            return this;
        }       

        public IDataTransformService UserAgentProcessing()
        {          
            dataCenter.UserAgentProcess(this.fromId, this.toId);

            System.Console.WriteLine("3.UserAgent Process");

            return this;
        }

        public IDataTransformService TransactionProcessing()
        {
            dataCenter.TransactionProcess(this.fromId, this.toId);

            System.Console.WriteLine("5.Transaction Process");

            return this;
        }

        public IDataTransformService UnhandleSubscriberAccountProcessing()
        {
            dataCenter.UnhandleSubscriberActionProcess(this.fromId, this.toId);

            System.Console.WriteLine("2.UnhandleSubscriberActionProcess");

            return this;
        }       

        public IDataTransformService CompleteProcessing()
        {
            dataCenter.CompleteProcess(this.fromId, this.toId);

            System.Console.WriteLine("===> 6.Completed Process <===");

            return this;
        }        

        private List<CustomerInfoModel> UpdateCustomerInfo(List<AccountExistedModel> accountList)
        {
            System.Console.WriteLine("=> 4.2.Update Customer Info");

            var getNewCustomer = accountList.Select(acc => TransformCustomerInfo(acc)).ToList();

            if (getNewCustomer.Any())
            {
                var accountListXml = Utilities.XMLConvert(getNewCustomer);

                return dataCenter.GetCustomerInfo(accountListXml);
            }

            return new List<CustomerInfoModel>();
        }

        private NewCustomerModel TransformCustomerInfo(AccountExistedModel model)
        {
            return new NewCustomerModel
            {
                AccountID = model.AccountID,
                LoginName = model.LoginName,
                SubscriberID = model.SubscriberID,
                SubscriberName = model.SubscriberName,
                LoginNameWithPrefix = model.LoginNameWithPrefix
            };
        }

        private List<AccountExistedModel> AccountProcess(int fromId, int toId)
        {
            System.Console.WriteLine("=> 4.1.Get Existing Account");

            return dataCenter.AccountProcess(fromId, toId);
        }

        private void InsertRawAccount(List<CustomerInfoModel> accounts)
        {
            var accountListJson = JsonConvert.SerializeObject(accounts);

            dataCenter.InsertRawAccount(accountListJson);

            System.Console.WriteLine("=> 4.3.Insert Raw Account");
        }

       
    }
}
