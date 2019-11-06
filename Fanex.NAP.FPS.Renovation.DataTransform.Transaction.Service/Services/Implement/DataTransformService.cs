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
