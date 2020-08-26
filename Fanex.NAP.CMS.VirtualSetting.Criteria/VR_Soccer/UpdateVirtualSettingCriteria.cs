namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// UpdateVirtualSettingCriteria Class.
    /// </summary>
    public class UpdateVirtualSettingCriteria : CriteriaBase
    {
        public string VirtualMultiplier { get; set; }

        public int UserId { get; set; }

        public string IP { get; set; }

        public short SportType { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_CustomerUpdVirtualMultiplier_Multiple";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
