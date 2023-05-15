
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Modules.Kiosk.Monitoring.TableDependencies;


namespace Modules.Kiosk.Monitoring.MiddlewareExtensions
{
    public static class AplicationBuilderExtension
    {
        public static void UseCheckInTableDependency(this IApplicationBuilder applicationBuilder)
        {
            var serviceProvider = applicationBuilder.ApplicationServices;
            var service = serviceProvider.GetService<SubcribeCheckInTableDependency>();
            service.SubcribeTableDependency();
        }
    }
}
