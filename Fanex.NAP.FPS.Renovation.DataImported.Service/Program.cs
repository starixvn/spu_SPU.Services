using EasyNetQ;
using Fanex.NAP.FPS.Renovation.Core;
using Fanex.NAP.FPS.Renovation.Queue;
using Fanex.Oneyed.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Fanex.NAP.FPS.Renovation.DataImported.Service
{
   static class Program
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

            //var ipfrom = Utilities.long2ip(281470698564864);
            //var ipto = Utilities.IPToLong("::ffff:100:0");
            

            //Console.WriteLine(ipfrom);
            //Console.WriteLine(ipto);

            // codefrom = Utilities.ConvertIntToIpAddress(16779264);
           // var codeto = Utilities.ConvertIpAddressToInt(ipto);

           // var ipfrom = Utilities.ConvertIpAddressToInt(codefrom);

            //Console.WriteLine(codefrom);
            // Console.WriteLine(ipfrom);


            //var strIPv6 = "::ffff:100:0".Ip2Long();

            //var strIPv4 = "1.0.8.0".Ip2Long();


            //Console.WriteLine(strIPv6.IpNumber + " - " + strIPv6.IsIpV6);
            //Console.WriteLine(strIPv4.IpNumber + " - " + strIPv4.IsIpV6);

            //Console.WriteLine(ipv4Addr.ToUncompressedString());
            //byte[] octets = ipv6Addr.GetAddressBytes();

            //string text = String.Join(",", octets.Select(x => string.Format("0x{0:X2}", x)));

            //var aa = IPAddress.Parse(text);

            //Console.WriteLine(text);



            Console.ReadKey();
        }

        static List<string> loadCsvFile(string filePath)
        {
            var reader = new StreamReader(File.OpenRead(filePath));
            List<string> searchList = new List<string>();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var lines = line.Split(',');
                if(lines.Any())
                {
                    searchList.Add(lines[1]);
                }                
            }

            return searchList;
        }

        static void ReadDataFromConfig()
        {
            var files = Directory.GetFiles(@"D:\Projects\FPS 4.0\SubscriberTickets");

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);

                if (fileName == "CN88.txt" || fileName == "Alpha789Y.txt" || fileName == "12BET.txt")
                {
                    Utilities.DeleteLog(fileName);

                    var dataLog = File.ReadAllLines(file);
                    
                    if (dataLog.Length > 0)
                    {
                        var usernames = loadCsvFile(@"D:\Projects\FPS 4.0\SubscriberTickets\"+ fileNameWithoutExtension + ".csv");

                        Executing(fileName, dataLog, usernames);
                    }

                    Console.WriteLine(Path.GetFileName(file));
                }
            }
        }

        private static void Executing(string fileName,  string[] dataLog, List<string> usernames)
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

                    Utilities.WriteLog(fileName, string.Join("|", splitingData));                                        

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
                    Console.WriteLine(log);
                }
            }            
        }
    }
}
