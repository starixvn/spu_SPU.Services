namespace Fanex.NAP.SPU.ExecutingService
{
    using Fanex.NAP.SPU.Service;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ExecuteJobBase
    {
        protected IUMControlAdapter maintenanceService;

        public ExecuteJobBase()
        {
            maintenanceService = new UMControlAdapter();
        }      

        public bool CheckUM()
        {
            return maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime;
        }

        public static async Task<IRestResponse> Executing<T>(List<T> listObject, string apiUrl, string actionPath)
        {
            var client = new RestClient(new Uri(apiUrl));
            var request = new RestRequest(actionPath, Method.POST);

            request.JsonSerializer = NewtonsoftJsonSerializer.Default;
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(listObject);

            return await client.ExecuteTaskAsync(request);
        }
    }
}
