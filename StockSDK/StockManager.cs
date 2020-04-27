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
            ItemLine result = null;
            if (quantity > 0)
            {
                RPCClient rpcClient = new RPCClient();
                JObject message = new JObject(){
                {"quantity", quantity},
                {"name", name}
                };
                string response = rpcClient.Call(message.ToString(), "stock_queue");
                rpcClient.Close();
                result = JsonConvert.DeserializeObject<ItemLine>(response);
            }
            return result;
        }
        public bool ReleaseItem(ItemLine line)
        {
            bool result = false;
            if (line != null)
            {
                RPCClient rpcClient = new RPCClient();
                string response = rpcClient.Call(JsonConvert.SerializeObject(line), "stock_queue");
                rpcClient.Close();
                result = (Boolean.TryParse(response, out bool parsingRes) && parsingRes);
            }
            return result;
        }
    }
}
