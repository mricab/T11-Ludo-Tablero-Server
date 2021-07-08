using System;
namespace server
{
    public class ListenerStateEvent
    {
        public bool State;
     
        public ListenerStateEvent(Object source, bool state)
        {
            this.State = state;
        }
    }
}
