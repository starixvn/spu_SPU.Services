namespace Fanex.SPU.GraphDb.Data.Interface
{
    using Fanex.SPU.GraphDb.Data.Model;
    using System.Collections.Generic;

    public interface ICommandExecutor<TDriver>
    {
        ExecutionResult<IEnumerable<T>> Execute<T>(TDriver driver, string command, string dbName, object parameters = null);

        ExecutionResult Execute(TDriver driver, string command, string dbName, object parameters = null);

        ExecutionResult<T> ExecuteSingle<T>(TDriver driver, string command, string dbName, object parameters = null);
    }
}
