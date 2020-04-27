using System;
using UserSDK;


namespace AppECommerce
{
    class AppECommerce
    {
        static void Main(string[] args)
        {

            RpcUser client = new RpcUser();

            User user = User.GetUser("noUser");
            if (user != null)
            {
                Console.WriteLine(user.ToString());
            }
            user = User.GetUser("lird");
            if (user != null)
            {
                Console.WriteLine(user.ToString());
            }
            user = User.GetUser("kleent");
            if (user != null)
            {
                Console.WriteLine(user.ToString());
            }
            client.Close();
        }
    }
}
