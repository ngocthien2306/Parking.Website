using InfrastructureCore.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace InfrastructureCore.DataAccess
{
    /// <summary>
    /// Get App config from web appsettings.json (asp.net mvc core 2.2)
    /// </summary>
    public class AppConfig
    {
        public AppConfig()
        {
            var configurationBuilder = new ConfigurationBuilder();
            //Get file information
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            //Get ConnectionString node
            ConnectionString = root.GetSection("ConnectionStrings").GetSection("FrameworkConnection").GetSection("ConnectionString").Value;
            DbType = root.GetSection("ConnectionStrings").GetSection("FrameworkConnection").GetSection("ConnectionType").Value;
            Secret = root.GetSection("AppSettings").GetSection("Secret").Value;
            SiteCode = root.GetSection("MESSiteStartup").GetSection("SiteCode").Value;
            SiteID = root.GetSection("MESSiteStartup").GetSection("SiteID").Value;
            DbConnectionsManager a  = new DbConnectionsManager();
            
            var appSetting = root.GetSection("ApplicationSettings");
        }
        /// <summary>
        /// Database connection string 
        /// </summary>
        public string ConnectionString { get; } = string.Empty;
        public string DbType { get; } = string.Empty;
        public string Secret{ get; } = string.Empty;
        public string SiteCode{ get; } = string.Empty;
        public string SiteID { get; } = string.Empty;


    }
}
