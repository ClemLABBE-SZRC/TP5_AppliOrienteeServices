using System;
using System.Collections.Generic;
using UserSDK;
using StockSDK;

namespace AppECommerce
{
    class AppECommerce
    {
        static void Main(string[] args)
        {
            User user = AuthenticateUser();
            List<ItemLine> cart = ManageShoppingCart(user);
            // Bill bill = Bill.CreateBill(user, cart);
        }
        private static User AuthenticateUser()
        {
            User result = null;
            while (result == null)
            {
                Console.Write("Username: ");
                string input = Console.ReadLine();
                result = new User(); //User.GetUser(input);
                if (result == null)
                {
                    Console.WriteLine("This user doesn't exist");
                }
            }
            return result;
        }
        private static List<ItemLine> ManageShoppingCart(User user)
        {
            List<ItemLine> result = new List<ItemLine>();
            bool finishing = false;
            while (!finishing)
            {
                Console.Write("1- Reserve Item\n2- Release Item\n3- See cart\n4- Pay bill\nYour choice: ");
                switch (Console.ReadLine())
                {
                    case "1":
                        ReserveItem(result);
                        break;
                    case "2":
                        if (result.Count > 0)
                        {
                            ReleaseItem(result);
                        }
                        else
                        {
                            Console.WriteLine("The cart is empty");
                        }
                        break;
                    case "3":
                        foreach (ItemLine item in result)
                        {
                            Console.WriteLine(item);
                        }
                        break;
                    case "4":
                        finishing = true;
                        break;
                }
                Console.WriteLine();
            }
            return result;
        }
        private static void ReserveItem(List<ItemLine> cart)
        {
            Console.Write("Item name: ");
            string name = Console.ReadLine();
            int quantity = -1;
            do
            {
                Console.Write("Item quantity: ");
            }
            while (!Int32.TryParse(Console.ReadLine(), out quantity) && quantity < 1);
            ItemLine reservedItem = (new StockManager()).ReserveItem(quantity, name);
            if (reservedItem != null)
            {
                cart.Add(reservedItem);
            }
        }

        private static void ReleaseItem(List<ItemLine> cart)
        {
            ItemLine item = null;
            while (item == null)
            {
                Console.Write("Item name: ");
                string name = Console.ReadLine();
                item = cart.Find(item => item.Item.Name == name);
            }
            if ((new StockManager()).ReleaseItem(item))
            {
                cart.Remove(item);
            }
        }
    }
}