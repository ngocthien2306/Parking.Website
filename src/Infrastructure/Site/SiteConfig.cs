using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InfrastructureCore.Site
{
    /// <summary>
    /// Get site code startup when loading at appsettings.json
    /// </summary>
    public class SiteConfig
    {
        public SiteConfig()
        {
            var configurationBuilder = new ConfigurationBuilder();
            //Get file information
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            //Get ConnectionString node
            SiteCode = root.GetSection("MESSiteStartup").GetSection("SiteCode").Value;

        }
        /// <summary>
        /// SiteCode string 
        /// </summary>
        public string SiteCode { get; } = string.Empty;
    }
}
