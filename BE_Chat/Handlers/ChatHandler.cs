using BE_Chat.Services;

namespace BE_Chat.Handlers
{
    public class ChatHandler : WebSocketHandler
    {
        public ChatHandler(ConnectionService connectionManager) : base(connectionManager)
        {
        }
    }
}
