namespace Fanex.NAP.CTS.Model
{
    using System.Collections.Generic;

    public class InsertRobotUserResultModel
    {
        public string ErrorMessage { get; set; }

        public IEnumerable<InsertProblemAccountsModel> CustClassification { get; set; }
    }
}
