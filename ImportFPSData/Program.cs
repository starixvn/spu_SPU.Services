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
using System.Threading;

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
                             Dsn = new Dsn("https://0640ef4e164f4a1da89a5183e1600b78@sentry.starixplay.com/123")
                         })
                   });

            Console.WriteLine("Import CSV file is begining...");

            var authMethod = new PasswordAuthenticationMethod(AppSettings.SSHUser, AppSettings.SSHPassword);
            var connectionInfo = new ConnectionInfo(AppSettings.SSHHost, AppSettings.SSHPort, AppSettings.SSHUser, authMethod);
            List<DirectoryInfo> subfolders = null;

            do
            {
                var di = new DirectoryInfo(ConfigurationManager.AppSettings["csv-destination-root-path"]);
                subfolders = di.EnumerateDirectories()
                                  .OrderBy(d => d.Name)
                                  .ToList();

                if (subfolders != null && subfolders.Any())
                {
                    subfolders.ForEach(x =>
                    {
                        if (!x.FullName.Contains("DONE"))
                        {
                            DirectoryInfo info = new DirectoryInfo(x.FullName);

                            var subNames = ConfigurationManager.AppSettings["subscriber"].Split(',');

                            if(subNames.Any())
                            {
                                foreach (var subName in subNames)
                                {
                                    var files = info.GetFiles().OrderBy(p => p.CreationTime).Select(z => z.FullName).Where(a => a.Contains(subName)).ToList();

                                    ProcessFile(connectionInfo, files);
                                }
                            }
                            else
                            {
                                var files = info.GetFiles().OrderBy(p => p.CreationTime).Select(z => z.FullName).ToList();

                                ProcessFile(connectionInfo, files);
                            }
                            
                        }
                    });

                    subfolders.Clear();
                }
                else
                {
                    Console.WriteLine("There is no any CSV files.");
                }

                Console.WriteLine($"=======================================================");

            } while (subfolders != null && subfolders.Any());

            Console.WriteLine("Import CSV file is DONE. BYE.");
        }

        private static void ProcessFile(ConnectionInfo connectionInfo, List<string> files)
        {
            files.ForEach(f =>
            {
                var directoryInfo = Directory.GetParent(f);
                var fileName = Path.GetFileName(f);

                try
                {
                    if (!fileName.Contains("Error"))
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
                            File.Move(f, f.Replace(".", "-Error."));

                            LogService.Error($"Import csv file ERROR({f})");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message + $". CVS file({f})");

                    throw;
                }

                Thread.Sleep(40000);
            });
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
