namespace Fanex.NAP.FPS.Renovation.RawTransaction.Service
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.FPS.Renovation.Core;
    using Fanex.NAP.FPS.Renovation.Model;
    using Fanex.NAP.FPS.Renovation.Queue;
    using Fanex.Oneyed.Infrastructure.Models;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceProcess;
    using System.Timers;

    public partial class Runner : ServiceBase
    {
        private readonly EasyNetQConnector _rabbitMQ;
        private readonly ICapturingDataService _capturingDataService;
        private readonly List<RawTransactionModel> _transactionList;
        private readonly System.Timers.Timer _timer;
        private DateTime _lastInsertTrans;        

        public Runner()
        {
            InitializeComponent();

            _capturingDataService = new CapturingDataService();

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

            _transactionList = new List<RawTransactionModel>();

            _timer = new System.Timers.Timer();
            _timer.Elapsed += InsertTransactionByTimer;
            _timer.Interval = AppSettings.Interval;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                ConsumerTransactions();

                if (!_timer.Enabled)
                {
                    _timer.Start();
                }

                LogService.Info($"Raw trans service is STARTED at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }

        protected override void OnStop()
        {
            _rabbitMQ.Disposed();

            LogService.Info($"Raw trans service is STOPED at {DateTime.Now}");
        }

        protected override void OnShutdown()
        {
            _rabbitMQ.Disposed();

            LogService.Info($"Raw trans service is STOPED at {DateTime.Now}");
        }

        private void ConsumerTransactions()
        {
            _rabbitMQ.AdvanceBus.Consume(_rabbitMQ.Queue, x => x.Add<MessageQueue>((message, info) =>
            {
                ProcessMessage(message.Body);
            }));
        }

        private void ProcessMessage(MessageQueue message)
        {
            var trans = TransformRawTransaction(message);

            _transactionList.Add(trans);

            if (_transactionList.Count >= AppSettings.TransBatch)
            {
                InsertTransaction();
            }
        }

        private void InsertTransaction()
        {
            try
            {
                _capturingDataService.InsertTransaction(_transactionList);

                _lastInsertTrans = DateTime.Now;
                _transactionList.Clear();
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                _lastInsertTrans = DateTime.Now;
                _transactionList.Clear();
            }
        }

        private RawTransactionModel TransformRawTransaction(MessageQueue message)
        {
            var invalidDevice = string.Empty;                       
            var deviceCode = Utilities.GetDeviceCodeFromCapturedData(message.Device);
            var deviceInfo = _capturingDataService.GetDeviceInfo(deviceCode);
            var url = Utilities.GetURLFromDeviceCode(message.Device);
            var userAgent = Utilities.GetUserAgentFromDeviceCode(message.Device);
            var transBit = new BitArray(16);

            if (string.IsNullOrWhiteSpace(deviceInfo.DeviceNo) || deviceInfo.DeviceNo == "N/A")
            {
                invalidDevice = string.IsNullOrWhiteSpace(deviceInfo.DeviceNo) ? "Empty" : "N/A";
                deviceInfo.DeviceNo = string.Empty;
                transBit.Set((int)TransStatus.DeviceCodeIsEmpty, true);
            }
            else
            {
                if (!Utilities.IsGuidDeviceId(deviceInfo.DeviceNo) || deviceInfo.InvalidDeviceId)
                {
                    invalidDevice = deviceCode;
                    deviceInfo.DeviceNo = string.Empty;                    
                    transBit.Set((int)TransStatus.InvalidDI, true);
                }
            }

            if(ContainsUnicodeCharacter(message.Account))
            {
                transBit.Set((int)TransStatus.InvalidLoginName, true);
            }

            return new RawTransactionModel
            {
                DeviceCode = deviceInfo.DeviceNo,
                InvalidDevice = invalidDevice,
                TransStatus = Core.Utilities.ConvertBitArray2Int(transBit),
                LoginName = message.Account,
                SubscriberName = message.Subscriber,
                TransDate = Convert.ToDateTime(message.TransDate.ToString("yyyy-MM-dd HH:mm:ss.ffff")),
                IP = message.IP,
                PluginID = 0,
                URL = string.IsNullOrWhiteSpace(url) ? ConvertNA2Empty(deviceInfo.HRef) : url.Split('?')[0],
                ActionResult = message.ActionResult,
                UserAgent = string.IsNullOrWhiteSpace(userAgent) ? ConvertNA2Empty(deviceInfo.UserAgent) : userAgent,
                FingerprintCode = deviceInfo.DeviceFPCode != "N/A" ? deviceInfo.DeviceFPCode : string.Empty,
                FingerprintMoreInfo = deviceInfo.DeviceFPCode != "N/A" ? EncodeCapturedData(deviceInfo) : string.Empty
            };
        }

        private void InsertTransactionByTimer(object sender, ElapsedEventArgs e)
        {
            if ((DateTime.Now - _lastInsertTrans).Minutes >= 1 && _transactionList.Count != 0)
            {
                InsertTransaction();
            }
        }

        private static string EncodeCapturedData(DeviceInfoModel deviceInfo)
        {
            return Cipher.Encrypt(string.Format("{0}#{1}#{2}#{3}#{4}",
                deviceInfo.Battery,
                deviceInfo.CapturedDate,
                deviceInfo.OSName,
                deviceInfo.TimeZone,
                deviceInfo.LocalIps));
        }

        private static string ConvertNA2Empty(string data)
        {
            return data != "N/A" ? data : string.Empty;
        }

        private static bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
        }       
    }
}