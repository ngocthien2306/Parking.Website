using InfrastructureCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Modules.Kiosk.Member.Repositories.IRepository;
using Modules.Kiosk.Member.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Kiosk.Member
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IKIOMemberManagement, KIOMemberManagement>();
            services.AddTransient<IKIOMemberRemoveMgt, KIOMemberRemoveMgt>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
