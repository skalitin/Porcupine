using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;
using System.Net;

namespace Porcupine.Agent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.ConfigureHttpsDefaults(httpsOptions =>
                        {
                            httpsOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                            httpsOptions.CheckCertificateRevocation = false;
                            httpsOptions.ClientCertificateValidation = (certificate, chain, arg3) =>
                            {
                                return certificate.Thumbprint == "9D16801332A4EBB0469030AF66C88CCDC0E288FB";
                            };
                        });

                         // options.Listen(System.Net.IPAddress.Any, 5001, listenOptions =>
                         // {
                         //     listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                         // });
                     });

                    webBuilder.UseStartup<Startup>();
                });
    }
}
