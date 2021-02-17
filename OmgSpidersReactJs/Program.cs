using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SpiderDiscordBot;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmgSpidersReactJs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            OmgSpidersBotDriver = new OmgSpidersBotDriver();
            OmgSpidersBotDriver.StartBot();
            CreateHostBuilder(args).Build().Run();
        }

        public static OmgSpidersBotDriver OmgSpidersBotDriver { get; set; }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
