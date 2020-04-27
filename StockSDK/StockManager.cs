using System;
using RPC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StockSDK
{
    public class StockManager
    {
        public ItemLine ReserveItem(int quantity, string name)
        {
            RPCClient rpcClient = new RPCClient();
            JObject message = new JObject(){
                {"quantity", quantity},
                {"name", name}
            };
            string response = rpcClient.Call(message.ToString(), "stock_queue");
            rpcClient.Close();
            return JsonConvert.DeserializeObject<ItemLine>(response);
        }
        public bool ReleaseItem(ItemLine line)
        {
            RPCClient rpcClient = new RPCClient();
            string response = rpcClient.Call(JsonConvert.SerializeObject(line), "stock_queue");
            rpcClient.Close();
            return Boolean.Parse(response);
        }
    }
}
