using System;
namespace server
{
    public class DisconnectionEvent
    {
        public ConnectionData Connection;

        public DisconnectionEvent(object source, ConnectionData connection)
        {
            this.Connection = connection;
        }
    }
}
