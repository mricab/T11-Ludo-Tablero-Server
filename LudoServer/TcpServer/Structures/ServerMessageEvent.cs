using System;
namespace server
{
    public class ServerMessageEvent
    {
        public ServerMessageData Data;

        public ServerMessageEvent(object source, ServerMessageData data)
        {
            this.Data = data;
        }
    }

}
