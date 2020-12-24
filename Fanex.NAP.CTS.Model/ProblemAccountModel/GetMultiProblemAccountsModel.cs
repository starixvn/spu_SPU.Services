namespace Fanex.NAP.CTS.Model
{
    using System;
    using System.Collections.Generic;

    public class GetMultiProblemAccountsModel
    {
        public List<MultipleProblemAccountsModel> ProblemAccounts { get; set; }

        public DateTime BackupTimingExecution { get; set; }
    }
}
