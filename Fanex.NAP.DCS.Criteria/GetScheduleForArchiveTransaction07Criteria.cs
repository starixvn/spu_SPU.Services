namespace Fanex.NAP.DCS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetScheduleForArchiveTransaction07Criteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "DCS_DC_ArchiveData_Transaction07_Schedule";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
