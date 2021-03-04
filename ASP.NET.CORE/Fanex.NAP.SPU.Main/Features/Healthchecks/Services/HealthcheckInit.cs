namespace Fanex.NAP.SPU.Main.Features.Healthchecks.Services
{
    using Fanex.NAP.SPU.Main.Features.Healthchecks.Models;
    using Fanex.NAP.SPU.Main.JobSchedulers.Services;
    using Fanex.NAP.SPU.Middleware;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;

    public static class HealthcheckInit
    {
        public static void AddSingletonHealthCheck(this IServiceCollection services, AppSettings appSettings, JobSchedules jobSchedules)
        {
            services
               .AddHealthChecksUI(setup =>
               {
                   setup.AddHealthCheckEndpoint("Main Service", "/healthz");

                   setup.AddHealthCheckEndpoint("Gateway Service", $"{appSettings.GatewayURL}/healthz");

                   foreach (var job in jobSchedules.JobScheduleList)
                   {
                       if (!string.IsNullOrWhiteSpace(job.HealthCheckUrl))
                       {
                           setup.AddHealthCheckEndpoint(job.JobName, job.HealthCheckUrl);
                       }
                   }

                   setup.AddWebhookNotification("notification",
                       "WebhookNotification/send",
                        payload: JsonConvert.SerializeObject(new NotificationModel("[[LIVENESS]]: [[FAILURE]] [[DESCRIPTIONS]]", 1)),
                        restorePayload: JsonConvert.SerializeObject(new NotificationModel("[[LIVENESS]] is back to LIFE", 0)));
               })
               .AddInMemoryStorage();
               //.AddMySqlStorage(appSettings.HealthCheckStorage);
        }
    }
}
