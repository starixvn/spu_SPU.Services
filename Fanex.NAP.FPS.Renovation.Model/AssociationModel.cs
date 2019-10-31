namespace Fanex.NAP.FPS.Renovation.Model
{
    public class AssociationModel
    {
        private string username2;
        private int? roleId;

        public long AssociationId { get; set; }

        public long DeviceId { get; set; }

        public int SubscriberId { get; set; }

        public int CustId { get; set; }

        public string SubscriberName { get; set; }

        public int IsDeposit { get; set; }

        public string Username { get; set; }

        public string Username2
        {
            get => username2 == string.Empty ? null : username2;
            set { username2 = value; }
        }

        public int? RoleId
        {
            get => roleId == 0 ? null : roleId;
            set { roleId = value; }
        }

        public int CurrencyId { get; set; }

        public string CurrencyName { get; set; }

        public string CreatedDate { get; set; }
    }
}