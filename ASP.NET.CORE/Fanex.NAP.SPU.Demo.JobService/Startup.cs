using Fanex.Data;
using Fanex.Data.MySql;
using Fanex.NAP.SPU.Demo.JobService.Controllers;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fanex.NAP.SPU.Demo.JobService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services
               .AddHealthChecks()
               .AddCheck<RandomHealthCheck>("random")
               .AddDiskStorageHealthCheck(setup => { DriveInfo.GetDrives(); });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fanex.NAP.SPU.Demo.JobService", Version = "v1" });
            });

            services.AddScoped<IJobService, Controllers.JobService>();

            FanexDataConfig();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();               
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fanex.NAP.SPU.Demo.JobService v1"));

            app.UseRouting();

            app.UseHealthChecks("/healthz", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //DbSettingProviderManager
            //    .StartNewSession()
            //    .WithMySql(resourcePath: "~/App_Data")                
            //    .Run();          

        }

        private void FanexDataConfig()
        {
            var connections = Configuration
                                  .GetSection("Connections")
                                  .GetChildren()
                                  .AsEnumerable()
                                  .ToDictionary(
                                           x => x.GetValue<string>("Name"),
                                           y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName"))
                                   );

            DbSettingProviderManager
              .StartNewSession()
              .Use(connections)
              .WithMySql()
              .UseDefaultDbSettingProvider(resourcePath: "~/App_Data")
              .Run();
        }
    }
}
