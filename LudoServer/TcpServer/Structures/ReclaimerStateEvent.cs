using System;
namespace server
{
    public class ReclaimerStateEvent
    {
        public bool State;

        public ReclaimerStateEvent(Object source, bool state)
        {
            this.State = state;
        }
    }
}
