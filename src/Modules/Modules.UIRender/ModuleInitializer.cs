using InfrastructureCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Modules.UIRender.Services.IService;
using Modules.UIRender.Services.ServiceImp;

namespace Modules.RenderPage
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddTransient<IActivityTypeRepository, ActivityRepository>();
            //services.AddTransient<INotificationHandler<EntityViewed>, EntityViewedHandler>();
            services.AddTransient<IDynamicPageService, DynamicPageService>();
            services.AddTransient<IValidateDataService, ValidateDataService>();
            //services.AddTransient<IDynamicPageService, DynamicPageService>();
            //GlobalConfiguration.RegisterAngularModule("simplAdmin.activityLog");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "javascript",
            //        template: "JavaScript/{action}.js",
            //        defaults: new { controller = "JavaScript" });
            //});
        }
    }
}
