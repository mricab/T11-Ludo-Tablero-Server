using System;
namespace server
{
    public class ServerMessageData
    {
        public int Id { get; set; }
        public Object Obj { get; set; }

        public ServerMessageData(int id, Object obj)
        {
            this.Id = id;
            this.Obj = obj;
        }
    }
}
