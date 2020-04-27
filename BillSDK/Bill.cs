using UserSDK;
using StockSDK;
using System.Collections.Generic;
using RPC;
using Newtonsoft.Json;

namespace BillSDK
{
    public class Bill
    {
        private static float taxe = 0.1f;

        public User User { get; }
        public List<BillLine> BillLines { get; }
        public float TotalHt { get; }
        public float TotalTtc { get; }

        public Bill(User user, List<BillLine> billLines)
        {
            this.User = user;
            this.BillLines = billLines;
            this.TotalHt = 0;
            foreach (BillLine billLine in billLines)
            {
                this.TotalHt += billLine.Total;
            }
            this.TotalTtc = this.TotalHt * (1 + taxe);
        }

        public static Bill CreateBill(User user, List<ItemLine> lines)
        {
            Bill result = null;
            if (user != null && lines != null)
            {
                RPCClient rpcClient = new RPCClient();
                BillRequest request = new BillRequest(lines, user);
                string response = rpcClient.Call(JsonConvert.SerializeObject(request), "bill_queue");
                rpcClient.Close();
                result = JsonConvert.DeserializeObject<Bill>(response);
            }
            return result;
        }
        
    }
}
