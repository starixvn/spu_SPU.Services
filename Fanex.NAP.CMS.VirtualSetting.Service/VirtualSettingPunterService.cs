namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Logging;
    using Fanex.NAP.CMS.VirtualSetting.DataAccess;
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.SPU.Core;

    public class VirtualSettingPunterService : IPunterService
    {
        private bool _soccer;
        private IVirtualSettingPunterDataAccess _dataAccess;
        private VirtualSetting _model;
        private IEnumerable<Punter> _punters { get; set; }
        private Dictionary<int, Punter> _puntersDict;
        private IEnumerable<Punter> _proPuntersUpdated;
        private IEnumerable<Punter> _normalPuntersUpdated;

        public VirtualSettingPunterService(VirtualSetting model, IVirtualSettingPunterDataAccess dataAccess, bool soccer = false)
        {
            _model = model;
            _dataAccess = dataAccess;
            _soccer = soccer;
        }

        public IPunterService GetWinningPunterSoccerList()
        {
            _punters = _dataAccess.GetWinningPunterSoccerList(_model.SiteIds, _model.SuperIds, _model.VirtualRateThreshold);

            return this;
        }

        public IPunterService GetWinningPunterList()
        {
            _punters = _dataAccess.GetWinningPunterList(_model.SiteIds, _model.SuperIds, _model.VirtualRateThreshold);

            return this;
        }

        public IPunterService FilterPunterBaseOnCurrentVirtualRate(short sportType, bool compareVROtherSports)
        {
            if (_punters == null || !_punters.Any())
            {
                return this;
            }

            _puntersDict = _punters.Distinct().ToDictionary(p => p.CustID);
            var custIds = string.Join(",", _puntersDict.Keys);

            var currentVRs = GetCurrentCustomerVirtualRate(custIds);

            if (currentVRs != null && currentVRs.Any())
            {
                foreach (var currentVR in currentVRs)
                {
                    if (currentVR.VRSoccer < _model.VirtualRateThreshold)
                    {
                        _puntersDict[currentVR.CustID].OldVirtualRate = currentVR.VRSoccer;
                        _puntersDict[currentVR.CustID].SportType = sportType;
                    }
                    else if (compareVROtherSports && currentVR.VROtherSports < _model.VirtualRateThreshold)
                    {
                        _puntersDict[currentVR.CustID].OldVirtualRate = currentVR.VROtherSports;
                        _puntersDict[currentVR.CustID].SportType = 2;
                    }
                    else
                    {
                        _puntersDict.Remove(currentVR.CustID);
                    }
                }
            }

            return this;
        }
      
        public IPunterService AdjustVirtualSettingForSuperProPunter(double virtualMultiplier)
        {
            if (_puntersDict == null || !_puntersDict.Any())
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

            this.LogPunters(_proPuntersUpdated, Constants.VSProPunter);

            return this;
        }

        public IPunterService AdjustVirtualSettingForNormal(double virtualMultiplier)
        {
            if (_puntersDict == null || !_puntersDict.Any())
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

            this.LogPunters(_normalPuntersUpdated, Constants.VSNormalPunter);

            return this;
        }

        public int CountAffected()
        {
            if (_proPuntersUpdated != null && _normalPuntersUpdated != null)
            {
                return _proPuntersUpdated.Union(_normalPuntersUpdated, _model.PunterEqualityComparer).Count();
            }
            else if (_proPuntersUpdated != null)
            {
                return _proPuntersUpdated.Count();
            }
            else if (_normalPuntersUpdated != null)
            {
                return _normalPuntersUpdated.Count();
            }

            return 0;
        }


        public IEnumerable<int> GetBasketballVRCustomers()
        {
            return _dataAccess.GetBasketballVRCustomers();
        }

        public IPunterService AdjustBasketballVR(IEnumerable<int> CustIds)
        {
            if (CustIds.Any())
            {
                return this;
            }


            _dataAccess.UpdateBasketballVR(CustIds);

            return this;
        }

        #region Private Methods

        private IEnumerable<CustomerVirtualRate> GetCurrentCustomerVirtualRate(string custIds)
        {
            try
            {
                return _dataAccess.GetCurrentCustomerVirtualRate(custIds);
            }
            catch (Exception ex)
            {
                LogService.Error($"VirtualSetting Service - GetCurrentCustomerVirtualRate Exception { ex.Message }");
            }

            return null;
        }        

        private bool UpdatePunterVirtualSetting(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip)
        {
            switch (_soccer)
            {
                case true:
                    var result = _dataAccess.UpdatePunterVirtualSetting(punters, virtualMultiplier, userId, ip, 1);

                    return result.IsSuccessful;

                default:
                    return UpdatePunterVirtualSettingForAllSports(punters, virtualMultiplier, userId, ip) &&
                            UpdatePunterVirtualSettingForOtherSports(punters, virtualMultiplier, userId, ip);

            }
        }

        public IEnumerable<Punter> UpdateVirtualSetting(IEnumerable<Punter> punters, double newVirtualRate)
        {
            var userId = AppSettings.DummyUser;
            var ip = AppSettings.IP;
            var chunkSize = AppSettings.PunterChunkSize;
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

                        LogService.Error($"VirtualSetting Service - UpdateVirtualSetting error on  { _model.ServiceName }");                        
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

                    LogService.Error($"VirtualSetting Service - UpdateVirtualSetting Exception { ex.Message }");
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
            catch (Exception ex)
            {
                LogService.Error($"VirtualSetting Service - DetectBAAccounts Exception { ex.Message }");
            }
        }

        private bool UpdatePunterVirtualSettingForAllSports(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip)
        {
            var puntersForAllSports = punters.Where(p => p.SportType == 0);

            if (puntersForAllSports != null && puntersForAllSports.Any())
            {
                var result = _dataAccess.UpdatePunterVirtualSetting(puntersForAllSports, virtualMultiplier, userId, ip, 0);

                return result.IsSuccessful;
            }

            return true;
        }

        private bool UpdatePunterVirtualSettingForOtherSports(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip)
        {
            var puntersForOtherSports = punters.Where(p => p.SportType == 2);

            if (puntersForOtherSports != null & puntersForOtherSports.Any())
            {
                var result = _dataAccess.UpdatePunterVirtualSetting(puntersForOtherSports, virtualMultiplier, userId, ip, 2);

                return result.IsSuccessful;
            }

            return true;
        }

        private void LoggingUpdateVirtualSetting(int userId, IEnumerable<Punter> selectedPunters)
        {
            var logResult = _dataAccess.LogPunterVirtualSetting(_model.ServiceName, selectedPunters, userId, _model.AdjustDate);

            if (!logResult.IsSuccessful)
            {
                LogService.Error($"VirtualSetting Service - LoggingUpdateVirtualSetting error { _model.ServiceName }");
            }
        }

        private void UpdateCustomerMarginHistory(IEnumerable<Punter> punters)
        {
            try
            {
                var numOfMonth = AppSettings.NumOfMonth;

                var result = _dataAccess.InsertCustomerMarginHistory(punters, _model.SportGroupID, numOfMonth, _model.ServiceName, _model.AdjustDate);

                if (!result.IsSuccessful)
                {
                    LogService.Error($"VirtualSetting Service - UpdateCustomerMarginHistory error { _model.ServiceName }");
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"VirtualSetting Service - UpdateCustomerMarginHistory Exception { ex.Message }");
            }
        }        

        private void LogPunters(IEnumerable<Punter> punters, string punterType)
        {
            var sportType = _soccer ? Constants.VSSoccer : Constants.VSSportbook;

            if (punters != null && punters.Any())
            {
                var info = $"List of { sportType } {punterType} updated({punters.Count()}): { string.Join(",", punters.Select(p => p.CustID))}";

                LogService.Info(info);
            }

            if (_model != null && _model.FailedUpdatePunterList.Any())
            {
                var error = $"List of { sportType } {punterType} update fail({_model.FailedUpdatePunterList.Count}) : {string.Join(",", _model.FailedUpdatePunterList.Select(p=>p.CustID))}";

                LogService.Error(error);
            }
        }

        #endregion
    }
}