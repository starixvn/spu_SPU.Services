namespace Fanex.NAP.SPU.Demo.JobService.Service
{
    using System.Collections.Generic;

    public interface IService
    {
        IEnumerable<object> GetPreviousTimingExecution(int paramId);
    }
}
