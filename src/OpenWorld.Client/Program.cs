using OpenWorld.Client.Authentication;

namespace OpenWorld.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Console.Out.WriteLineAsync("[OpenWorld Client] Started.");

            await SendTestMessage();
            //await TestAuthenticate();

            await Console.Out.WriteLineAsync("[OpenWorld Client] Press any key to quit...");
            Console.ReadKey();
            await Console.Out.WriteLineAsync("[OpenWorld Client] Stopped.");
        }

        private static async Task SendTestMessage()
        {
            var chatClient = new ChatClient(new AuthenticationService(new OpenWorldHttpClient()));

            var connected = false;

            while (!connected)
            {
                var username = await Prompt("Username: ");
                var password = await Prompt("Password: ");

                connected = await chatClient.ConnectAsync(new UriBuilder("https", "localhost", 7192, "/hubs/chat").ToString(), username, password);
            }

            await chatClient.SendMessageAsync("blah");
        }

        private static async Task TestAuthenticate()
        {
            var success = false;

            while (!success)
            {
                var username = await Prompt("Username: ");
                var password = await Prompt("Password: ");

                var authenticationClient = new AuthenticationClient(new AuthenticationService(new OpenWorldHttpClient()));

                success = await authenticationClient.AuthenticateAsync(username, password);
            }
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
