using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace server
{

    public class ClientListener
    {
        // Properties
        private static bool Listen;
        private TcpListener Listener;
        private Thread ThreadListen;

        // Events properties
        private List<IClientListener> Observers;

        // Methods
        public ClientListener(ref TcpListener Listener)
        {
            this.Listener = Listener;
            Observers = new List<IClientListener>();
        }

        public void Start()
        {
            Listen = true;
            ThreadListen = new Thread(new ThreadStart(Process));
            ThreadListen.Start();
            OnClientListenerStateChanged();
        }

        public void Stop()
        {
            Listen = false;
            Listener.Stop();
            ThreadListen.Join(); // Blocks the calling thread until ThreadListen ends.
        }

        private void Process()
        {
            try
            {
                Listener.Start();
                Console.WriteLine("(Listener)\tListener up.");

                while (Listen)
                {
                    TcpClient client = Listener.AcceptTcpClient();
                    Console.WriteLine("(Listener)\tClient Accepted!");
                    OnClientConnected(client);
                }

            }
            catch (SocketException)
            {
                Console.WriteLine("(Listener)\tListener interrupted!");
            }
            catch (Exception e)
            {
                Console.WriteLine("(Listener)\tCan't accept conections.");
                Console.WriteLine(e.ToString());
            }
            finally
            {
                Listener.Stop();
            }
            Console.WriteLine("(Listener)\tListener down.");
        }

        // Interface Methods
        public void RegisterObserver(IClientListener observer)
        {
            Observers.Add(observer);
        }

        public void RemoveObserver(IClientListener observer)
        {
            Observers.Remove(observer);
        }

        // Dispachers
        public void OnClientListenerStateChanged()
        {
            ListenerStateEvent e = new ListenerStateEvent(this, true);
            foreach(IClientListener observer in Observers)
            {
                observer.OnClientListenerStateChanged(e);
            }
        }

        public void OnClientConnected(TcpClient client)
        {
            ConnectionEvent e = new ConnectionEvent(this, new ConnectionData(client));
            foreach (IClientListener observer in Observers)
            {
                observer.OnClientConnected(e);
            }
        }

    } // class ClientListener
}
