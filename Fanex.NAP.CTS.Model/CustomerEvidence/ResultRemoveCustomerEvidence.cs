namespace Fanex.NAP.CTS.Model
{
    using System.Collections.Generic;

    public class ResultRemoveCustomerEvidence
    {
        public DBResult DBResult { get; set; }

        public IEnumerable<CustomerEvidenceRemovedModel> CustEviRemovedList { get; set; }

    }
}