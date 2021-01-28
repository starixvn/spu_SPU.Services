namespace Fanex.NAP.SPU.Main
{
    using AutoMapper;
    using CrystalQuartz.AspNetCore;
    using Fanex.NAP.SPU.Main.Features.JobSchedulers;
    using Fanex.NAP.SPU.Main.JobSchedulers;
    using Fanex.NAP.SPU.Middleware;
    using HealthChecks.UI.Client;
    using HealthChecks.UI.Configuration;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Prometheus;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;
    using Refit;
    using System.Collections.Specialized;
    using System.IO;
    using System.Reflection;

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
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(typeof(Startup));

            services.AddControllersWithViews();
            services.AddAppSettings();            
            var appSettings = new AppSettings(this.Configuration);
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services
                .AddHealthChecks()
                .AddMySql("Data Source=10.18.200.70;Port=3307;Initial Catalog=CTS_DataCenter;Persist Security Info=True;User ID=ctsServiceAdmin;Password=Welcome11")
                .AddCheck<RandomHealthCheck>("random")
                .AddDiskStorageHealthCheck(setup => { DriveInfo.GetDrives(); })
                .ForwardToPrometheus();

           

            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton<ISchedulerFactory>(p => new StdSchedulerFactory(new NameValueCollection {
                { "quartz.threadPool.type", "Quartz.Simpl.SimpleThreadPool, Quartz" },
                { "quartz.threadPool.threadCount", "100" },
                { "quartz.threadPool.threadPriority", "Normal" },                                       
                { "quartz.scheduler.exporter.bindName", "QuartzScheduler" },
                { "quartz.scheduler.instanceName", "SPUService" },
                { "quartz.scheduler.instanceId", "AUTO" },
            }));
                        
            services.AddRefitClient<IJobServiceApi>();
            services.AddSingleton<IJobService, JobService>();

            services.AddSingleton<QuartzJobRunner>();
            services.AddSingleton<QuartzHostedService>();
            services.AddHostedService<QuartzHostedService>();
            services.AddSingleton<JobExecute>();
            services.AddSingletonJob(appSettings);            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHealthChecks("/healthz", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecksUI(delegate (Options option)
            {
                option.UIPath = "/healthcheck";
                option.AddCustomStylesheet(@"wwwroot/css/healthcheck.css");
            });

            

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapMetrics();
            });

            var schedulerProvider = (ISchedulerFactory)app.ApplicationServices.GetService(typeof(ISchedulerFactory));

            app.UseCrystalQuartz(() => schedulerProvider.GetScheduler().Result);

            app.UseMiddleware<ExceptionHandlerMiddleware>();                      

        }

    }
}
