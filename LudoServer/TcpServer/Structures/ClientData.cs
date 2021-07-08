using System;
using System.Net.Sockets;

namespace server
{
    public class ClientData
    {
        public TcpClient Client { get; set; }
        public ClientHandler Handler { get; set; }

        public ClientData(TcpClient client, ClientHandler handler)
        {
            this.Client = client;
            this.Handler = handler;
        }
    }
}
