namespace Fanex.NAP.DCS.FP.RawTransaction.Services
{
    using Fanex.DeviceDetector.Factory;
    using Fanex.NAP.DCS.DataAccess.FPRawTransaction;
    using Fanex.NAP.DCS.Model.FPRawTransaction;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public interface IFPCapturingService
    {
        int InsertTransaction(List<FPRawTransactionModel> rawTrans);

        FPDeviceInfoModel GetDeviceInfo(string deviceCode);
    }

    public class FPCapturingService : IFPCapturingService
    {
        private readonly FPRawTransactionDataAccess capturingData;

        public FPCapturingService()
        {
            capturingData = new FPRawTransactionDataAccess();
        }

        public int InsertTransaction(List<FPRawTransactionModel> rawTrans)
        {
            var model = JsonConvert.SerializeObject(rawTrans);

            return capturingData.InsertTransaction(model);
        }

        public FPDeviceInfoModel GetDeviceInfo(string deviceCode)
        {
            if (string.IsNullOrWhiteSpace(deviceCode))
            {
                return TransformEmptyDeviceInfo();
            }

            var deviceInfo = DeviceFactory.GetDeviceInfo(deviceCode);

            if (deviceInfo != null && !string.IsNullOrEmpty(deviceInfo.DeviceCode))
            {
                return TransformDeviceInfo(deviceInfo);
            }

            return TransformEmptyDeviceInfo();
        }

        private FPDeviceInfoModel TransformEmptyDeviceInfo()
        {
            return new FPDeviceInfoModel
            {
                AppVersion = string.Empty,
                CapturedDate = 0,
                DeviceFPCode = string.Empty,
                DeviceNo = string.Empty,
                HRef = string.Empty,
                NA = string.Empty,
                Screen = new FPScreenInfoModel
                {
                    ColorDepth = 0,
                    Height = 0,
                    Width = 0
                },
                TimeZone = string.Empty,
                UserAgent = string.Empty,
                InvalidDeviceId = true
            };
        }

        private FPDeviceInfoModel TransformDeviceInfo(DeviceDetector.Models.DeviceInfo deviceInfo)
        {
            return new FPDeviceInfoModel
            {
                AppVersion = deviceInfo.AppVersion,
                CapturedDate = deviceInfo.CapturedDate,
                DeviceFPCode = deviceInfo.DeviceCode,
                DeviceNo = deviceInfo.DeviceId,
                HRef = deviceInfo.HRef,
                NA = deviceInfo.NA,
                TimeZone = deviceInfo.TimeZone,
                UserAgent = deviceInfo.UserAgent,
                Screen = deviceInfo.Screen == null
                    ? new FPScreenInfoModel
                    {
                        ColorDepth = 0,
                        Height = 0,
                        Width = 0
                    }
                    : new FPScreenInfoModel
                    {
                        ColorDepth = deviceInfo.Screen.ColorDepth,
                        Height = deviceInfo.Screen.Height,
                        Width = deviceInfo.Screen.Width
                    }
            };
        }

    }
}
