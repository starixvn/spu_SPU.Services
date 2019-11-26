using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria
{
    public class TransformDCSTransactionIPToCTSAssociationByIPCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransactionIPJson")]
        public string TransactionJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_TransformDCSTransactionIPToCTSAssociationByIP";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
