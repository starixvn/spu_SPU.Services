using EasyNetQ;
using Fanex.NAP.SPU.Core;
using Fanex.NAP.SPU.Queue;
using Fanex.Oneyed.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.NAP.DCS.ImportData.Console
{
    public class Program
    {
        private static EasyNetQConnector _rabbitMQ;

        static void Main(string[] args)
        {
            _rabbitMQ = new EasyNetQConnector(AppSettings.Host + ":" + AppSettings.Post,
                                     AppSettings.VirtualHost,
                                     AppSettings.PrefetchCount,
                                     AppSettings.UserName,
                                     AppSettings.Password);

            _rabbitMQ.CreateBus()
                    .DeclareExchange(AppSettings.Exchange)
                    .DeclareQueue(AppSettings.QueueName)
                    .DeclareRoutingKey(AppSettings.RoutingKey)
                    .Bind();

            ReadDataFromConfig();
        }

        static List<string> loadCsvFile(string filePath)
        {
            var reader = new StreamReader(File.OpenRead(filePath));
            List<string> searchList = new List<string>();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var lines = line.Split(',');
                if (lines.Any())
                {
                    searchList.Add(lines[1]);
                }
            }

            return searchList;
        }

        static void ReadDataFromConfig()
        {
            var localPath = @"D:\Projects\FPS 4.0\SubscriberTickets";

            var files = Directory.GetFiles(localPath);

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);

                if (fileName == "CN88.txt" || fileName == "Alpha789Y.txt" || fileName == "12BET.txt")
                {
                    SPU.Core.Utilities.DeleteLog(fileName);

                    var dataLog = File.ReadAllLines(file);

                    if (dataLog.Length > 0)
                    {
                        var usernames = loadCsvFile(localPath + "\\" + fileNameWithoutExtension + ".csv");

                        Executing(fileName, dataLog, usernames);
                    }

                    System.Console.WriteLine(Path.GetFileName(file));
                }
            }
        }

        private static void Executing(string fileName, string[] dataLog, List<string> usernames)
        {
            var counter = 0;
            var random = new Random();

            foreach (var log in dataLog)
            {
                var splitingData = log.Split('|');

                if (splitingData.Length >= 9)
                {
                    counter++;

                    int indexUsername = random.Next(usernames.Count);

                    splitingData[0] = usernames[indexUsername];

                    SPU.Core.Utilities.WriteLog(fileName, string.Join("|", splitingData));

                    var objectData = new MessageQueue
                    {
                        Account = splitingData[0],
                        Action = splitingData[1],
                        ActionResult = splitingData[2],
                        Device = splitingData[3],
                        IP = splitingData[4],
                        Subscriber = splitingData[5],
                        SubscriberDate = Convert.ToDateTime(splitingData[6]),
                        Token = splitingData[7],
                        TransDate = Convert.ToDateTime(splitingData[8]),
                    };

                    var message = new Message<MessageQueue>(objectData, new MessageProperties { DeliveryMode = 2, Priority = 9 });

                    _rabbitMQ.AdvanceBus.PublishAsync(_rabbitMQ.Exchange, _rabbitMQ.RoutingKey, false, message);
                }
                else
                {
                    System.Console.WriteLine(log);
                }
            }
        }
    }
}
