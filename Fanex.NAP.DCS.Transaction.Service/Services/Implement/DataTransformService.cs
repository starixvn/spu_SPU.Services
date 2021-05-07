namespace Fanex.NAP.DCS.Transaction.Service.Services.Implement
{
    using Fanex.NAP.DCS.Core;
    using Fanex.NAP.DCS.DataAccess;
    using Fanex.NAP.DCS.Transaction.Service.Services.Interface;
    using Fanex.NAP.DCS.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    class DataTransformService : IDataTransformService
    {
        private readonly DataAccess.DataCenterDataAccess dataCenter;

        private long fromId;
        private long toId;

        public DataTransformService()
        {
            dataCenter = new DataCenterDataAccess();
        }

        public IDataTransformService Initial(long fromId, long toId)
        {
            this.fromId = fromId;
            this.toId = toId;

            System.Console.WriteLine($"1.Initial Min Id: {fromId} - Max Id: {toId}");

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
    }
}
