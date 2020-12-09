using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Services
{
    public sealed class WrapperDataCenterService : DataCenterService
    {
        private WrapperDataCenterService()
        {
        }
        private static DataCenterService instance = null;
        public static DataCenterService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataCenterService();
                }
                return instance;
            }
        }
    }
}
