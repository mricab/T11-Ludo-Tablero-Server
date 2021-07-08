using System;
using System.Collections.Generic;
using System.Threading;

namespace server
{
    public class ClientReclaimer
    {
        //Properties
        private static bool Reclaim = false;
        private Dictionary<int, ClientData> Clients;
        private List<int> ClientsToReclaim;
        private Thread ThreadReclaim;

        //Events properties
        private List<IClientReclaimer> Observers = new List<IClientReclaimer>();

        //Methods
        public ClientReclaimer(ref Dictionary<int, ClientData> Clients)
        {
            this.Clients = Clients;
        }

        public void Start()
        {
            Reclaim = true;
            ThreadReclaim = new Thread(new ThreadStart(Process));
            ThreadReclaim.Start();
            OnClientReclaimerStateChanged(true);
        }

        public void Stop()
        {
            Reclaim = false;
            ThreadReclaim.Join(); // Blocks the calling thread until ThreadReclaim ends.
        }

        public void Process()
        {
            Console.WriteLine("(Reclaimer)\tReclaimer up.");

            while (Reclaim)
            {
                ClientsToReclaim = new List<int>();

                foreach (KeyValuePair<int, ClientData> Client in Clients)
                {
                    if(!Client.Value.Handler.Alive)
                    {
                        ClientsToReclaim.Add(Client.Key);
                    }
                }
                foreach(int key in ClientsToReclaim)
                {
                    Console.WriteLine("(Reclaimer)\tClaiming client#{0}.", key);
                    OnClientConnectionReclaimed(key);
                }

                Thread.Sleep(2000); // 2s
            }
            Console.WriteLine("(Reclaimer)\tReclaimer down.");
        }

        // Interface Methods
        public void RegisterObserver(IClientReclaimer observer)
        {
            Observers.Add(observer);
        }

        public void RemoveObserver(IClientReclaimer observer)
        {
            Observers.Remove(observer);
        }

        // Dispachers
        public void OnClientReclaimerStateChanged(bool state)
        {
            ReclaimerStateEvent e = new ReclaimerStateEvent(this, state);
            foreach (IClientReclaimer observer in Observers)
            {
                observer.OnClientReclaimerStateChanged(e);
            }
        }

        public void OnClientConnectionReclaimed(int id)
        {
            ReclaimEvent e = new ReclaimEvent(this, id);
            foreach (IClientReclaimer observer in Observers)
            {
                observer.OnClientConnectionReclaimed(e);
            }
        }
    }
}
