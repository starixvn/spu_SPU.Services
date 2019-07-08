namespace Fanex.NAP.CMS.VirtualSetting.DataAccess
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.CMS.VirtualSetting.Criteria;
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.Framework.Command;
    using Fanex.NAP.SPU.Core;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class VirtualSettingPunterDataAccess : IVirtualSettingPunterDataAccess
    {
        private readonly IDynamicRepository _repository;

        public VirtualSettingPunterDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public IEnumerable<int> GetBACustIds(string custIds)
        {
            var criteria = new GetBAAccountsCriteria
            {
                CustIds = custIds
            };

            return _repository.Fetch<int>(criteria);
        }

        public IEnumerable<CustomerVirtualRate> GetCurrentCustomerVirtualRate(string custIds)
        {
            var criteria = new GetCurrentCustomerVirtualRate
            {
                CustIds = custIds
            };

            return _repository.Fetch<CustomerVirtualRate>(criteria);
        }       

        public IEnumerable<Punter> GetWinningPunterSoccerList(string siteNames, string superIds, double virtualRateThreshold)
        {
            var criteria = new GetProPrunterSoccerCriteria
            {
                SiteIds = siteNames,
                SuperIds = superIds,
                VirtualRateThreshold = virtualRateThreshold
            };

            return _repository.Fetch<Punter>(criteria);
        }

        public IEnumerable<Punter> GetWinningPunterList(string siteNames, string superIds, double virtualRateThreshold)
        {
            var criteria = new GetProPrunterCriteria
            {
                SiteIds = siteNames,
                SuperIds = superIds,
                VirtualRateThreshold = virtualRateThreshold
            };

            return _repository.Fetch<Punter>(criteria);
        }

        public ICommandResult UpdatePunterVirtualSetting(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip, short sportType)
        {
            var cusIds = string.Join(",", punters.Select(x => x.CustID));

            var criteria = new UpdateVirtualSettingCriteria
            {
                VirtualMultiplier = Utilities.GenerateVirtualSettingXmlForUpdating(cusIds, virtualMultiplier),
                UserId = userId,
                IP = ip,
                SportType = sportType
            };

            return ExecuteOutput(criteria);
        }        

        public ICommandResult LogPunterVirtualSetting(string serviceName, IEnumerable<Punter> punters, int userId, DateTime adjustDate)
        {
            var rate = string.Empty;

            foreach (var punter in punters)
            {
                object[] objParams =
                {
                    punter.CustID,
                    punter.ReportDate.ToShortDateString(),
                    punter.SiteID,
                    punter.SRecommend,
                    punter.MRecommend,
                    punter.Recommend,
                    punter.OldVirtualRate,
                    punter.NewVirtualRate,
                    punter.IsBA
                };

                rate += Utilities.GenerateRateVirtualSettingXmlForLogging(objParams);
            }

            var criteria = new LogVirtualSettingCriteria
            {
                PunterVRSettingsDoc = Utilities.GenerateVirtualSettingXmlFromRate(rate),
                CreatorId = userId,
                ServiceName = serviceName,
                AdjustDate = adjustDate
            };

            return ExecuteOutput(criteria);
        }

        public ICommandResult InsertCustomerMarginHistory(IEnumerable<Punter> punters, byte sportGroupID, int numOfMonth, string serviceName, DateTime adjustDate)
        {
            var rate = string.Empty;
            var customerMargins = GetCustomerMarginHistory(punters, sportGroupID, numOfMonth);            

            foreach (var customerMargin in customerMargins)
            {
                object[] objParams =
                {
                    customerMargin.CustId,
                    customerMargin.ReportMonth,
                    customerMargin.ReportYear,
                    customerMargin.Turnover,
                    customerMargin.Winloss,
                    customerMargin.BetCount,
                    serviceName
                };

                rate += Utilities.GenerateRateVirtualSettingXmlForCustomerMarginHistory(objParams);
            }

            var doc = Utilities.GenerateVirtualSettingXmlFromRate(rate);

            return this.InsertCustomerMarginHistory(doc, adjustDate);
        }

        private ICommandResult InsertCustomerMarginHistory(string customerMarginDoc, DateTime adjustDate)
        {
            var criteria = new InsertCustomerMarginHistoryCriteria
            {
                CustomerMarginDoc = customerMarginDoc,
                AdjustDate = adjustDate,
                ErrCode = 0,
                ErrMsg = string.Empty
            };

            return ExecuteOutput(criteria);
        }

        public IEnumerable<CustomerMargin> GetCustomerMarginHistory(IEnumerable<Punter> punters, byte sportGroupID, int numOfMonth)
        {
            var criteria = new GetCustomerMarginHistoryCriteria
            {
                CustIds = string.Join(",", punters.Select(c => c.CustID).Distinct()),
                SportGroupID = sportGroupID,
                NumOfMonth = numOfMonth
            };

            return _repository.Fetch<CustomerMargin>(criteria);
        }        

        private ICommandResult ExecuteOutput(ICriteria criteria)
        {
            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var param = criteria.GetSpParams();
                objectDb.ExecuteNonQuery(param);

                return GetCommandResult(param);
            }
        }

        private ICommandResult GetCommandResult(Hashtable hash)
        {
            var errCode = hash["errcode"];
            var errMsg = hash["errmsg"];

            return new CommandResult
            {
                Code = (errCode is DBNull) ? 0 : Convert.ToInt32(errCode),
                Message = (errMsg is DBNull) ? string.Empty : Convert.ToString(errMsg)
            };
        }
    }
}