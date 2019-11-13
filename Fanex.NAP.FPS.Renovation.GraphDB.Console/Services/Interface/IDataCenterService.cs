using Fanex.NAP.FPS.Renovation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.GraphDB.Console.Services.Interface
{
    public interface IDataCenterService
    {
        DataResult UpdateIsSyncByAssociationId(string associationXml);
    }
}
