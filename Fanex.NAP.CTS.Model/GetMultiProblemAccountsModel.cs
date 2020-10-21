using System;
using System.Collections.Generic;

namespace Fanex.NAP.CTS.Model
{
    public class GetMultiProblemAccountsModel
    {
        public List<MultipleProblemAccountsModel> ProblemAccounts { get; set; }

        public DateTime BackupTimingExecution { get; set; }
    }
}
