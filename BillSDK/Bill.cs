using System;

namespace BillSDK
{
    public class Bill
    {
        private static float taxe = 0.1;

        private User user;
        private List<BillLine> billLines;
        private int totalHt;
        private int totalTtc;

        public Bill(User user, List<BillLine> billLines)
        {
            this.user = user;
            this.billLines = billLines;
            this.totalHt = 0;
            foreach(BillLine billLine in billLines)
            {
                this.totalHt += billLine.Total;
            }
            this.totalTtc = this.totalHt * (1 + taxe);
        }

        public static Bill CreateBill(User user, List<ItemLine> lines)
        {
            List<BillLine> billLines = new List<BillLine>();
            foreach(ItemLine line in lines)
            {
                billLines.Add(new BillLine(line.Item, line.Quantity));
            }
            return new Bill(user, billLines);
        }
    }
}
