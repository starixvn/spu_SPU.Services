namespace Fanex.NAP.SPU.Main
{
    using CrystalQuartz.AspNetCore;
    using HealthChecks.UI.Configuration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;   
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Quartz;
    using Quartz.Impl;

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
            services.AddControllersWithViews();

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.AddHealthChecksUI(setup =>
            {
                setup.SetEvaluationTimeInSeconds(5);
                setup.DisableDatabaseMigrations();
            })
            .AddMySqlStorage("Data Source=10.18.200.70;Port=3307;Initial Catalog=CTS_Monitoring;Persist Security Info=True;User ID=ctsLogOwner;Password=password");
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

            app.UseHealthChecksUI(delegate (Options option) {
                option.UIPath = "/hc";
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
            });

            var scheduler = CreateScheduler();
            app.UseCrystalQuartz(() => scheduler);

        }

        private static IScheduler CreateScheduler()
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().Result;

            var job = JobBuilder.Create<ExecuteJob>()
                .WithIdentity("localJob", "default")
                .Build();

            var trigger = TriggerBuilder.Create()
               .WithIdentity("trigger1", "default")
               .ForJob(job)
               .StartNow()
               .WithCronSchedule("0 /1 * ? * *")
               .Build();

            scheduler.ScheduleJob(job, trigger);

            scheduler.Start();

            return scheduler;
        }

    }
}
