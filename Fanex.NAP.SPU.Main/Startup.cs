namespace Fanex.NAP.SPU.Main
{
    using CrystalQuartz.AspNetCore;
    using Fanex.NAP.SPU.Main.Provider;
    using Fanex.NAP.SPU.Middleware;
    using HealthChecks.UI.Client;
    using HealthChecks.UI.Configuration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
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
            services.AddAppSettings();

            var appSettings = new AppSettings(this.Configuration);
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services
                .AddHealthChecks()
                .AddMySql("Data Source=10.18.200.70;Port=3307;Initial Catalog=CTS_DataCenter;Persist Security Info=True;User ID=ctsServiceAdmin;Password=Welcome1");

            services.AddHealthChecksUI()
            .AddMySqlStorage(appSettings.HealthCheckStorage);

            services.AddSingleton<ISchedulerProvider, SchedulerProvider>();
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

            app.UseHealthChecks("/self", new HealthCheckOptions() { 
                Predicate = _=> true,
                ResponseWriter  = UIResponseWriter.WriteHealthCheckUIResponse
            });

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

            var schedulerProvider = (ISchedulerProvider)app.ApplicationServices.GetService(typeof(ISchedulerProvider));
            var scheduler = schedulerProvider.CreateScheduler();            

            app.UseCrystalQuartz(() => scheduler);

        }
       
    }
}
