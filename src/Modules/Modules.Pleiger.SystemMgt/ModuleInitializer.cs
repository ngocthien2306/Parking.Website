using InfrastructureCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Modules.Admin.Services.IService;
using Modules.Admin.Services.ServiceImp;
using Modules.Pleiger.SystemMgt.Services.IService;
using Modules.Pleiger.SystemMgt.Services.ServiceImp;
namespace Modules.Pleiger.SystemMgt
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IChartService, ChartService>();
            services.AddTransient<ISiteService, SiteService>();
            services.AddTransient<IMESComCodeService, MESComCodeService>();
            services.AddTransient<ICommonService, CommonService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
