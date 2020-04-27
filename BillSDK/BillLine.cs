using StockSDK;

namespace BillSDK
{
    public class BillLine
    {
        private Item item;
        private int quantity;
        private float total;

        public BillLine(Item item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
            this.total = item.UnitPrice * quantity;
        }

        public Item Item { get => item; }
        public int Quantity { get => quantity; }
        public float Total { get => total; }
    }
}
