using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Nop.Web
{
    public class Program
    {
        private static IWebHost host;

        public static void Main(string[] args)
        {
            host = WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options => options.AddServerHeader = false)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
