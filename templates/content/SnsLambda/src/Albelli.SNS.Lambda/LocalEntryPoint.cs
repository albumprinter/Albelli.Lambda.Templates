using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Albelli.SNS.Lambda
{
    /// <summary>
    /// Use this to test your logic. For testing whole pipeline have a look at Local.LocalEntryPoint
    /// </summary>
    [PublicAPI]
    public sealed class LocalEntryPoint
    {
        public static async Task Main(string[] args)
        {
            await BuildWebHost(args).RunAsync();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}