namespace Fanex.SPU.GraphDb.Data.Model
{
    public class ExecutionResult
    {
        public int AffectedNodes { get; set; }

        public int AffectedRelationship { get; set; }

        public static ExecutionResult operator +(ExecutionResult r1, ExecutionResult r2)
        {
            if (r1 == null)
            {
                return r2;
            }

            if (r2 == null)
            {
                return r1;
            }

            return new ExecutionResult
            {
                AffectedNodes = r1.AffectedNodes + r2.AffectedNodes,
                AffectedRelationship = r1.AffectedRelationship + r2.AffectedRelationship
            };
        }
    }

    public class ExecutionResult<T> : ExecutionResult
    {
        public T Data { get; set; }
    }
}
