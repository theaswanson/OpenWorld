using Microsoft.AspNetCore.SignalR.Client;

namespace OpenWorld.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Console.Out.WriteLineAsync("[OpenWorld Client] Started.");

            await SendTestMessage();

            await Console.Out.WriteLineAsync("[OpenWorld Client] Press any key to quit...");
            Console.ReadKey();
            await Console.Out.WriteLineAsync("[OpenWorld Client] Stopping...");
        }

        private static async Task SendTestMessage()
        {
            var chatClient = new ChatClient();

            await chatClient.ConnectAsync(new UriBuilder("https", "localhost", 7192, "/hubs/chat").ToString());

            await chatClient.SendMessageAsync("TheLegend27", "blah");
        }
    }
}
