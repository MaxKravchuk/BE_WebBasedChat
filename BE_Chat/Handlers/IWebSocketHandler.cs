using System.Net.WebSockets;

namespace BE_Chat.Handlers
{
    public interface IWebSocketHandler
    {
        Task OnConnected(WebSocket socket, string username);
        string ValidateUsername(string username);
        Task<string> OnDisconnected(WebSocket socket);
        Task SendMessageAsync(WebSocket socket, string message);
        Task SendMessageAsync(string socketId, string message);
        Task SendMessageToAllAsync(string message);
        Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
        string ReceiveString(WebSocketReceiveResult result, byte[] buffer);
        Task BroadcastMessage(string message);
        List<string> GetAllUsers();
        string GetUsernameBySocket(WebSocket socket);
    }
}
