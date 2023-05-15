using InfrastructureCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Modules.Kiosk.Management.Repositories.IRepositories;
using Modules.Kiosk.Management.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Kiosk.Management
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IKIOCommonCodeRepository, KIOCommonCodeRepository>();
            services.AddTransient<IKIOAccountRepository, KIOAccountRepository>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
