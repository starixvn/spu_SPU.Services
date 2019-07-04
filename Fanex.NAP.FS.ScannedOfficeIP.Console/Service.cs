namespace Fanex.NAP.FS.ScannedOfficeIP.Console
{
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.FS.Model;
    using Fanex.NAP.FS.Service;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class Service : BaseService
    {
        private IEnumerable<Match> _matches;     
        protected readonly FS.Service.ScannedOfficeIpService scannedOfficeIpService;
        private const int ChunkSize = 100;
        private const int SleepSize = 2;

        public Service() : base(Model.FraudCaseId.FlaggedAccount)
        {
            _matches = new List<Match>();            

            scannedOfficeIpService = new FS.Service.ScannedOfficeIpService();
        }

        

        public override BaseService CleanResult()
        {
            if (_matches == null || !_matches.Any())
            {
                return this;
            }

            _matches = null;

            return base.CleanResult();
        }

        public override BaseService Precondition()
        {
            _matches = scannedOfficeIpService.PreCondition<Match>();

            return base.Precondition();
        }

        public override BaseService Rule()
        {
            if (_matches == null || !_matches.Any()) return this;

            foreach (var chunk in _matches.Chunk(ChunkSize))
            {
                var ids = chunk.Select(m => m.MatchId);

                var matchIds = string.Join(",", ids);

                scannedOfficeIpService.Rule(matchIds);
            }

            Thread.Sleep(TimeSpan.FromSeconds(SleepSize));

            return base.Rule();
        }             
    }
}