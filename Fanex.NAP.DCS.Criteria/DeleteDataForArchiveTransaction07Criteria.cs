namespace Fanex.NAP.DCS.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class DeleteDataForArchiveTransaction07Criteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ArchivedDate")]
        public DateTime ArchivedDate { get; set; }

        [SpParam(SqlParamName = "ip_NoOfRecord")]
        public int BatchSize { get; set; }

        [SpParam(SqlParamName = "ip_Completed")]
        public bool IsCompleted { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_ArchiveData_Transaction07_Delete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
