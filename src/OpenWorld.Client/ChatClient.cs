using Microsoft.AspNetCore.SignalR.Client;
using OpenWorld.Client.Authentication;

namespace OpenWorld.Client
{
    internal class ChatClient
    {
        private readonly IAuthenticationService _authenticationService;

        private HubConnection? _connection;
        private string? _token = null;

        public ChatClient(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task ConnectAsync(string url, string username, string password)
        {
            if (_connection is not null)
            {
                if (_connection.State != HubConnectionState.Disconnected)
                {
                    await _connection.StopAsync();
                }

                await _connection.DisposeAsync();
            }

            _connection = new HubConnectionBuilder()
                .WithUrl(url, options =>
                {
                    options.AccessTokenProvider = () => GetAccessToken(username, password);
                })
                .Build();

            ConfigureConnectionHandlers(_connection);
            ConfigureMessageHandlers(_connection);

            await Console.Out.WriteLineAsync("Connecting...");

            await _connection.StartAsync();

            await Console.Out.WriteLineAsync("Connected.");
        }

        public async Task SendMessageAsync(string message)
        {
            if (_connection is null || _connection.State != HubConnectionState.Connected)
            {
                throw new Exception("Not connected.");
            }

            await Console.Out.WriteLineAsync("Sending message...");

            await _connection.SendAsync("SendMessage", message);

            await Console.Out.WriteLineAsync("Sent.");
        }

        private static void ConfigureConnectionHandlers(HubConnection connection)
        {
            connection.Closed += async (error) =>
            {
                if (error != null)
                {
                    await Console.Out.WriteLineAsync($"Connection closed due to error: {error.Message}");
                    return;
                }

                await Console.Out.WriteLineAsync("Connection closed.");
            };

            connection.Reconnecting += async (error) =>
            {
                if (error != null)
                {
                    await Console.Out.WriteLineAsync($"Connection reconnecting due to error: {error.Message}");
                    return;
                }

                await Console.Out.WriteLineAsync("Connection lost. Attempting to reestablish...");
            };

            connection.Reconnected += async (connectionId) =>
            {
                await Console.Out.WriteLineAsync($"Reconnected with new connection ID {connectionId}");
            };
        }

        private static void ConfigureMessageHandlers(HubConnection connection)
        {
            connection.On<string, string>("ReceiveMessage", async (user, message) =>
            {
                await Console.Out.WriteLineAsync($"[ReceiveMessage] {user} {message}");
            });
        }

        private async Task<string?> GetAccessToken(string username, string password)
        {
            if (_token is not null && _authenticationService.IsTokenValid(_token, DateTime.UtcNow))
            {
                return _token;
            }

            var result = await _authenticationService.AuthenticateAsync(username, password);

            if (!result.IsSuccessful)
            {
                return null;
            }

            _token = result.Success!.Token;

            return _token;
        }
    }
}
