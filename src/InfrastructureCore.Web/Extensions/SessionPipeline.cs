using InfrastructureCore.Web.Services.IService;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace InfrastructureCore.Web.Extensions
{
    public class SessionPipeline
    {
        private readonly ISessionService _sessionService;

        public SessionPipeline(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public int GetSessionTimeoutFromDB()
        {
            return _sessionService.GetSessionTimeoutFromSite();
        }

    }
    public static class SessionExtension
    {
        public static void ConfigureSession(this IServiceCollection serviceCollection, ISessionService sessionService)
        {
            var sessionpineline = new SessionPipeline(sessionService);
            int sessionTimeoutNum = sessionpineline.GetSessionTimeoutFromDB();
            serviceCollection.AddSession(opts =>
            {
                opts.IdleTimeout = TimeSpan.FromMinutes(sessionTimeoutNum);
            });
        }
    }
}
