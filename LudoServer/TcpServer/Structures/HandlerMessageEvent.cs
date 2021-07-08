using System;
namespace server
{
    public class HandlerMessageEvent
    {
        public HandlerMessageData Data;

        public HandlerMessageEvent(object source, HandlerMessageData data)
        {
            this.Data = data;
        }
    }
}
