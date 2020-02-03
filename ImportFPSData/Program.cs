using Fanex.Logging;
using Fanex.Logging.Sentry;
using Fanex.NAP.SPU.Core;
using Renci.SshNet;
using Sentry;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImportFPSData
{
   static class Program
    {
        private static string sshScript = "sh /etc/init.d/ImportMigrateFPSData.sh {0} {1}";

        static void Main(string[] args)
        {
            LogManager.SetDefaultLogCategory("Import-FPS-Data")
                   .Use(new List<ILogEngine>
                   {
                         new SentryLogging(new SentryEngineOptions
                         {
                             Dsn = new Dsn("http://5db4409de047476aacba9ee1f3c70e26@10.99.99.37:801/63")
                         })
                   });

            Console.WriteLine("Import CSV file is begining...");

            var authMethod = new PasswordAuthenticationMethod(AppSettings.SSHUser, AppSettings.SSHPassword);
            var connectionInfo = new ConnectionInfo(AppSettings.SSHHost, AppSettings.SSHPort, AppSettings.SSHUser, authMethod);
            List<string> directories = null;

            do
            {
                directories = Directory.GetDirectories(ConfigurationManager.AppSettings["csv-destination-root-path"]).ToList();

                if (directories != null && directories.Any())
                {
                    directories.ToList().ForEach(x =>
                    {
                        var files = Directory.GetFiles(x).ToList();

                        files.ForEach(f =>
                        {
                            var directoryInfo = Directory.GetParent(f);
                            var fileName = Path.GetFileName(f);

                            try
                            {
                                var result = ImportCSV(connectionInfo, directoryInfo.Name, fileName);

                                if (result)
                                {
                                    Console.WriteLine("-" + f + " ==> DONE.");

                                    if (File.Exists(f))
                                    {
                                        File.Delete(f);
                                    }
                                }
                                else
                                {
                                    var fullPath = Path.GetFullPath(f);
                                    var fileNameWithoutEx = Path.GetFileNameWithoutExtension(f);

                                    File.Move(f, fullPath + fileNameWithoutEx + "-Error.csv");

                                    LogService.Error($"Import csv file ERROR({f})");
                                }
                            }
                            catch (Exception ex)
                            {
                                LogService.Error(ex.Message + $". CVS file({f})");

                                throw;
                            }
                        });
                    });

                    directories.Clear();

                    Thread.Sleep(2);
                }
                else
                {
                    Console.WriteLine("There is no any CSV files.");
                }

                Console.WriteLine($"=======================================================");

            } while (directories != null && directories.Any());

            Console.WriteLine("Import CSV file is DONE. BYE.");

            Console.ReadKey();
        }

        private static bool ImportCSV(ConnectionInfo connectionInfo, string csvFolder, string csvFileName)
        {
            using (var client = new SshClient(connectionInfo))
            {
                client.Connect();

                var scriptCommand = string.Format(sshScript, csvFolder, csvFileName);

                var sshCommand = client.RunCommand(scriptCommand);

                if (!string.IsNullOrWhiteSpace(sshCommand.Error))
                {
                    LogService.Error(sshCommand.Error);

                    return false;
                }

                return true;
            }
        }
    }
}
