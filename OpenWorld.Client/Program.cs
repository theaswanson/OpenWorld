namespace OpenWorld.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Console.Out.WriteLineAsync("[OpenWorld Client] Started.");

            //await SendTestMessage();
            await TestAuthenticate();

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

        private static async Task<string> TestAuthenticate()
        {
            await Console.Out.WriteAsync("Username:");
            var username = await Console.In.ReadLineAsync();

            await Console.Out.WriteAsync("Password:");
            var password = await Console.In.ReadLineAsync();

            var authenticationClient = new AuthenticationClient();

            return await authenticationClient.AuthenticateAsync(username, password);
        }
    }
}
