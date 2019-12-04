using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Model
{
    public class CustomerMainDbModel
    {
        public int BatchId { get; set; }

        public int CustId { get; set; }

        public int CustSubId { get; set; }

        public string Username { get; set; }

        public string Username2 { get; set; }

        public string Site { get; set; }

        public int RoleId { get; set; }

        public int CurrencyId { get; set; }

        public string Currency { get; set; }

        public int Srecommend { get; set; }

        public int Mrecommend { get; set; }

        public int Recommend { get; set; }
    }
}
