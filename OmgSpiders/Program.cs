using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OmgSpiders.DiscordBot.HrothChecksOut;

namespace OmgSpiders
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HrothBot = new HrothChecksOut();
            HrothBot.StartBot();
            CreateWebHostBuilder(args).Build().Run();

            
        }

        public static HrothChecksOut HrothBot { get; set; }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
