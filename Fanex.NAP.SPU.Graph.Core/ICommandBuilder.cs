namespace Fanex.NAP.SPU.Graph.Core
{
    using System;

    public interface ICommandBuilder
    {
        Type DriverType { get; }

        string BuildCommand(GraphSetting setting);
    }
}