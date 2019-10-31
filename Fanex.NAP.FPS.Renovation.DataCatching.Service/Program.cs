using Fanex.NAP.FPS.Renovation.Core;
using Fanex.NAP.FPS.Renovation.Queue;
using Fanex.Oneyed.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NAP.FPS.Renovation.DataCatching.Service
{
    static class Program
    {       
        private static Dictionary<string, int> Subscribers;

        static void Main(string[] args)
        {
            var _rabbitMQ = new EasyNetQConnector(AppSettings.Host + ":" + AppSettings.Post,
                                       AppSettings.VirtualHost,
                                       AppSettings.PrefetchCount,
                                       AppSettings.UserName,
                                       AppSettings.Password);
            _rabbitMQ.CreateBus()
                     .DeclareExchange(AppSettings.Exchange)
                     .DeclareQueue(AppSettings.QueueName)
                     .DeclareRoutingKey(AppSettings.RoutingKey)
                     .Bind();

            _rabbitMQ.AdvanceBus.Consume(_rabbitMQ.Queue, x => x.Add<MessageQueue>((message, info) =>
            {
                ProcessMessage(message.Body);
            }));

            Subscribers = new Dictionary<string, int>();
        }

        private static void ProcessMessage(MessageQueue message)
        {
            try
            {
                if(message.Subscriber.Contains("CN88"))
                {
                    if (!Subscribers.ContainsKey(message.Subscriber))
                    {
                        Subscribers.Add(message.Subscriber, 0);

                        Console.WriteLine(message.Subscriber);
                    }
                    else
                    {
                        Subscribers[message.Subscriber] += 1;
                    }

                    if (Subscribers[message.Subscriber] <= AppSettings.TransBatch)
                    {
                        Utilities.WriteLog(message.Subscriber + ".txt", TransformMessageQueueToString(message));
                    }
                }
                            
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string TransformMessageQueueToString(MessageQueue message)
        {
            var result = new StringBuilder();

            result.Append(message.Account);
            result.Append("|");
            result.Append(message.Action);
            result.Append("|");
            result.Append(message.ActionResult);
            result.Append("|");
            result.Append(message.Device);
            result.Append("|");
            result.Append(message.IP);
            result.Append("|");
            result.Append(message.Subscriber);
            result.Append("|");
            result.Append(message.SubscriberDate);
            result.Append("|");
            result.Append(message.Token);
            result.Append("|");
            result.Append(message.TransDate);

            return result.ToString();
        }
    }
}
