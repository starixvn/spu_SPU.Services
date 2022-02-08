namespace Fanex.SPU.GraphDb.Data.Interface
{
    using System;

    public interface IDriverFactory
    {
        (Type driverType, object driver) GetDriver(string config);
    }
}
