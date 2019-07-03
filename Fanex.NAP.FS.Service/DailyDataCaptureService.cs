using Fanex.Data.Repository;

namespace Fanex.NAP.FS.Service
{
    public class DailyDataCaptureService
    {
        public void Rule()
        {
            var repository = new DynamicRepository();
            var criteria = new DailyDataCaptureRuleCommand();

            repository.Execute(criteria);
        }
    }
}
