using InfrastructureCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Modules.Pleiger.FileUpload.Services.IService;
using Modules.Pleiger.FileUpload.Services.Service;
using Modules.Pleiger.SalesProject.Services.IService;
using Modules.Pleiger.SalesProject.Services.ServiceImp;

namespace Modules.Pleiger.SalesProject
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMESSaleProjectService, MESSaleProjectService>();
            services.AddTransient<ITaskDrawingService, TaskDrawingService>();
            services.AddTransient<IUploadFileWithTemplateService, UploadFileWithTemplateService>();
            services.AddTransient<IMESSaleOrderProjectService, MESSaleOrderProjectService>();
            services.AddTransient<IMESSaleOrderProjectService, MESSaleOrderProjectService>();

            services.AddTransient<IMESProductionProjectService, MESProductionProjectService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
