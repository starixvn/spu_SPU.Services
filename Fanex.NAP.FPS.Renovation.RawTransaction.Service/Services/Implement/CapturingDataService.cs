namespace Fanex.NAP.FPS.Renovation.RawTransaction.Service
{
    using Fanex.DeviceDetector.Factory;
    using Fanex.NAP.FPS.Renovation.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class CapturingDataService : ICapturingDataService
    {
        private readonly DataAccess.RawTransactionDataAccess capturingData;
              
        public CapturingDataService()
        {
            capturingData = new DataAccess.RawTransactionDataAccess();           
        }

        public int InsertTransaction(List<RawTransactionModel> rawTrans)
        {
            var model = JsonConvert.SerializeObject(rawTrans);

           return capturingData.InsertTransaction(model);
        }

        public DeviceInfoModel GetDeviceInfo(string deviceCode)
        {
            if (string.IsNullOrWhiteSpace(deviceCode))
            {
                return TransformEmptyDeviceInfo();
            }

            var deviceInfo = DeviceFactory.GetDeviceInfo(deviceCode);

            if (deviceInfo != null)
            {
                return TransformDeviceInfo(deviceInfo);
            }

            return TransformEmptyDeviceInfo();
        }              

        private DeviceInfoModel TransformEmptyDeviceInfo()
        {
            return new DeviceInfoModel
            {
                AppVersion = string.Empty,
                CapturedDate = 0,
                DeviceFPCode = string.Empty,
                DeviceNo = string.Empty,
                HRef = string.Empty,
                NA = string.Empty,
                Screen = new ScreenInfoModel
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

        private DeviceInfoModel TransformDeviceInfo(DeviceDetector.Models.DeviceInfo deviceInfo)
        {
            return new DeviceInfoModel
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
                    ? new ScreenInfoModel
                    {
                        ColorDepth = 0,
                        Height = 0,
                        Width = 0
                    }
                    : new ScreenInfoModel
                    {
                        ColorDepth = deviceInfo.Screen.ColorDepth,
                        Height = deviceInfo.Screen.Height,
                        Width = deviceInfo.Screen.Width
                    }
            };
        }
    }
}
