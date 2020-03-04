using EasyNetQ;
using EasyNetQ.Consumer;
using EasyNetQ.Topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.DCS.Queue
{
    public static class RabbitAdvancedBusConsumeExtension
    {
        public static IDisposable Consume(this IAdvancedBus bus, IQueue queue, Action<byte[], MessageProperties, MessageReceivedInfo> onMessage)
        {
            return bus.Consume(queue, (bytes, properties, info) => ExecuteSynchronously(() => onMessage(bytes, properties, info)));
        }

        public static IDisposable Consume(this IAdvancedBus bus, IQueue queue, Action<byte[], MessageProperties, MessageReceivedInfo> onMessage, Action<IConsumerConfiguration> configure)
        {
            return bus.Consume(queue, (bytes, properties, info) => ExecuteSynchronously(() => onMessage(bytes, properties, info)), configure);
        }

        private static Task ExecuteSynchronously(Action action)
        {
            var tcs = new TaskCompletionSource<object>();

            try
            {
                action();

                tcs.SetResult(null);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }

            return tcs.Task;
        }
    }
}
