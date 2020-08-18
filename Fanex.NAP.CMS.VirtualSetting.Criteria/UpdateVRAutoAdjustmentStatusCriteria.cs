
namespace Fanex.NAP.CMS.VirtualSetting.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class UpdateVRAutoAdjustmentStatusCriteria : CriteriaBase
    {
        public string CustIDList { get; set; }
        public int Sporttype { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_VR_CustomerUpdAutoAdjustmentStatus";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIDList);
        }
    }
}
