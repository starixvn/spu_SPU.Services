namespace Fanex.NAP.SPU.Core
{
    using System;
    using System.IO;

    public static class Utilities
    {
        public static string ReadTextFile(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }

        public static string GenerateVirtualSettingXmlForUpdating(string param1, double param2)
        {
            var rate = string.Format(Constants.VSXmlForUpdating, param1, param2);

            return string.Format(Constants.VSXmlRoot, rate);
        }

        public static string GenerateVirtualSettingXmlFromRate(string param)
        {
            return string.Format(Constants.VSXmlRoot, param);
        }

        public static string GenerateRateVirtualSettingXmlForLogging(params object[] objParams)
        {
            return string.Format(Constants.VSXmlForLogging, objParams);
        }

        public static string GenerateRateVirtualSettingXmlForCustomerMarginHistory(params object[] objParams)
        {
            return string.Format(Constants.VSXmlForCustomerMarginHistory, objParams);
        }

        public static void WriteLog(string filename, string content)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            File.AppendAllText(filePath, string.Format("{0}{1}", content, System.Environment.NewLine));
        }

        public static void DeleteLog(string fileName)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static void WriteLogAllLine(string filename, string[] content)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            File.WriteAllLines(filePath, content);

        }
    }
}