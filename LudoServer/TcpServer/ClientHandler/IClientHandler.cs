using System;
namespace server
{
    public interface IClientHandler
    {
        void OnClientDisconnected(DisconnectionEvent e);
        void OnClientMessageReceived(HandlerMessageEvent e);
    }
}
