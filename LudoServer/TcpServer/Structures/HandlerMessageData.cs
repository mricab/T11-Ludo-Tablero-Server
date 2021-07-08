using System;
using TcpProtocol;

namespace server
{
    public class HandlerMessageData
    {
        public int Id { get; set; }
        public Package Package { get; set; }

        public HandlerMessageData(int id, Package Package)
        {
            this.Id = id;
            this.Package = Package;
        }
    }
}
