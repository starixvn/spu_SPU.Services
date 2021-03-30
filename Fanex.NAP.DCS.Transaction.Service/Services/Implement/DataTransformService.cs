namespace Fanex.NAP.DCS.Transaction.Service.Services.Implement
{
    using Fanex.NAP.DCS.Core;
    using Fanex.NAP.DCS.DataAccess;
    using Fanex.NAP.DCS.Transaction.Service.Services.Interface;
    using Fanex.NAP.DCS.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Logging;

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

            LogService.Info($"1.Initial Min Id: {fromId} - Max Id: {toId}");

            return this;
        }             

        public IDataTransformService UserAgentProcessing()
        {          
            dataCenter.UserAgentProcess(this.fromId, this.toId);

            LogService.Info($"3.UserAgent Process - move data: { this.fromId} - {this.toId}");

            return this;
        }

        public IDataTransformService TransactionProcessing()
        {
            dataCenter.TransactionProcess(this.fromId, this.toId);

            LogService.Info($"5.Transaction Process { this.fromId} - {this.toId}");

            return this;
        }

        public IDataTransformService UnhandleSubscriberAccountProcessing()
        {
            dataCenter.UnhandleSubscriberActionProcess(this.fromId, this.toId);

            LogService.Info("2.UnhandleSubscriberActionProcess");

            return this;
        }                              
    }
}
