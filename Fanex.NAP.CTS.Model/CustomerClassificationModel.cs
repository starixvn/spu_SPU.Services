namespace Fanex.NAP.CTS.Model
{
    using System;

    public class CustomerClassificationModel
    {
        public long CustId { get; set; }

        public int SportGroupId { get; set; }

        public int CategoryId { get; set; }

        public DateTime CreatedTime { get; set; }       
    }
}