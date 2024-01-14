using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace OpenWorld.Server.Hubs;

[Authorize]
public class ChatHub(ILogger<ChatHub> logger) : Hub
{
    private readonly ILogger<ChatHub> _logger = logger;

    public async Task SendMessage(string message)
    {
        var user = Context.UserIdentifier;

        _logger.LogInformation("Got message: {user} {message}", user, message);

        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
