using OpenWorld.Client.Authentication;

namespace OpenWorld.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Console.Out.WriteLineAsync("[OpenWorld Client] Started.");

            var client = new OpenWorldClient(new ChatClient(new AuthenticationService(new OpenWorldHttpClient())));
            await client.SendTestMessage();

            await Console.Out.WriteLineAsync("[OpenWorld Client] Press any key to quit...");
            Console.ReadKey();
            await Console.Out.WriteLineAsync("[OpenWorld Client] Stopped.");
        }
    }
}
