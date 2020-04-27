using System;

namespace StockSDK
{
    public class Item
    {
        public string Name { get; }
        public float UnitPrice { get; set; }

        public Item(string name, float unitPrice)
        {
            this.Name = name;
            this.UnitPrice = unitPrice;
        }
        public override string ToString()
        {
            return $"Name: {Name}, Price: {UnitPrice}";
        }
    }
}
