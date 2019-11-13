using Fanex.NAP.CTS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Services.Interface
{
    public interface IDataCenterService
    {
        void InsertAccount(List<RawAccountModel> accountList);
    }
}
