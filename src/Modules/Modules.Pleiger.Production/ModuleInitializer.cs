using InfrastructureCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Modules.Pleiger.Production.Services.IService;
using Modules.Pleiger.Production.Services.ServiceImp;

namespace Modules.Pleiger.Production
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {      
            services.AddTransient<IMESProductionRequestService, MESProductionRequestService>();
            services.AddTransient<IMESProductionService, MESProductionService>();
            services.AddTransient<IMESProjectStatusService, MESProjectStatusService>();
            services.AddTransient<IMESMaterialsStatusService, MESMaterialsStatusService>();
            services.AddTransient<IMESProductStatusService, MESProductStatusService>();
            services.AddTransient<IMESMaterialIssueService, MESMaterialIssueService>();
            services.AddTransient<IMESOrderStatusService, MESOrderStatusService>();
            services.AddTransient<IMESProductionResultService, MESProductionResultService>();
            services.AddTransient<IMESProjectWarehouseInventoryService, MESProjectWarehouseInventoryService>();
            services.AddTransient<IMESProductionLineService, MESProductionLineService>();
           
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
