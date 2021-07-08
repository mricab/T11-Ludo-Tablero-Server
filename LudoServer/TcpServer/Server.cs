using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TcpProtocol;

namespace server
{
    public class Server : IClientListener, IClientReclaimer, IClientHandler
    {
        // Properties
        private Dictionary<int, ClientData> Clients;
        private static ClientListener Listener;
        private static ClientReclaimer Reclaimer;
        private int Connections;
        private int Disconnections;
        private bool ListenerUp;
        private bool ReclaimerUp;

        // Received Properties
        private TcpListener ListenerSocket;
        private Type ApplicationPackageType;

        // Events properties
        private List<IServer> Observers;

        // Constructor
        public Server(int ListeningPort, Type PackageType)
        {
            try
            {
                Clients = new Dictionary<int, ClientData>();
                Reclaimer = new ClientReclaimer(ref Clients);
                ListenerSocket = new TcpListener(IPAddress.Loopback, ListeningPort);
                ApplicationPackageType = PackageType;
                Listener = new ClientListener(ref ListenerSocket);
                Connections = 0;
                Disconnections = 0;
                ListenerUp = false;
                ReclaimerUp = false;
                Observers = new List<IServer>();
            }
            catch (Exception e)
            {
                Console.WriteLine("(Server)\tServer couldn't be initialized.");
                Console.WriteLine(e.ToString());
            }
        }

        // Main Methods
        public void Start()
        {
            Console.WriteLine("\n(Server)\tStarting server on port {0}.", ((IPEndPoint)ListenerSocket.LocalEndpoint).Port );
            // Events subscriptions
            Listener.RegisterObserver(this); // Registers this instance
            Reclaimer.RegisterObserver(this); // Registers this instance
            // Server main processes
            Reclaimer.Start(); 
            Listener.Start();
            // Closing message
            Thread.Sleep(1000); // 1s
            if(ListenerUp && ReclaimerUp) Console.WriteLine("(Server)\tServer up.");
        }

        public void Stop()
        {
            Console.WriteLine("(Server)\tStopping server.");
            Listener.Stop();
            Reclaimer.Stop();
            CloseClients(); // Closing all remaining connections
            Console.WriteLine("(Server)\tServer down.");
        }

        public void Restart()
        {
            Console.WriteLine("(Server)\tRestarting server on port {0}.", ((IPEndPoint)ListenerSocket.LocalEndpoint).Port);
            Console.WriteLine("(Server)\tServer up.");
        }

        public void Pause()
        {
            Console.WriteLine("(Server)\tPausing server.");
            Console.WriteLine("(Server)\tServer paused.");
        }

        // Actions
        public void Send(int connection, object obj)
        {
            ClientData clientData;
            if (Clients.TryGetValue(connection, out clientData))
            {
                Package package = Protocol.GetPackage("message", obj);
                clientData.Handler.Send(package);
            }
            else
            {
                throw new Exception("Invalid connection supplied.");
            }            
        }


        // Client's handling methods
        private void StartClient(int key)
        {
            ClientData clientData;
            Clients.TryGetValue(Connections, out clientData);
            clientData.Handler.RegisterObserver(this);
            clientData.Handler.Start();
            clientData.Handler.Send(Protocol.GetPackage("connected", key));
        }

        private void StopClient(int key)
        {
            ClientData clientData;
            Clients.TryGetValue(Connections, out clientData);
            clientData.Handler.Stop();
        }

        private void CloseClients()
        {
            List<int> ClientsToRemove = new List<int>();

            Console.WriteLine("(Server)\tRemoving remaining clients.");
            foreach (KeyValuePair<int, ClientData> Client in Clients)
            {
                ClientsToRemove.Add(Client.Key);
                Client.Value.Handler.Stop();
            }
            foreach (int key in ClientsToRemove)
            {
                Console.WriteLine("(Server)\tRemoving client#{0}.", key);
                Clients.Remove(key);
            }
            Console.WriteLine("(Server)\tNo clients left.");
        }

        // IClientListener
        public void OnClientListenerStateChanged(ListenerStateEvent e)
        {
            ListenerUp = e.State;
        }

        public void OnClientConnected(ConnectionEvent e)
        {
            ++Connections;
            Console.WriteLine("(Server)\tAdding client#{0}.", Connections);
            ClientData clientData = new ClientData(e.Data.Client, new ClientHandler(Connections, e.Data.Client, ApplicationPackageType));
            Clients.Add(Connections, clientData);
            StartClient(Connections);
        }

        // IClientReclaimer
        public void OnClientReclaimerStateChanged(ReclaimerStateEvent e)
        {
            ReclaimerUp = e.State;
        }

        public void OnClientConnectionReclaimed(ReclaimEvent e)
        {
            ++Disconnections;
            Console.WriteLine("(Server)\tRemoving client#{0} due to timeout or broken connection.", e.Id);
            // No need to StopClient(), ClientHandler stops itself.
            Clients.Remove(e.Id);
        }

        // IClientHandler
        public void OnClientDisconnected(DisconnectionEvent e)
        {
            ++Disconnections;
            Console.WriteLine("(Server)\tRemoving client#{0}.", e.Connection.Id);
            StopClient(e.Connection.Id.Value);
            Clients.Remove(e.Connection.Id.Value);
        }

        public void OnClientMessageReceived(HandlerMessageEvent e)
        {
            Console.WriteLine("(Server)\tClient#{0}: Message Received", e.Data.Id);
            OnMessageReceived(e.Data.Id, e.Data.Package.obj);
        }

        // Interface Methods
        public void RegisterObserver(IServer observer)
        {
            Observers.Add(observer);
        }

        public void RemoveObserver(IServer observer)
        {
            Observers.Remove(observer);
        }

        // IServer Dispatchers
        public void OnMessageReceived(int id, Object obj)
        {
            ServerMessageData message = new ServerMessageData(id, obj);
            ServerMessageEvent e = new ServerMessageEvent(this, message);
            foreach (IServer observer in Observers)
            {
                observer.OnMessageReceived(e);
            }
        }

    }
}
