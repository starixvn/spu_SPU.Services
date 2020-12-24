namespace Fanex.NAP.CTS.Model
{
    using System;

    public class AssociationModel
    {
        public int AssociationId { get; set; }

        public int CustId { get; set; }

        public int DeviceId { get; set; }

        public int SubscriberId { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
