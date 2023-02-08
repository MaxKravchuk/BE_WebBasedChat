﻿using BE_Chat.Handlers;
using BE_Chat.Models;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace BE_Chat
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebSocketHandler _handler;
        public WebSocketMiddleware(RequestDelegate next, IWebSocketHandler handler)
        {
            _next = next;
            _handler = handler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            string username = context.Request.Query["username"];

            WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
            await _handler.OnConnected(socket, username);

            await Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var msg = _handler.ReceiveString(result, buffer);

                    await HandleMessage(socket, msg);

                    return;
                }

                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await HandleDisconnect(socket);
                    return;
                }

            });
        }

        private async Task HandleDisconnect(WebSocket socket)
        {
            string disconnectedUser = await _handler.OnDisconnected(socket);

            ServerMessage disconnectMessage = new ServerMessage(disconnectedUser, true, _handler.GetAllUsers());

            await _handler.BroadcastMessage(JsonSerializer.Serialize(disconnectMessage));
        }

        private async Task HandleMessage(WebSocket socket, string message)
        {
            ClientMessage clientMessage = TryDeserializeClientMessage(message);

            if (clientMessage == null)
            {
                return;
            }

            if (clientMessage.IsTypeConnection())
            {
                // For future improvements
            }
            else if (clientMessage.IsTypeChat())
            {
                string expectedUsername = _handler.GetUsernameBySocket(socket);

                if (clientMessage.IsValid(expectedUsername))
                {
                    ServerMessage chatMessage = new ServerMessage(clientMessage);
                    await _handler.BroadcastMessage(JsonSerializer.Serialize(chatMessage));
                }
            }
        }

        private ClientMessage TryDeserializeClientMessage(string str)
        {
            try
            {
                return JsonSerializer.Deserialize<ClientMessage>(str);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: invalid message format");
                return null;
            }
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                           cancellationToken: CancellationToken.None);

                    handleMessage(result, buffer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                await HandleDisconnect(socket);
            }
        }
    }
}
