namespace Fanex.NAP.FS.Hedging.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.FS.Model;
    using Fanex.NAP.FS.Service;
    using Fanex.NAP.FS.XML;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class Service : BaseService
    {
        private IEnumerable<Match> matches;
        private IEnumerable<FlaggedCustomer> hedgingCustomers;
        protected readonly HedgingService hedgingService;
        private const int ChunkSize = 100;
        private const int SleepSize = 2;

        public Service() : base(Model.FraudCaseId.GroupBetting)
        {
            matches = new List<Match>();

            hedgingCustomers = new List<FlaggedCustomer>();

            hedgingService = new HedgingService();
        }

        private IEnumerable<FlaggedCustomer> GetFlaggedCustomer()
        {
            var param = new CustomerEvidenceList
            {
                CustIdList = string.Empty,
                EvidenceCodeList = string.Empty
            };

            return ApiHelper.Post<IEnumerable<FlaggedCustomer>>(AppSetting.CustomerEvidenceApi, param).Result;
        }

        public override BaseService CleanResult()
        {
            if (matches == null || !matches.Any())
            {
                return this;
            }

            matches = Enumerable.Empty<Match>();

            return base.CleanResult();
        }

        public override BaseService Precondition()
        {
            matches = hedgingService.PreCondition<Match>();

            return base.Precondition();
        }

        public override BaseService Rule()
        {
            hedgingCustomers = GetFlaggedCustomer();

            ExecuteBySeparatedThreads();

            return base.Rule();
        }

        private void ExecuteBySeparatedThreads()
        {
            if (matches == null || !matches.Any())
            {
                return;
            }
            var matchChunks = matches.Chunk(ChunkSize);

            foreach (var matches in matchChunks)
            {
                ExecuteMatches(matches.ToList());
            }

            Thread.Sleep(TimeSpan.FromSeconds(SleepSize));
        }

        private void ExecuteMatches(List<Match> matches)
        {
            try
            {
                var matchIds = string.Join(",", (matches).Select(t => t.MatchId.ToString()));
                var flaggedAccounts = XmlHelper.ToXmlString(hedgingCustomers);

                hedgingService.Rule(matchIds, flaggedAccounts);

            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Hedging service - ExecuteMatches Exeption {logMessage}");
            }
        }
    }
}
