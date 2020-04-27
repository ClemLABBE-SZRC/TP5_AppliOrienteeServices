using System;
using System.Collections.Generic;
using UserSDK;
using StockSDK;

namespace AppECommerce
{
    class AppECommerce
    {
        private List<ItemLine> cart = new List<ItemLine>();
        private User user;
        private void AuthenticateUser()
        {
            while (user == null)
            {
                Console.Write("Username: ");
                string input = Console.ReadLine();
                user = User.GetUser(input);
                if (user == null)
                {
                    Console.WriteLine("No such user founded!");
                }
            }
            Console.WriteLine($"Welcome {user.prenom}, {user.nom}\n");
        }
        private void ManageShoppingCart()
        {
            bool finishing = false;
            while (!finishing)
            {
                Console.Write("1- Reserve Item\n2- Release Item\n3- See cart\n4- Pay bill\nYour choice: ");
                switch (Console.ReadLine())
                {
                    case "1":
                        ReserveItem();
                        break;
                    case "2":
                        ReleaseItem();
                        break;
                    case "3":
                        ShowCart();
                        break;
                    case "4":
                        finishing = true;
                        break;
                }
                Console.WriteLine();
            }
        }
        private void ReserveItem()
        {
            Console.Write("Product name: ");
            string name = Console.ReadLine();
            int quantity = -1;
            do
            {
                Console.Write("Quantity: ");
            }
            while (!Int32.TryParse(Console.ReadLine(), out quantity) && quantity < 1);
            ItemLine reservedItem = (new StockManager()).ReserveItem(quantity, name);
            if (reservedItem != null)
            {
                cart.Add(reservedItem);
                if (reservedItem.Quantity != quantity)
                {
                    Console.WriteLine($"Product {name} add to the cart ({reservedItem.Quantity} pieces instead of {quantity})");
                }
                else
                {
                    Console.WriteLine($"Product {name} add to the cart");
                }
            }
            else
            {
                Console.WriteLine($"Product {name} could not be found");
            }
        }
        private void ShowCart()
        {
            if (cart.Count > 0)
            {
                cart.ForEach(Console.WriteLine);
            }
            else
            {
                Console.WriteLine("The cart is empty");
            }
        }

        private void ReleaseItem()
        {
            if (cart.Count > 0)
            {
                ItemLine item = null;
                while (item == null)
                {
                    Console.Write("Product name: ");
                    string name = Console.ReadLine();
                    item = cart.Find(item => item.Item.Name == name);
                }
                if ((new StockManager()).ReleaseItem(item))
                {
                    cart.Remove(item);
                }
            }
            else
            {
                Console.WriteLine("The cart is empty");
            }
        }
        private void PayCart()
        {
            // Bill bill = Bill.CreateBill(user, cart);
        }
        static void Main(string[] args)
        {
            AppECommerce appClient = new AppECommerce();
            appClient.AuthenticateUser();
            appClient.ManageShoppingCart();
            appClient.PayCart();
        }
    }
}