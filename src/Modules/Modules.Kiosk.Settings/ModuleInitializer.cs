using InfrastructureCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Modules.Kiosk.Management.Repositories.IRepository;
using Modules.Kiosk.Management.Repositories.Repository;
using Modules.Kiosk.Monitoring.Repositories.IRepository;
using Modules.Kiosk.Settings.Repositories.IRepository;
using Modules.Kiosk.Settings.Repositories.Repository;
namespace Modules.Kiosk.Management
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IKIOVoiceFileRepository, KIAVoiceFileRepository>();
            services.AddTransient<IKIOStoreRepository, KIOStoreRepository>();
            services.AddTransient<IKIOEquipmentRepository, KIOEquipmentRepository>();
            services.AddTransient<IKIOAdMgtRepository, KIOAdMgtRepository>();
            services.AddTransient<IKIOPreferenceRepository, KIOPreferenceRepository>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
