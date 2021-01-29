using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;

namespace Fanex.NAP.SPU.Middleware
{
    public interface IAppSettings
    {
        string HealthCheckStorage { get; }

        string GatewayURL { get; }
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;
                                    
            HealthCheckStorage = GetValue<string>(nameof(HealthCheckStorage));

            GatewayURL = GetValue<string>(nameof(GatewayURL));
        }

        public string HealthCheckStorage { get; }

        public string GatewayURL { get; set; }

        public T GetValue<T>(string key)
        {
            try
            {
                var value = configuration[key];

                var typeConverter = TypeDescriptor.GetConverter(typeof(T));

                return (T)typeConverter.ConvertFromString(value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Key: {key}", ex);
            }
        }
    }
}
