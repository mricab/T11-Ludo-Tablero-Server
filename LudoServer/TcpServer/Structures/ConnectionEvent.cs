using System;
namespace server
{
    public class ConnectionEvent
    {
        public ConnectionData Data { get; }

        public ConnectionEvent(Object source, ConnectionData data)
        {
            this.Data = data;
        }
    }
}
