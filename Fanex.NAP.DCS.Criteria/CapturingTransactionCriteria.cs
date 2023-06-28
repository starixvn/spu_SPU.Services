namespace Fanex.NAP.DCS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class CapturingTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_RawTransJson")]
        public string RawTrans { get; set; }

        [SpParam(Ignored = true)]
        public bool IsTransOfDBExtra { get; set; }

        public override string GetSettingKey() 
        {
            if (this.IsTransOfDBExtra)
            {
                return "DCS_ET_Transform_RawTrans_Insert";
            }
            else
            {
                return "DCS_DC_Transform_RawTrans_Insert";
            }            
        }

        public override bool IsValid() => true;
    }
}
