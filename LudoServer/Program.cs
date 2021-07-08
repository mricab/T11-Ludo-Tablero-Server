using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameServer;

namespace LudoServer
{
    class Program
    {
        private static GServer GServer;
        static void Main(string[] args)
        {
            int ListeningPort;
            if (args.Length > 0 && ProgramHelperFunctions.GetPort(args[0], out ListeningPort))
            {
                GServer = new GServer(ListeningPort);
                GServer.Start();
                Thread.Sleep(120000); // 2 min
                GServer.Stop();
            }
            else
            {
                Console.WriteLine("Server port expected as first and only argument (Invalid port or no port supplied).");
            }
        }
    }
}
