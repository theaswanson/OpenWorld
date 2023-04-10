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

        private static async Task TestAuthenticate()
        {
            var username = await Prompt("Username: ");
            var password = await Prompt("Password: ");

            var authenticationClient = new AuthenticationClient();

            var token = await authenticationClient.AuthenticateAsync(username, password);

            await Console.Out.WriteLineAsync(token);
        }

        private static async Task<string> Prompt(string prompt)
        {
            string? value = null;

            while (value is null)
            {
                await Console.Out.WriteAsync(prompt);

                value = await Console.In.ReadLineAsync();
            }

            return value;
	    }
    }
}
