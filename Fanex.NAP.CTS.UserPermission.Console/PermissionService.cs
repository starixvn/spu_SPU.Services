using Fanex.NAP.Core.Models.ValueObject;
using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.CTS.Model;
using Fanex.NAP.CTS.Services;
using Fanex.NAP.SPU.Core;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Fanex.NAP.CTS.UserPermission.Console
{
    public class PermissionService
    {
        private readonly UserService userService;
        private readonly DataCenterService dataService;

        public PermissionService()
        {
            userService = new UserService();
            dataService = new DataCenterService();
        }

        public PermissionService SyncUserPermission(string functionName, int functionKey)
        {
            var userPolicy = userService.GetUserPolicies(functionName, Int32.Parse(AppSettings.SiteId));

            if(userPolicy != null && userPolicy.Any())
            {
                var userPermission = userPolicy.Select(x => TransformUserPermission(x, functionKey));

                if(userPermission != null && userPermission.Any())
                {
                    dataService.InsertUserPermission(JsonConvert.SerializeObject(userPermission));
                }
            }

            return this;
        }

        private UserPermissionModel TransformUserPermission(UserPolicy userPolicy, int functionKey)
        {
            return new UserPermissionModel { 
            
                UserID = userPolicy.User.Id,
                FuncID = functionKey
            };
        }
    }
}
