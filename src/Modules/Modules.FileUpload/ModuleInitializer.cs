using InfrastructureCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Modules.FileUpload.Services.IService;
using Modules.FileUpload.Services.ServiceImp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.FileUpload
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IFileService, FileService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
