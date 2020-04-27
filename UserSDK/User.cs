using System;
using Newtonsoft.Json;
using RPC;

namespace UserSDK
{
    public class User
    {
        public string nom { get; set; } 
        public string prenom { get; set; }
        public string mail { get; set; }
        public string username { get; set; }

        public static User GetUser(string username)
        {

            var rpcClient = new RPCClient();
            var response = rpcClient.Call(username, "user_queue");

            rpcClient.Close();

            try
            {
                return JsonConvert.DeserializeObject<User>(response);
            } catch (Exception)
            {
                return null;
            }
        }

        override
        public string ToString()
        {
            string desc = "";
            desc += "nom: " + nom;
            desc += "\tprenom: " + prenom;
            desc += "\tmail: " + mail;
            desc += "\tpseudo: " + username;
            return desc;
        }
    }
}
    