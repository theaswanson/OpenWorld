using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenWorld.Client.Authentication;

namespace OpenWorld.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        await Console.Out.WriteLineAsync("[OpenWorld Client] Starting...");

        var builder = Host.CreateApplicationBuilder(args);

        ConfigureServices(builder.Services);

        var serviceProvider = builder.Services.BuildServiceProvider();

        var client = serviceProvider.GetService<OpenWorldClient>();

        await client!.JoinChatroomAsync();

        await Console.Out.WriteLineAsync("[OpenWorld Client] Press any key to quit...");
        Console.ReadKey(intercept: true);
        await Console.Out.WriteLineAsync("[OpenWorld Client] Stopped.");
    }

    static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IOpenWorldHttpClient, OpenWorldHttpClient>();
        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<ChatClient>();
        services.AddSingleton<OpenWorldClient>();
    }
}
