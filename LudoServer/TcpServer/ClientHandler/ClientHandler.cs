using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Xml.Serialization;
using TcpProtocol;

namespace server
{
    public class ClientHandler
    {
        //Properties
        private static bool Handle;
        private int Id;
        private TcpClient Client;
        private Thread ClientThread;
        private Type AppPackageType;

        // Events properties
        private List<IClientHandler> Observers;

        // Constructor
        public ClientHandler(int Id, TcpClient Client, Type AppPackageType)
        {
            this.Id = Id;
            this.Client = Client;
            this.AppPackageType = AppPackageType;
            this.Observers = new List<IClientHandler>();
        }

        // Main methods
        public void Start()
        {
            Handle = true;
            ClientThread = new Thread(new ThreadStart(Receive));
            ClientThread.Start();
        }

        public void Stop()
        {
            Handle = false;
            ClientThread.Join();
            //if (ClientThread != null && ClientThread.IsAlive) ClientThread.Join();
        }

        // Handler actions
        public void Send(Package package)
        {
            NetworkStream networkStream = Client.GetStream();
            XmlSerializer serializer = new XmlSerializer(typeof(Package), new Type[] { AppPackageType });
            serializer.Serialize(networkStream, package);
        }

        private void Receive()
        {
            if (Client != null)
            {
                Console.WriteLine("(Handler#" + Id + ")\tStarting thread for client#" + Id + " from {0}:{1}.", ((IPEndPoint)Client.Client.RemoteEndPoint).Address, ((IPEndPoint)Client.Client.RemoteEndPoint).Port);
                Client.ReceiveTimeout = 10000; // 10s
                byte[] bytes; // Incoming data buffer.
                NetworkStream networkStream = Client.GetStream();
                XmlSerializer serializer = new XmlSerializer(typeof(Package), new Type[] { AppPackageType });

                while (Handle)
                {
                    try
                    {
                        bytes = new byte[Client.ReceiveBufferSize]; // 8192 Bytes
                        int BytesRead = networkStream.Read(bytes, 0, (int)Client.ReceiveBufferSize); // Receiving response

                        if (BytesRead > 0)
                        {
                            MemoryStream memoryStream = new MemoryStream(bytes);
                            Package package = (Package)serializer.Deserialize(memoryStream);
                            if (package.type == Protocol.TypeCode("message"))
                            {
                                Console.WriteLine("(Handler#" + Id + ")\tMessage Package received ({0} Bytes).", BytesRead);
                                OnClientMessageReceived(package);
                            }
                            else
                            {
                                //Console.WriteLine("(Handler#" + Id + ")\tKeep Package received ({0} Bytes).", BytesRead);
                                serializer.Serialize(networkStream, Protocol.GetPackage("keep"));
                            }
                        }
                        else
                        {
                            // Other end closed or not responding (BytesRead == 0)
                            throw new System.Net.Sockets.SocketException();
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        //break;
                    }
                    catch (IOException)  // Timeout
                    {
                        Console.WriteLine("(Handler#" + Id + ")\tClient timed out!");
                        Handle = false;
                    }
                    catch (SocketException)
                    {
                        Console.WriteLine("(Handler#" + Id + ")\tConection broken!");
                        break;
                    }
                    Thread.Sleep(200); // 0.2s
                }

                networkStream.Close();
                Client.Close();
                Console.WriteLine("(Handler#" + Id + ")\tClient stopped.");
            }
        }

        // Helper Methods
        public bool Alive
        {
            get
            {
                return (ClientThread != null && ClientThread.IsAlive);
            }
        }


        // Interface Methods
        public void RegisterObserver(IClientHandler observer)
        {
            Observers.Add(observer);
        }

        public void RemoveObserver(IClientHandler observer)
        {
            Observers.Remove(observer);
        }

        // Dispachers
        public void OnClientMessageReceived(Package package)
        {
            HandlerMessageData data = new HandlerMessageData(this.Id, package);
            HandlerMessageEvent e = new HandlerMessageEvent(this, data);
            foreach (IClientHandler observer in Observers)
            {
                observer.OnClientMessageReceived(e);
            }
        }

        public void OnClientDisconnected()
        {
            ConnectionData connection = new ConnectionData(this.Client, this.Id);
            DisconnectionEvent e = new DisconnectionEvent(this, connection);
            foreach (IClientHandler observer in Observers)
            {
                observer.OnClientDisconnected(e);
            }
        }

    }

}
