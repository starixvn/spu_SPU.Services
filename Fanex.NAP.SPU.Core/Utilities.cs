namespace Fanex.NAP.SPU.Core
{
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
            var rate = string.Format(Constants.VirtualSettingXmlForUpdating, param1, param2);

            return string.Format(Constants.VirtualSettingXmlRoot, rate);
        }

        public static string GenerateVirtualSettingXmlFromRate(string param)
        {
            return string.Format(Constants.VirtualSettingXmlRoot, param);
        }

        public static string GenerateRateVirtualSettingXmlForLogging(params object[] objParams)
        {
            return string.Format(Constants.VirtualSettingXmlForLogging, objParams);
        }

        public static string GenerateRateVirtualSettingXmlForCustomerMarginHistory(params object[] objParams)
        {
            return string.Format(Constants.VirtualSettingXmlForCustomerMarginHistory, objParams);
        }
    }
}