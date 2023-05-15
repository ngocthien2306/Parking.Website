using InfrastructureCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Modules.Pleiger.PurchaseOrder.Services.ServiceImp;
using Modules.Pleiger.PurchaseOrder.Services.IService;

namespace Modules.Pleiger.PurchaseOrder
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IPORequestService, PORequestService>();
            services.AddTransient<IPurchaseService, PurchaseService>();
            services.AddTransient<IMESPurchaseOrderDeliveryService, MESPurchaseOrderDeliveryService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
