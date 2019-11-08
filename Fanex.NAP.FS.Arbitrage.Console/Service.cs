namespace Fanex.NAP.FS.Arbitrage.Console
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
    using System.Threading;
    using System.Threading.Tasks;

    public class Service : BaseService
    {
        private readonly FS.Service.ArbitrageService _service;
        private IEnumerable<ArbitrageTicket> _ticket;
        private const int SleepTime = 10;

        public Service() : base(Model.FraudCaseId.Arbitrage)
        {
            _service = new FS.Service.ArbitrageService();
            _ticket = new List<ArbitrageTicket>();
        }

        public override BaseService CleanResult()
        {
            if (_ticket == null || !_ticket.Any())
            {
                return this;
            }

            _ticket = Enumerable.Empty<ArbitrageTicket>();

            return base.CleanResult();
        }

        public override BaseService Precondition()
        {
            _ticket = _service.PreCondition<ArbitrageTicket>();

            return base.Precondition();
        }

        public override BaseService Rule()
        {
            ExecuteBySeparatedThreads();

            return base.Rule();
        }

        private void ExecuteBySeparatedThreads()
        {
            if (_ticket == null || !_ticket.Any())
            {
                return;
            }

            var ticketGroupBy = _ticket
                .GroupBy(t => t.MatchId)
                .Select(x => new
                {
                    MatchId = x.Key,
                    ArbitrageTicketList = x.ToList()
                }).ToList();

            var options = new ParallelOptions { MaxDegreeOfParallelism = 1 };

            Parallel.ForEach(ticketGroupBy, options, ticket =>
            {
                var custIds = ticket.ArbitrageTicketList
                    .Select(t => t.CustId)
                    .Distinct()
                    .ToList();

                var groupCustomer = GroupCustomer(custIds);

                ExecuteGroupCustomer(ticket.MatchId, groupCustomer, ticket.ArbitrageTicketList);

                Thread.Sleep(SleepTime);
            });
        }

        private List<ArbitrageCustomer> GroupCustomer(List<int> custIds)
        {
            var arbitrageCustomers = custIds.Chunk(500).ToList();

            var taskCustomers = new List<Task<IEnumerable<ArbitrageCustomer>>>();

            foreach (var arbitrageCustomer in arbitrageCustomers)
            {
                var taskCustomer = GroupCustomer(arbitrageCustomer);

                taskCustomers.Add(taskCustomer);

                Thread.Sleep(SleepTime);
            }

            Task.WhenAll(taskCustomers.ToArray());

            var result = taskCustomers.SelectMany(t => t.Result).ToList();

            if (result != null && result.Any())
            {
                var missingCustId = custIds.Where(x => !result.Any(z => z.CustId == x)).ToList();

                if (missingCustId != null && missingCustId.Any())
                {
                    var maxGroupId = result.Max(x => x.GroupId);

                    missingCustId.Select(x => new { missingCustId = x, missingGroupId = ++maxGroupId })
                        .ToList()
                        .ForEach(x => result.Add(new ArbitrageCustomer
                        {
                            CustId = x.missingCustId,
                            GroupId = x.missingGroupId
                        }));
                }

                return result;
            }

            return custIds.Select((x, i) => new ArbitrageCustomer { CustId = x, GroupId = i }).ToList();
        }

        private async Task<IEnumerable<ArbitrageCustomer>> GroupCustomer(IEnumerable<int> custids)
        {
            try
            {
                var result = await ApiHelper.Post<IEnumerable<ArbitrageCustomer>>(AppSetting.GroupCustomerOneLevelPath, custids);                               

                return result;
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(
                    $"Arbitrage service - GroupCustomer Exception: Cannot get Group Customer from Web API - Exception: {logMessage}");

                return Enumerable.Empty<ArbitrageCustomer>();
            }
        }

        private void ExecuteGroupCustomer(int matchId, List<ArbitrageCustomer> associations, List<ArbitrageTicket> matchIds)
        {
            try
            {
                if (matchIds.Any() && associations.Any())
                {
                    var xmlMatchIds = matchIds.ToXmlString();
                    var xmlAssociations = associations.ToXmlString();

                    _service.Rule(xmlMatchIds, xmlAssociations);
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(
                    $"Arbitrage service - MatchId: {matchId} - ExecuteGroupCustomer Exeption: {logMessage}");
            }
        }
    }
}