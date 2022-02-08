namespace Fanex.SPU.GraphDb.Data.Neo4j
{
    using Fanex.SPU.GraphDb.Data.Interface;
    using global::Neo4j.Driver;
    using System;

    public class Neo4jDriverFactory : IDriverFactory
    {
        public (Type driverType, object driver) GetDriver(string config)
        {
            var configs = config.Split(';');

            return (typeof(IDriver), GraphDatabase.Driver(configs[0], AuthTokens.Basic(configs[1], configs[2])
                , configNeo4j => configNeo4j.WithEncryptionLevel(EncryptionLevel.None)));

        }
    }
}
