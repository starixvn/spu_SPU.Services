using Fanex.NAP.FS.XML;

namespace Fanex.NAP.FS.Model
{
    [XName("Match")]
    public class ArbitrageTicket
    {
        public int MatchId { get; set; }

        public int CustId { get; set; }

        [XName("BetType")]
        public int Bettype { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public double Handicap { get; set; }
    }
}