using InfrastructureCore.Modules;
using InfrastructureCore.Web.Services.IService;
using InfrastructureCore.Web.Services.ServiceImpl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Admin.Services.IService;
using Modules.Admin.Services.ServiceImp;

namespace Modules.Admin
{
    public class ModuleInitializer : IModuleInitializer
    {
       // public readonly IConfiguration _configuration;
        public void ConfigureServices(IServiceCollection services)
        {
            // register service
            //services.AddTransient<IWebAuthentication, WebAuthentication>();
            services.AddTransient<IAdminLayoutService, AdminLayoutService>();
            services.AddTransient<IMenuService, MenuService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IModulesService, ModulesService>();
            services.AddTransient<ISiteService, SiteService>();
            services.AddTransient<IAccessMenuService, AccessMenuService>();
            services.AddTransient<IGroupUserService, GroupUserService>();
            services.AddTransient<IToolbarService, ToolbarService>();
            services.AddTransient<IBoardManagementService, BoardManagementService>();
            services.AddTransient<ISendMailServices, SendMailServices>();

            // var sitecode = _configuration.GetSection("MESSiteStartup").GetSection("SiteCode").Value;
            services.AddTransient<ISessionService,SessionService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
