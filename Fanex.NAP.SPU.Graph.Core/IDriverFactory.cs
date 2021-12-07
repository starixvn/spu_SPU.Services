using System;

namespace Fanex.NAP.SPU.Graph.Core
{
    public interface IDriverFactory
    {
        (Type driverType,object driver) GetDriver(string config);
    }
}