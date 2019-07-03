using Fanex.Data.Repository;

namespace Fanex.NAP.FS.Service
{
    public class CheckBetService
    {
        public void Rule()
        {
            var repository = new DynamicRepository();
            var criteria = new CheckBetRuleCommand();

            repository.Execute(criteria);
        }
    }
}
