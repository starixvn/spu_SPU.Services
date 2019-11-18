namespace Fanex.NAP.DCS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class CapturingTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_RawTransJson")]
        public string RawTrans { get; set; }

        public override string GetSettingKey() => "DCS_RT_RawTrans_Insert";

        public override bool IsValid() => true;
    }
}
