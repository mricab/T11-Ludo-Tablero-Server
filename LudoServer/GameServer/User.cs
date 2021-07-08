using System;
namespace GameServer
{
    public class User
    {
        // Properties
        public int connection { get; set; }
        public string token { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        // Constructor
        public User(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
