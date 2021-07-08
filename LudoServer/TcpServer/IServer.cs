using System;
namespace server
{
    public interface IServer
    {
        void OnMessageReceived(ServerMessageEvent e);
    }
}
