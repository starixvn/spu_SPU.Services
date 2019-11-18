using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.NAP.DCS.Core;
using Fanex.NAP.DCS.Services.Implement;
using Fanex.NAP.DCS.Services.Interface;
using Fanex.NAP.SPU.Core;
using Renci.SshNet;
using SharpRaven;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Fanex.NAP.DCS.IPRange.Console
{
    static class Program
    {
        private static string rangeIpCSVFileName = "IpRanges.csv";
        private static string rangeIPTXTFileName = "IpRanges.txt";
        private static string sshScript = "sh /etc/init.d/fps40_mysql_initial_ip_range.sh";
        private static int timeout = 5000;
        private static int fromId;
        private static int toId;

        private static IIPRangeService ipRangeService;
        private static string originalPath;
        private static ConnectionInfo connectionInfo;

        static void Main(string[] args)
        {
            DbSettingProviderManager
                .StartNewSession()
                .WithMySql()
                .WithSqlServer()
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

            ipRangeService = new IPRangeService();

            originalPath = AppDomain.CurrentDomain.BaseDirectory + @"\IpRangeData\";

            if (!Directory.Exists(originalPath))
            {
                Directory.CreateDirectory(originalPath);
            }

            var authMethod = new PasswordAuthenticationMethod(AppSettings.SSHUser, AppSettings.SSHPassword);
            connectionInfo = new ConnectionInfo(AppSettings.SSHHost, AppSettings.SSHPort, AppSettings.SSHUser, authMethod);

            try
            {
                var ipDBVersionDate = ipRangeService.GetIPDBVersion();
                var ipLatestIPDBLogDate = ipRangeService.GetLatestUpdatedDateIPTableLog();

                if (ipDBVersionDate.CompareTo(ipLatestIPDBLogDate) != 0)
                {
                    var filePath = Path.Combine(originalPath, rangeIpCSVFileName);

                    UpdateIpTableV6(ipDBVersionDate, filePath);
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
        }

        private static void UpdateIpTableV6(DateTime updatedDate, string filePath)
        {
            var rangeIpUpdatedPath = Path.Combine(originalPath, rangeIPTXTFileName);

            ipRangeService.GetRangeIpId(ref fromId, ref toId, AppSettings.SizeOfIpRange, rangeIpUpdatedPath);

            var ipV6Ranges = ipRangeService.GetIpV6Range(fromId, toId);

            if (ipV6Ranges != null && ipV6Ranges.Any())
            {
                ipRangeService.SaveCSV(ipV6Ranges, updatedDate, filePath);

                SaveACopyfileToServer(filePath, Path.Combine(AppSettings.SSHDestinationPath, rangeIpCSVFileName));

                using (var client = new SshClient(connectionInfo))
                {
                    client.Connect();

                    var sshCommand = client.RunCommand(sshScript);

                    if (string.IsNullOrWhiteSpace(sshCommand.Error))
                    {
                        var result = ipRangeService.UpdateIpRange(updatedDate);

                        if (result >= 0)
                        {                            
                            fromId = (toId + 1);
                            toId += SPU.Core.AppSettings.SizeOfIpRange;

                            ipRangeService.SaveRangeIpId(fromId, toId, rangeIpUpdatedPath);
                        }
                    }
                    else
                    {
                        LogService.Error(sshCommand.Error);
                    }
                }
            }
            else
            {
                ipRangeService.InsertNewTableLog(updatedDate);

                fromId = toId = 0;

                ipRangeService.SaveRangeIpId(fromId, toId, rangeIpUpdatedPath);
            }
        }

        private static void ExecuteCommand(string command, int timeout)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/C " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = "C:\\",
            };

            var process = Process.Start(processInfo);
            process.WaitForExit(timeout);

            process.Close();
        }

        private static void SaveACopyfileToServer(string filePath, string savePath)
        {
            var directory = Path.GetDirectoryName(savePath).Trim();

            var filenameToSave = Path.GetFileName(savePath);

            if (!directory.EndsWith("\\"))
                filenameToSave = "\\" + filenameToSave;

            var command = "NET USE " + directory + " /delete";
            ExecuteCommand(command, timeout);

            command = "NET USE " + directory + " /user:" + AppSettings.SSHUser + " " + AppSettings.SSHPassword;
            ExecuteCommand(command, timeout);

            if (!Directory.Exists(directory))
            {
                var parentDir = Directory.GetParent(directory);

                foreach (DirectoryInfo dir in parentDir.GetDirectories())
                {
                    dir.Delete(true);
                }

                Directory.CreateDirectory(directory);
            }

            var destinationDirInfo = new DirectoryInfo(directory);

            foreach (FileInfo file in destinationDirInfo.GetFiles())
            {
                file.Delete();
            }

            var destinationFilePath = directory + filenameToSave;

            File.Copy(filePath, destinationFilePath, true);

            File.Delete(filePath);
        }
    }
}
