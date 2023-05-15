using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Modules.Parking.TableDependencies;

namespace Modules.Parking.MiddlewareExtensions
{
    public static class AplicationBuilderExtension
    {
        public static void UseTrackTableDependency(this IApplicationBuilder applicationBuilder)
        {
            var serviceProvider = applicationBuilder.ApplicationServices;
            var service = serviceProvider.GetService<TrackDependency>();
            service.SubcribeTableDependency();
        }
    }
}
