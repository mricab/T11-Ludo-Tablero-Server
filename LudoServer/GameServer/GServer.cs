using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using server;
using LudoProtocol;

namespace GameServer
{
    public class GServer : IServer
    {
        // Constants
        private const string usersFile = "./users.txt";
        private const string boardsAddress = "LudoServer.GameServer.Resources.Boards.";

        // Properties
        private static Server TServer;
        private Dictionary<string, User> Users;
        private Dictionary<int, User> UsersConnected;

        // Constructor
        public GServer(int ListeningPort)
        {
            TServer = new Server(ListeningPort, typeof(LPackage));
            TServer.RegisterObserver(this);
            Users = new Dictionary<string, User>();
            UsersConnected = new Dictionary<int, User>();
            CreateUsersFile();          
        }

        // Main Methods
        public void Start()
        {
            LoadUsers();
            TServer.Start();
        }

        public void Stop()
        {
            SaveUsers();
            TServer.Stop();
        }

        // IServer
        public void OnMessageReceived(ServerMessageEvent e)
        {
            int connection = e.Data.Id;
            LPackage lPackage = (LPackage)e.Data.Obj;
            Console.WriteLine("(GServer)\tClient#{0}: Message Received [{1}]", e.Data.Id, lPackage.ToString());
            AnalizeRequest(connection, lPackage);
        }

        // Actions responses
        private void AnalizeRequest(int connection, LPackage lPackage)
        {
            string actionName = LProtocol.ActionName(lPackage.action);
            switch (actionName)
            {
                case "login":       Login(connection, lPackage.contents);   break;
                case "logout":      Logout(connection);                     break;
                case "register":    Register(connection, lPackage.contents);break;
                case "get board":   GetBoard(connection, lPackage.contents);break;
                default:            InvalidRequest(connection);             break;
            }
        }

        public void InvalidRequest(int connection)
        {
            TServer.Send(connection, LProtocol.GetPackage("unknown request"));
        }

        public void Login(int connection, string[] contents)
        {
            User user;
            if (Users.TryGetValue(contents[0], out user))
            {
                if (user.password == contents[1])
                {
                    try
                    {
                        // Successful Authentication
                        user.connection = connection;
                        user.token = GetToken();
                        UsersConnected.Add(user.connection, user);
                        TServer.Send(user.connection, LProtocol.GetPackage("login successful", new string[] { user.token }));
                    }
                    catch (ArgumentException e) // Duplicate Key
                    {
                        // Duplicate login
                        TServer.Send(connection, LProtocol.GetPackage("duplicate login"));
                    }
                }
                else
                {
                    // Password incorrect
                    TServer.Send(connection, LProtocol.GetPackage("login failure"));
                }              
            }
            else
            {
                // User doesn't exists
                TServer.Send(connection, LProtocol.GetPackage("login failure"));
            }
        }

        public void Logout(int connection)
        {
            if (UsersConnected.Remove(connection))
            {
                TServer.Send(connection, LProtocol.GetPackage("logout successful"));
            }
            else
            {
                // Connection not found
                TServer.Send(connection, LProtocol.GetPackage("logout failure"));
            }
        }

        public void Register(int connection, string[] contents)
        {            
            try
            {
                User user = new User(contents[0], contents[1]);
                Users.Add(user.username, user);
                user.connection = connection;
                user.token = GetToken();
                UsersConnected.Add(connection, user);
                TServer.Send(user.connection, LProtocol.GetPackage("register successful", new string[] { user.token }));
            }
            catch (ArgumentException e) // Duplicate Key
            {
                // Duplicate user
                TServer.Send(connection, LProtocol.GetPackage("register failure", new string[] { "Username taken." }));
            }
        }

        public void GetBoard(int connection, string[] contents)
        {
            
            switch (contents[0])
            {
                case "A":   TServer.Send(connection, LProtocol.GetPackage("board", LoadBoard("A"))); break;
                case "B":   TServer.Send(connection, LProtocol.GetPackage("board", LoadBoard("B"))); break;
                default:    InvalidRequest(connection); break;
            }
        }

        // Load Board
        private string[] LoadBoard(string boardType)
        {
            // https://stackoverflow.com/questions/11996803/c-sharp-where-to-place-txt-files
            string[] data; char[] separators = new char[] { ' ', '\t', '\n'};
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(boardsAddress+boardType+".txt"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    data = reader.ReadToEnd().Split(separators);                 
                }
            }
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = data[i].Trim();
            }
            return data;
        }

        // Users Methods
        private void CreateUsersFile()
        {
            if (!File.Exists(usersFile))
            {
                File.Create(usersFile).Close();
                GenerateSampleUsers(usersFile);
            }
        }
        public void GenerateSampleUsers(string usersFile)
        {
            if (File.Exists(usersFile))
            {
                StreamWriter writer = new StreamWriter(usersFile);
                String[] names = new String[] { "ana", "jaime", "dani", "vico" };
                foreach (var name in names)
                {
                    writer.WriteLine(name+"|"+"1234");
                }
                writer.Close();
            }
        }

        private void LoadUsers()
        {
            if (File.Exists(usersFile))
            {
                StreamReader stream = new StreamReader(usersFile);
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    int sepIdx = line.IndexOf("|");
                    string name = line.Substring(0, sepIdx);
                    string pass = line.Substring(sepIdx + 1, line.Length-sepIdx-1);
                    //Console.WriteLine(name + "(" + name.Length + ")" + " " + pass + "(" + pass.Length + ")");
                    Users.Add(name, new User(name, pass));
                }
                stream.Close();
            }
            else
            {
                throw new Exception("Users data file not found.");
            }
        }

        private void SaveUsers()
        {
            File.Create(usersFile).Close();
            StreamWriter writer = new StreamWriter(usersFile);
            foreach (var item in Users)
            {
                writer.WriteLine(item.Value.username + "|" + item.Value.password);
            }
            writer.Close();
        }

        // Helpher Methods
        private string GetToken()
        {
            Random r = new Random();
            return r.Next().ToString();
        }
    }
}
