using Microsoft.AspNetCore.SignalR;

namespace OpenWorld.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Console.Out.WriteLineAsync($"Got message: {user} {message}");

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
