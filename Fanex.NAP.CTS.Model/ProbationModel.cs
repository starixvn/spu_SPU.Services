namespace Fanex.NAP.CTS.Model
{
    public class ProbationModel
    {
        public long CustId { get; set; }

        public int SportGroupId { get; set; }

        public int CategoryId { get; set; }

        public bool IsPassed { get; set; }

        public bool Is60Day { get; set; }
    }
}
