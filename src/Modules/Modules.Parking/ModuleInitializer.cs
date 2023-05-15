using InfrastructureCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Modules.Parking.Repositories.IRepo;
using Modules.Parking.Repositories.Repo;

namespace Modules.Parking.Views
{
    class ModuleInitializer : IModuleInitializer
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IParkingRepository, ParkingRepository>();
            services.AddTransient<IVehicleCheckinRepository, VehicleCheckinRepository>();

        }
    }
}
