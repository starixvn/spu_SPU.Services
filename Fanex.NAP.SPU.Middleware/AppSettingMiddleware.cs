
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fanex.NAP.SPU.Middleware
{

    public static class AppSettingMiddleware
    {
        public static void AddAppSettings(this IServiceCollection services)
        {            
            services.AddSingleton<IAppSettings, AppSettings>();
        }
    }
}
