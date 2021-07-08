using System;

namespace server
{
    public interface IClientListener
    {
        void OnClientListenerStateChanged(ListenerStateEvent e);
        void OnClientConnected(ConnectionEvent e);
    }
}
