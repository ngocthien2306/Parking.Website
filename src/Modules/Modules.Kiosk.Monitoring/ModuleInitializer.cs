using InfrastructureCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Modules.Kiosk.Monitoring.Repositories.IRepository;
using Modules.Kiosk.Monitoring.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Kiosk.Monitoring
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IKIOCheckInRepository, KIOCheckInRepository>();
            services.AddTransient<IKIOSubscriptionRepository, KIOSubscriptionRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
