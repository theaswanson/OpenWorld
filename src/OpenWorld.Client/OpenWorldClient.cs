using OpenWorld.Client.Authentication;

namespace OpenWorld.Client;

public class OpenWorldClient
{
    private readonly ChatClient _chatClient;

    public OpenWorldClient(ChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task SendTestMessage()
    {
        var connected = false;

        while (!connected)
        {
            var username = await Prompt("Username: ");
            var password = await Prompt("Password: ");

            connected = await _chatClient.ConnectAsync(new UriBuilder("https", "localhost", 7192, "/hubs/chat").ToString(), username, password);
        }

        await _chatClient.SendMessageAsync("blah");
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
