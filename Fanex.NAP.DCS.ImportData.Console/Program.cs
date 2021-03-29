using EasyNetQ;
using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.Logging;
using Fanex.Logging.Sentry;
using Fanex.NAP.SPU.Core;
using Fanex.NAP.SPU.Queue;
using Fanex.Oneyed.Infrastructure.Models;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Fanex.NAP.DCS.ImportData.Console
{
    public class Program
    {
        private static EasyNetQConnector _rabbitMQ;

        static void Main(string[] args)
        {
            DbSettingProviderManager
                 .StartNewSession()
                 .WithMySql()
                 .UseDefaultDbSettingProvider(resourcePath: AppSettings.AppDataSetting)
                 .UseDefaultConnectionStringProvider()
                 .Run();

            LogManager
               .SetDefaultLogCategory(AppSettings.FanexLogCategory)
               .Use(new List<ILogEngine>
               {
                    new SentryLogging(new SentryEngineOptions
                    {
                        Dsn = new Dsn(AppSettings.SentryLogging)
                    })
               });

            _rabbitMQ = new EasyNetQConnector(AppSettings.RabbitMQ_Host + ":" + AppSettings.RabbitMQ_Post,
                                     AppSettings.RabbitMQ_VirtualHost,
                                     AppSettings.RabbitMQ_PrefetchCount,
                                     AppSettings.RabbitMQ_UserName,
                                     AppSettings.RabbitMQ_Password);

            _rabbitMQ.CreateBus()
                    .DeclareExchange(AppSettings.RabbitMQ_Exchange)
                    .DeclareQueue(AppSettings.RabbitMQ_QueueName)
                    .DeclareRoutingKey(AppSettings.RabbitMQ_RoutingKey)
                    .Bind();

            var ecServices = new ECServices();

            int menu;
            int sizeOfBatch = 0;
            int delayTime = 0;
            Timer timer = new Timer();
            do
            {
                System.Console.WriteLine("WELCOME TO EC 2021 LOADING TEST TOOL\n");

                System.Console.WriteLine("***********************************");

                System.Console.WriteLine("Please select a number from 1 to 5");

                System.Console.WriteLine("***********************************");

                System.Console.WriteLine("1. Size of batch");
                System.Console.WriteLine("2. Interval time(ms)");
                System.Console.WriteLine("3. Start");
                System.Console.WriteLine("4. Pause");
                System.Console.WriteLine("0. End");

                System.Console.Write("Please choose your number: ");

                var result = Int32.TryParse(System.Console.ReadLine(), out menu);

                if (result)
                {
                    switch (menu)
                    {
                        case 1:
                            {
                                System.Console.Write("1. Size of batch: ");

                                if (!Int32.TryParse(System.Console.ReadLine(), out sizeOfBatch))
                                {
                                    System.Console.WriteLine("DDDD-MMMM");
                                    menu = 100;
                                }
                                else
                                {
                                    System.Console.WriteLine($"\t-BATCH SIZE IS SETTED: {sizeOfBatch}");
                                }
                                break;
                            }
                        case 2:
                            {
                                System.Console.Write("2. Interval time(ms): ");

                                if (!Int32.TryParse(System.Console.ReadLine(), out delayTime))
                                {
                                    System.Console.WriteLine("DDDD-MMMM");
                                    menu = 100;
                                }
                                else
                                {
                                    System.Console.WriteLine($"\t-DELAY TIME IS SETTED: {delayTime}");
                                }
                                break;
                            }
                        case 3:
                            {
                                System.Console.WriteLine("3. Start");

                                timer.Interval = delayTime;
                                timer.Elapsed += (Object source, ElapsedEventArgs e) => OnTimedEvent(source, e, sizeOfBatch, ecServices);

                                timer.Enabled = true;

                                System.Console.WriteLine($"\t-JOB IS STARTING AT {DateTime.Now}");

                                break;
                            }
                        case 4:
                            {
                                System.Console.WriteLine("4. Pause");

                                timer.Stop();

                                System.Console.WriteLine($"\t-JOB IS STOPPED.");

                                break;
                            }
                        default:
                            {
                                if (menu == 0)
                                {
                                    System.Console.WriteLine("Thank you for your using.");
                                    System.Console.WriteLine("Tool will be end in 5s.\n");
                                    var currentTime = DateTime.Now;
                                    int counter = 1;
                                    Timer t = new Timer(1000);
                                    t.Elapsed += (Object source, ElapsedEventArgs e) =>
                                    {

                                        System.Console.WriteLine("Countdown: " + (counter++));
                                        if (currentTime.AddSeconds(5) < DateTime.Now)
                                        {
                                            System.Environment.Exit(-1);
                                        }

                                    };
                                    t.Enabled = true;

                                    return;
                                }

                                break;
                            }
                    }

                    System.Console.WriteLine("====================================\n\n");
                }
                else
                {
                    System.Console.WriteLine("\n\nDDDDDDDDDDD-MMMMMMMMMMM\n\n");
                    menu = 100;
                }
            }
            while (menu != 0);
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e, int sizeOfBatch, ECServices services)
        {
            try
            {
                var rawTrans = services.GetECRawTrans(sizeOfBatch);

                if (rawTrans != null && rawTrans.Any())
                {
                    rawTrans.ToList().ForEach(trans => {
                        var objectData = new MessageQueue
                        {
                            Account = trans.LoginName,
                            Action = trans.Action,
                            ActionResult = trans.ActionResult,
                            Device = trans.DeviceCode, // DI
                            IP = trans.Ip,
                            Subscriber = trans.SubscriberName,
                            SubscriberDate = DateTime.Now,
                            Token = string.Empty,
                            TransDate = trans.TransTime,
                        };

                        var message = new Message<MessageQueue>(objectData, new MessageProperties { DeliveryMode = 2, Priority = 9 });

                        _rabbitMQ.AdvanceBus.PublishAsync(_rabbitMQ.Exchange, _rabbitMQ.RoutingKey, false, message);
                    });
                    

                    var result = JsonConvert.SerializeObject(rawTrans);

                    LogService.Info(result);
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
            }
            
        }

        //static List<string> loadCsvFile(string filePath)
        //{
        //    var reader = new StreamReader(File.OpenRead(filePath));
        //    List<string> searchList = new List<string>();

        //    while (!reader.EndOfStream)
        //    {
        //        var line = reader.ReadLine();

        //        searchList.Add(line.Trim());
        //    }

        //    return searchList;
        //}

        //static void ReadDataFromConfig(string localPath)
        //{            
        //    //var localPath = @"D:\Projects\FPS 4.0\SubscriberTickets";

        //    var files = Directory.GetFiles(localPath);

        //    foreach (var file in files)
        //    {
        //        var fileName = Path.GetFileName(file);

        //        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);

        //        var dataLog = File.ReadAllLines(file);

        //        if (dataLog.Length > 0)
        //        {
        //            var usernames = loadCsvFile(localPath + "\\" + fileNameWithoutExtension + ".csv");

        //            Executing(fileName, dataLog, usernames);
        //        }

        //        System.Console.WriteLine(Path.GetFileName(file));
        //    }
        //}

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
                        SubscriberDate = DateTime.Now,
                        Token = splitingData[7],
                        TransDate = DateTime.Now,
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
