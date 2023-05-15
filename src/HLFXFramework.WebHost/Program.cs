using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Modular.WebHost
{
    public class Program
    {
        //public static void Main(string[] args)
        //{
        //    var host = new WebHostBuilder()
        //        .UseKestrel()
        //        .UseContentRoot(Directory.GetCurrentDirectory())
        //        .UseIISIntegration()
        //        .UseStartup<Startup>()
        //        .Build();

        //    host.Run();
        //}

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>();
                });
        }

        //public static void Main(string[] args)
        //{
        //    BuildWebHost2(args).Build().Run();
        //}

        //// Changed to BuildWebHost2 to make EF don't pickup during design time
        //private static IHostBuilder BuildWebHost2(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .UseContentRoot(Directory.GetCurrentDirectory())
        //        .ConfigureWebHostDefaults(webBuilder => {
        //            webBuilder.UseStartup<Startup>();
        //            webBuilder.ConfigureAppConfiguration(SetupConfiguration);
        //            webBuilder.ConfigureLogging(SetupLogging);
        //            webBuilder.UseIISIntegration();
        //        });

        private static void SetupConfiguration(WebHostBuilderContext hostingContext, IConfigurationBuilder configBuilder)
        {
            var env = hostingContext.HostingEnvironment;
            var configuration = configBuilder.Build();
            //configBuilder.AddEntityFrameworkConfig(options =>
            //        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            //);
            Log.Logger = new LoggerConfiguration()
                       .ReadFrom.Configuration(configuration)
                       .CreateLogger();
        }
        private static void SetupLogging(WebHostBuilderContext hostingContext, ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddSerilog();
        }

    }
}
