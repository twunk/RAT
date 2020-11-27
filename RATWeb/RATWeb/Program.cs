using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace RATWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                // Old config load
                //.ConfigureServices((context, services) =>
                //{
                //    services.Configure<KestrelServerOptions>(
                //        context.Configuration.GetSection("Kestrel"));
                //})
                .UseStartup<Startup>()
                .UseKestrel(serverOptions =>
                {
                    // Set Kestrel options
                    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(20);
                    serverOptions.Limits.MaxConcurrentConnections = 64;
                    serverOptions.Limits.MaxConcurrentUpgradedConnections = 64;
                    serverOptions.Limits.MaxRequestBodySize = 2 * 1024;

                    serverOptions.Limits.MinRequestBodyDataRate =
                        new MinDataRate(
                            bytesPerSecond: 124, 
                            gracePeriod: TimeSpan.FromSeconds(5)
                        );

                    serverOptions.Limits.MinResponseDataRate =
                        new MinDataRate(
                            bytesPerSecond: 124, 
                            gracePeriod: TimeSpan.FromSeconds(5)
                        );

                    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(28);

                    serverOptions.AllowSynchronousIO = false;

                    // Configure endpoint
                    serverOptions.ConfigureEndpointDefaults(listenOptions =>
                    {
                        // listenOptions.ServerCertificate = certificate;


                    });

                    serverOptions.Listen(IPAddress.Loopback, 16420, listenOptions =>
                    {
                        listenOptions.UseConnectionLogging();
                        // listenOptions.UseHttps("testCert.pfx", "testPassword");
                    });

                });
    }
}
