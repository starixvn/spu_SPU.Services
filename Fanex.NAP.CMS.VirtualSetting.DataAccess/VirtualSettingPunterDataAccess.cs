using Fanex.Data.Repository;
using Fanex.NAP.CMS.VirtualSetting.Criteria;
using Fanex.NAP.CMS.VirtualSetting.Model;
using Fanex.NAP.Framework.Command;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fanex.NAP.CMS.VirtualSetting.DataAccess
{
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

        public IEnumerable<CustomerMargin> GetCustomerMarginHistory(string custIds, byte sportGroupId, int numOfMonth)
        {
            var criteria = new GetCustomerMarginHistoryCriteria
            {
                CustIds = custIds,
                SportGroupID = sportGroupId,
                NumOfMonth = numOfMonth
            };

            return _repository.Fetch<CustomerMargin>(criteria);
        }       

        public ICommandResult InsertCustomerMarginHistory(string customerMarginDoc, DateTime adjustDate)
        {
            var criteria = new InsertCustomerMarginHistoryCriteria
            {
                CustomerMarginDoc = customerMarginDoc,
                AdjustDate = adjustDate,
                ErrCode = 0,
                ErrMsg = string.Empty
            };

            _repository.Execute(criteria);

            return new CommandResult
            {
                Code = criteria.ErrCode,
                Message = criteria.ErrMsg
            };
        }

        public ICommandResult LogVirtualSetting(string punterVRSettingsDoc, int creatorId, string serviceName, DateTime adjustDate)
        {
            var criteria = new LogVirtualSettingCriteria
            {
                PunterVRSettingsDoc = punterVRSettingsDoc,
                CreatorId = creatorId,
                ServiceName = serviceName,
                AdjustDate = adjustDate
            };

            var result = _repository.Execute(criteria);

            return new CommandResult
            {
                Code = result,
                Message = string.Empty
            };
        }

        public ICommandResult UpdateVirtualSetting(string VirtualMultiplier, int userId, string ip, short sportType)
        {
            var criteria = new UpdateVirtualSettingCriteria
            {
                VirtualMultiplier = VirtualMultiplier,
                UserId = userId,
                IP = ip,
                SportType = sportType
            };

            var result = _repository.Execute(criteria);

            return new CommandResult
            {
                Code = result,
                Message = string.Empty
            };
        }
    }
}
