using InfrastructureCore.Web;
using InfrastructureCore.Web.LocalizationResources;
using LazZiya.ExpressLocalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Modules.Kiosk.Monitoring.Hubs;
using Modules.Kiosk.Monitoring.MiddlewareExtensions;
using Modules.Kiosk.Monitoring.TableDependencies;
using Modules.Parking.Hubs;
using Modules.Parking.MiddlewareExtensions;
using Modules.Parking.TableDependencies;
using System.Globalization;
using System.IO;

namespace Modular.WebHost
{
    public class Startup : CoreStartup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment) : base(configuration, hostingEnvironment)
        {
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            CoreStartup.CoreConfigure(app, env);
            //// Inject more DbContext;
            //app.UseRequestLocalization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{culture=en}/{controller=MESHome}/{action=Index}/{id?}");
                endpoints.MapHub<CheckInHub>("/checkinHub");
                endpoints.MapHub<TrackHub>("/trackHub");
                
            });

            app.UseCheckInTableDependency();
            app.UseTrackTableDependency();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "StaticFiles")),
                RequestPath = "/StaticFiles"
            });
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            var cultures = new[]
           {
                new CultureInfo("en"),
                new CultureInfo("vi"),
            };
            services.AddSignalR();
            services.AddScoped<IRazorViewRenderer, RazorViewRenderer>();
            services.AddSingleton<CheckInHub>();
            services.AddSingleton<TrackHub>();
            services.AddSingleton<SubcribeCheckInTableDependency>();
            services.AddSingleton<TrackDependency>();
            services.AddControllersWithViews()
                .AddExpressLocalization<ExpressLocalizationResource, ViewLocalizationResource>(ops =>
                {
                    // When using all the culture providers, the localization process will
                    // check all available culture providers in order to detect the request culture.
                    // If the request culture is found it will stop checking and do localization accordingly.
                    // If the request culture is not found it will check the next provider by order.
                    // If no culture is detected the default culture will be used.

                    // Checking order for request culture:
                    // 1) RouteSegmentCultureProvider
                    //      e.g. http://localhost:1234/tr
                    // 2) QueryStringCultureProvider
                    //      e.g. http://localhost:1234/?culture=tr
                    // 3) CookieCultureProvider
                    //      Determines the culture information for a request via the value of a cookie.
                    // 4) AcceptedLanguageHeaderRequestCultureProvider
                    //      Determines the culture information for a request via the value of the Accept-Language header.
                    //      See the browsers language settings

                    // Uncomment and set to true to use only route culture provider
                    ops.UseAllCultureProviders = false;
                    ops.ResourcesPath = "LocalizationResources";
                    ops.RequestLocalizationOptions = o =>
                    {
                        o.SupportedCultures = cultures;
                        o.SupportedUICultures = cultures;
                        o.DefaultRequestCulture = new RequestCulture("en");
                    };
                });
            //.AddExpressLocalization<PleigerExpressLocalizationResource, PleigerViewLocalizationResource>(ops =>
            //{
            //    // When using all the culture providers, the localization process will
            //    // check all available culture providers in order to detect the request culture.
            //    // If the request culture is found it will stop checking and do localization accordingly.
            //    // If the request culture is not found it will check the next provider by order.
            //    // If no culture is detected the default culture will be used.

            //    // Checking order for request culture:
            //    // 1) RouteSegmentCultureProvider
            //    //      e.g. http://localhost:1234/tr
            //    // 2) QueryStringCultureProvider
            //    //      e.g. http://localhost:1234/?culture=tr
            //    // 3) CookieCultureProvider
            //    //      Determines the culture information for a request via the value of a cookie.
            //    // 4) AcceptedLanguageHeaderRequestCultureProvider
            //    //      Determines the culture information for a request via the value of the Accept-Language header.
            //    //      See the browsers language settings

            //    // Uncomment and set to true to use only route culture provider
            //    ops.UseAllCultureProviders = false;
            //    ops.ResourcesPath = "PleigerLocalizationResources";
            //    ops.RequestLocalizationOptions = o =>
            //    {
            //        o.SupportedCultures = cultures;
            //        o.SupportedUICultures = cultures;
            //        o.DefaultRequestCulture = new RequestCulture("en");
            //    };
            //});
            base.ConfigureServices(services);

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/MESAccount/MESLogin";
                options.AccessDeniedPath = "/MESHome";
            });
        }
    }
}
