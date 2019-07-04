namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NAP.CMS.VirtualSetting.DataAccess;
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.SPU.Core;

    public class VirtualSettingPunterService : IPunterService
    {
        private IVirtualSettingPunterDataAccess _dataAccess;
        private VirtualSetting _model;
        private IEnumerable<Punter> _punters;
        private Dictionary<int, Punter> _puntersDict;
        private IEnumerable<Punter> _proPuntersUpdated;
        private IEnumerable<Punter> _normalPuntersUpdated;

        public VirtualSettingPunterService(VirtualSetting model, IVirtualSettingPunterDataAccess dataAccess)
        {
            _model = model;
            _dataAccess = dataAccess;
        }

        public IPunterService GetPunterList()
        {
            _punters = _dataAccess.GetWinningPunterSoccerList(_model.SiteIds, _model.SuperIds, _model.VirtualRateThreshold);

            return this;
        }

        public IPunterService FilterPunterBaseOnCurrentVirtualRate()
        {
            if (!_punters.Any())
            {
                return this;
            }

            var puntersDict = _punters.Distinct().ToDictionary(p => p.CustID);
            var custIds = string.Join(",", puntersDict.Keys);

            var currentVRs = GetCurrentCustomerVirtualRate(custIds);
            if (currentVRs != null && currentVRs.Any())
            {
                foreach (var currentVR in currentVRs)
                {
                    if (currentVR.VRSoccer < _model.VirtualRateThreshold)
                    {
                        puntersDict[currentVR.CustID].OldVirtualRate = currentVR.VRSoccer;
                        puntersDict[currentVR.CustID].SportType = 1;
                    }
                    else
                    {
                        puntersDict.Remove(currentVR.CustID);
                    }
                }
            }

            return this;
        }
      
        public IPunterService AdjustVirtualSettingForSuperProPunter(double virtualMultiplier)
        {
            if (!_puntersDict.Any())
            {
                return this;
            }

            var proPunters = _puntersDict.Values.Where(p => p.IsSuperProPunters);

            if (!proPunters.Any())
            {
                return this;
            }

            _model.FailedUpdatePunterList.Clear();

            _proPuntersUpdated = UpdateVirtualSetting(proPunters, virtualMultiplier);

            _proPuntersUpdated = _proPuntersUpdated.Except(_model.FailedUpdatePunterList, _model.PunterEqualityComparer);

            return this;
        }

        public IPunterService AdjustVirtualSettingForNormal(double virtualMultiplier)
        {
            if (!_puntersDict.Any())
            {
                return this;
            }

            var normalPunters = _puntersDict.Values.Where(p => !p.IsSuperProPunters);

            if (!normalPunters.Any())
            {
                return this;
            }

            _model.FailedUpdatePunterList.Clear();

            _normalPuntersUpdated = UpdateVirtualSetting(normalPunters, virtualMultiplier);

            _normalPuntersUpdated = _normalPuntersUpdated.Except(_model.FailedUpdatePunterList, _model.PunterEqualityComparer);

            return this;
        }

        #region Private Methods

        private IEnumerable<CustomerVirtualRate> GetCurrentCustomerVirtualRate(string custIds)
        {
            try
            {
                return _dataAccess.GetCurrentCustomerVirtualRate(custIds);
            }
            catch (Exception exception)
            {
                //Logger.Log.Error("Error (exception) for GetCurrentCustomerVirtualRate on " + ServiceName + " ; " + exception.Message);
            }

            return null;
        }        

        private bool UpdatePunterVirtualSetting(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip)
        {
            var result = _dataAccess.UpdatePunterVirtualSettingForSoccer(punters, virtualMultiplier, userId, ip);

            return result.IsSuccessful;
        }

        public IEnumerable<Punter> UpdateVirtualSetting(IEnumerable<Punter> punters, double newVirtualRate)
        {
            var userId = AppSetting.DummyUser;
            var ip = AppSetting.IP;
            var chunkSize = AppSetting.PunterChunkSize;
            var chunkPunters = punters.Partition(chunkSize);

            foreach (var selectedPunters in chunkPunters)
            {
                try
                {
                    DetectBAAccounts(newVirtualRate, selectedPunters);

                    var updateResult = UpdatePunterVirtualSetting(selectedPunters, newVirtualRate, userId, ip);

                    if (!updateResult)
                    {
                        _model.FailedUpdatePunterList.AddRange(selectedPunters);
                        //Logger.Log.Error("Error for updating virtual settings on " + _model.ServiceName);
                    }
                    else
                    {
                        LoggingUpdateVirtualSetting(userId, selectedPunters);
                        UpdateCustomerMarginHistory(selectedPunters);
                    }
                }
                catch (Exception ex)
                {
                    _model.FailedUpdatePunterList.AddRange(selectedPunters);
                    //Logger.Log.Error("Error (exception) for UpdateVirtualSetting on " + ServiceName + " ; " + exception.InnerException.Message);
                }
            }

            return punters;
        }       

        private void DetectBAAccounts(double newVirtualRate, IEnumerable<Punter> punters)
        {
            try
            {
                var custIds = string.Join(",", punters.Select(p => p.CustID));

                var listBAAccounts = _dataAccess.GetBACustIds(custIds);

                foreach (var p in punters)
                {
                    if (newVirtualRate == 3 && listBAAccounts.Contains(p.CustID))
                    {
                        p.NewVirtualRate = 1.0;
                        p.IsBA = true;
                    }
                    else
                    {
                        p.NewVirtualRate = newVirtualRate;
                    }
                }
            }
            catch (Exception exception)
            {
                //Logger.Log.Error("Error (exception) for DetectBAAccounts on " + _model.ServiceName + " ; " + exception.InnerException.Message);
            }
        }

        private void LoggingUpdateVirtualSetting(int userId, IEnumerable<Punter> selectedPunters)
        {
            var logResult = _dataAccess.LogPunterVirtualSetting(_model.ServiceName, selectedPunters, userId, _model.AdjustDate);

            if (!logResult.IsSuccessful)
            {
                //Logger.Log.Error("Error for logging virtual settings on " + ServiceName);
            }
        }

        private void UpdateCustomerMarginHistory(IEnumerable<Punter> punters)
        {
            try
            {
                var numOfMonth = AppSetting.NumOfMonth;

                var result = _dataAccess.InsertCustomerMarginHistory(punters, _model.SportGroupID, numOfMonth, _model.ServiceName, _model.AdjustDate);

                if (!result.IsSuccessful)
                {
                    //Logger.Log.Error("Error for logging customer margin history on " + ServiceName);
                }
            }
            catch (Exception ex)
            {
                //Logger.Log.Error("Error (exception) for UpdateCustomerMarginHistory on " + ServiceName + " ; " + exception.Message);
            }
        }

        #endregion
    }
}