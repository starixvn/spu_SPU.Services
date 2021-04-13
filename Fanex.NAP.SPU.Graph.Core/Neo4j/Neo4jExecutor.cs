namespace Fanex.NAP.SPU.Graph.Core.Neo4j
{
    using global::Neo4j.Driver;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Neo4jExecutor : ICommandExecutor<IDriver>
    {
        private int affectedNodes = 0, affectedRelationship = 0;

        public ExecutionResult<T> ExecuteSingle<T>(IDriver driver, string command, object parameters = null)
        {
            T data = default;
            using (driver)
            {
                var session = driver.AsyncSession();
                var taskResult = session.WriteTransactionAsync(async tx =>
                {
                    var result = await tx.RunAsync(command, parameters);
                    var tempData = result.As<IRecord>();
                    data = ParseRecord<T>(tempData);

                    var counters = result.ConsumeAsync().Result.Counters;
                    affectedNodes = counters.NodesCreated + counters.NodesDeleted;
                    affectedRelationship = counters.RelationshipsCreated + counters.RelationshipsDeleted;
                });

                taskResult.Wait();
            }

            return new ExecutionResult<T>
            {
                AffectedNodes = affectedNodes,
                AffectedRelationship = affectedRelationship,
                Data = data
            };
        }

        public ExecutionResult<IEnumerable<T>> Execute<T>(IDriver driver, string command, object parameters = null)
        {
            IEnumerable<T> data = null;
            using (driver)
            {
                var session = driver.AsyncSession();
                var taskResult = session.WriteTransactionAsync(async tx =>
                {
                    var result = await tx.RunAsync(command, parameters);
                    var tempData = await result.ToListAsync(r => r.As<IRecord>());
                    data = tempData.Select(ParseRecord<T>).ToArray();

                    var counters = result.ConsumeAsync().Result.Counters;
                    affectedNodes = counters.NodesCreated + counters.NodesDeleted;
                    affectedRelationship = counters.RelationshipsCreated + counters.RelationshipsDeleted;
                });

                taskResult.Wait();
            }

            return new ExecutionResult<IEnumerable<T>>
            {
                AffectedNodes = affectedNodes,
                AffectedRelationship = affectedRelationship,
                Data = data
            };
        }

        public ExecutionResult Execute(IDriver driver, string command, object parameters = null)
        {

            using (driver)
            {
                var session = driver.AsyncSession();
                var taskResult = session.WriteTransactionAsync(async tx =>
                {
                    var result = await tx.RunAsync(command, parameters);

                    var counters = result.ConsumeAsync().Result.Counters;
                    affectedNodes = counters.NodesCreated + counters.NodesDeleted;
                    affectedRelationship = counters.RelationshipsCreated + counters.RelationshipsDeleted;
                });

                taskResult.Wait();
            }

            return new ExecutionResult
            {
                AffectedNodes = affectedNodes,
                AffectedRelationship = affectedRelationship
            };
        }

        private T ParseRecord<T>(IRecord record)
        {
            var t = typeof(T);

            if (t.IsPrimitive || t == typeof(string) || t == typeof(decimal))
            {
                return record[0].As<T>();
            }
            else if (t == typeof(DateTime))
            {
                object obj = DateTime.Parse(record[0].As<string>());

                return (T)obj;
            }

            return JObject.FromObject(record.Values).ToObject<T>();
        }
    }
}