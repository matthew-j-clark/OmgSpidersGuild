using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SpiderDiscordBot;

namespace OmgSpiders
{
    public class Program
    {
        public static void Main(string[] args)
        {
            OmgSpidersBotDriver = new OmgSpidersBotDriver();
            OmgSpidersBotDriver.StartBot();
            CreateWebHostBuilder(args).Build().Run();            
        }

        public static OmgSpidersBotDriver OmgSpidersBotDriver { get; set; }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
