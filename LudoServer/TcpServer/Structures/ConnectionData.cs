using System;
using System.Net;
using System.Net.Sockets;

namespace server
{
    public class ConnectionData
    {
        public int? Id { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }
        public TcpClient Client { get; set; }

        public ConnectionData(TcpClient client, int? id = null)
        {
            this.Client = client;
            this.RemoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            this.Id = id;
        }
    }
}
