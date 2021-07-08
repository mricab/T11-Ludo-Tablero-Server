using System;
namespace server
{
    public class ReclaimEvent
    {
        public int Id { get; }

        public ReclaimEvent(Object source, int id)
        {
            this.Id = id;
        }
    }
}
