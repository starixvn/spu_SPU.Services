namespace Fanex.SPU.GraphDb.Data.Interface
{
    using System;

    public interface ICommandBuilder
    {
        Type DriverType { get; }

        string BuildCommand(GraphSetting setting);
    }
}
