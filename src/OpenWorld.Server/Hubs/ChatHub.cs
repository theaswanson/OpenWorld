using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace OpenWorld.Server.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message)
        {
            var user = Context.UserIdentifier;

            await Console.Out.WriteLineAsync($"Got message: {user} {message}");

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
