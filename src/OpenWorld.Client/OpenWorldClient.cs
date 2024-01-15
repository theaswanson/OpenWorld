using Microsoft.AspNetCore.SignalR.Client;

namespace OpenWorld.Client;

public class OpenWorldClient(ChatClient chatClient)
{
    private readonly ChatClient _chatClient = chatClient;

    public async Task JoinChatroomAsync()
    {
        await ConnectToChatroomAsync();

        var input = GetChatAction();

        while (input.Key != ConsoleKey.Q)
        {
            if (input.Key == ConsoleKey.Enter)
            {
                await SendMessageAsync();
            }

            input = GetChatAction();
        }

        async Task ConnectToChatroomAsync()
        {
            if (_chatClient.State == HubConnectionState.Connected)
            {
                return;
            }

            var connected = false;

            while (!connected)
            {
                var username = await Prompt("Username: ");
                var password = await Prompt("Password: ");

                connected = await _chatClient.ConnectAsync(new UriBuilder("https", "localhost", 7192, "/hubs/chat").ToString(), username, password);
            }
        }

        async Task SendMessageAsync()
        {
            string? message = await Prompt("> ");

            await _chatClient.SendMessageAsync(message);
        }

        static ConsoleKeyInfo GetChatAction()
        {
            Console.WriteLine("[Q] Quit, [Enter] Send Message");

            return Console.ReadKey(intercept: true);
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
