using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Dispel.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IHost BuildWebHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(web => web.UseStartup<Startup>())
                .Build();
    }
}
