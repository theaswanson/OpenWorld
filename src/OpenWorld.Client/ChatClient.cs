﻿using System.Net;
using Microsoft.AspNetCore.SignalR.Client;
using OpenWorld.Client.Authentication;

namespace OpenWorld.Client;

public class ChatClient(IAuthenticationService authenticationService)
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public HubConnectionState State => _connection?.State ?? HubConnectionState.Disconnected;

    private HubConnection? _connection;
    private string? _token;

    /// <returns>True if a successful connection was made, false otherwise.</returns>
    public Task<bool> ConnectAsync(string url, string username, string password)
    {
        return ConnectAsync(url, () => GetAccessToken(username, password));
    }

    /// <returns>True if a successful connection was made, false otherwise.</returns>
    public Task<bool> ConnectAsync(string url, string token)
    {
        return ConnectAsync(url, () => Task.FromResult<string?>(token));
    }

    private async Task<bool> ConnectAsync(string url, Func<Task<string?>> accessTokenProvider)
    {
        try
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
                    options.AccessTokenProvider = accessTokenProvider;
                })
                .Build();

            ConfigureConnectionHandlers(_connection);
            ConfigureMessageHandlers(_connection);

            await Console.Out.WriteLineAsync("Connecting...");

            await _connection.StartAsync();

            await Console.Out.WriteLineAsync("Connected.");

            return true;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Console.Out.WriteLineAsync("Invalid username or password. Please try again.");
            }
            else
            {
                await Console.Out.WriteLineAsync($"Connection failed. Please try again. ({ex.HttpRequestError})");
                await Console.Out.WriteLineAsync(ex.ToString());
            }

            return false;
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync("Connection failed. Please try again.");
            await Console.Out.WriteLineAsync(ex.ToString());

            return false;
        }
    }

    public async Task SendMessageAsync(string message)
    {
        if (_connection is null || _connection.State != HubConnectionState.Connected)
        {
            throw new Exception("Not connected.");
        }

        await _connection.SendAsync("SendMessage", message);
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
            await Console.Out.WriteLineAsync($"{user}: {message}");
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
