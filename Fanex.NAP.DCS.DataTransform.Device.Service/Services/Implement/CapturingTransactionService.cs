namespace Fanex.NAP.FPS.Renovation.CapturingData.Service.Services
{
    using Fanex.NAP.FPS.Renovation.Criteria;
    using Fanex.NAP.FPS.Renovation.DataAccess;
    using Fanex.Oneyed.Infrastructure.Models;

    public class CapturingDataService : ICapturingDataService
    {
        public CapturingDataService()
        {
        }

        public void InsertTransaction(MessageQueue message)
        {
            var criteria = new CapturingTransactionCriteria()
            {
                Account = message.Account,
                Action = message.Action,
                ActionResult = message.ActionResult,
                Device = message.Device,
                Ip = message.IP,
                Subscriber = message.Subscriber,
                SubscriberDate = message.SubscriberDate,
                Token = message.Token,
                CaptchaResult = string.Empty
            };

            DataService.UpdateData(criteria, string.Empty);
        }

    }
}
