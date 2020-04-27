using System;

namespace BillSDK
{
    public class BillLine
    {
        private Item item;
        private int quantity;
        private int total;

        public BillLine(Item item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
            this.total = item.Price * quantity;
        }

        public Item Item { get => item; }
        public int Quantity { get => quantity; }
        public int Total { get => total; }
    }
}
