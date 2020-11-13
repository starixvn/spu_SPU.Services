namespace Fanex.NAP.CTS.Model
{
    public class ProbationModel
    {
        public long CustId { get; set; }

        public int SportGroupId { get; set; }

        public int IsProbationLastDay { get; set; }  // 0. VIP & Normal OR 1.Update to Smart, High Risk, VIP & Normal  

        public int CategoryId { get; set; }

        public bool IsPassed { get; set; }
    }
}
