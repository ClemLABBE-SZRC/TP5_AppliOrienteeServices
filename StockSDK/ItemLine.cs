using System;
using Newtonsoft.Json;

namespace StockSDK
{
    public class ItemLine
    {
        public Item Item { get; }
        public int Quantity { get; set; }
        public ItemLine(Item item, int quantity)
        {
            this.Item = item;
            this.Quantity = quantity;
        }
        public override string ToString()
        {
            return $"Item: [{Item}], Quantity: {Quantity}";
        }
    }
}
