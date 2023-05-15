using InfrastructureCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Modules.Pleiger.Inventory.Services.IService;

using Modules.Pleiger.Inventory.Services.ServiceImp;
using Modules.Pleiger.MasterData.Services.IService;
using Modules.Pleiger.MasterData.Services.ServiceImp;

namespace Modules.Pleiger.Inventory
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMESWarehouseService, MESWarehouseService>();
            services.AddTransient<IMESWHInventoryStatusService, MESWHInventoryStatusService>();
            services.AddTransient<IMESVirtualWarehouseService, MESVirtualWarehouseService>();
            services.AddTransient<IMESInventoryService, MESInventoryService>();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
