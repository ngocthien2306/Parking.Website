using InfrastructureCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Modules.Admin.Services.IService;
using Modules.Admin.Services.ServiceImp;
using Modules.Pleiger.Services.IService;
using Modules.Pleiger.Services.ServiceImp;

namespace Modules.Pleiger
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // register service
            services.AddTransient<ICompanyService, CompanyService>();
            services.AddTransient<IMESComCodeService, MESComCodeService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IItemClassService, ItemClassService>();
            services.AddTransient<IMESPartnerService, MESPartnerService>();
            services.AddTransient<IMESWarehouseService, MESWarehouseService>();
            services.AddTransient<IMESSaleProjectService, MESSaleProjectService>();
            services.AddTransient<IMESItemService, MESItemService>();
            services.AddTransient<IMESItemPartnerService, MESItemPartnerService>();
            services.AddTransient<IPORequestService, PORequestService>();
            services.AddTransient<IMESItemSlipService, MESItemSlipService>();
            services.AddTransient<IMESProductionService, MESProductionService>();
            services.AddTransient<IMESProductionRequestService, MESProductionRequestService>();
            services.AddTransient<IPurchaseService, PurchaseService>();
            services.AddTransient<IMESInventoryService, MESInventoryService>();
            services.AddTransient<IMESVirtualWarehouseService, MESVirtualWarehouseService>();
            services.AddTransient<IMESWHInventoryStatusService, MESWHInventoryStatusService>();
            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<IMESBOMMgtService, MESBOMMgtService>();
            services.AddTransient<IWidgetService, WidgetService>();
            services.AddTransient<ISiteService, SiteService>();



            services.AddTransient<IBoardManagementService, BoardManagementService>();
            services.AddTransient<IChartService, ChartService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
