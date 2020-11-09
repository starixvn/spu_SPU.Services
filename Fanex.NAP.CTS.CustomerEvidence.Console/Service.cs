namespace Fanex.NAP.CTS.CustomerEvidence.Console
{
    using Fanex.NAP.CTS.Services;
    using Fanex.NAP.CTS.Services.Interface;
    using System.Linq;

    public class Service
    {
        private IDataCenterService dataService;

        public Service()
        {
            dataService = new DataCenterService();
        }

        public void GetCustomerEvidence()
        {
            var customerEvidenceRaws = dataService.GetCustomerEvidenceRaw();

            if (customerEvidenceRaws != null && customerEvidenceRaws.Any())
            {

            }
        }

    }
}
 