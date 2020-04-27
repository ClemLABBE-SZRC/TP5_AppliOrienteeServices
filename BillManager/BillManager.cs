using System;
using Newtonsoft.Json;

namespace BillManager
{
    class BillManager
    {
        public Bill HandleRequest(string jsonRequest)
        {
            List<BillRequest> billRequest = JsonConvert.DeserializeObject<List<BillRequest>>(jsonRequest);
            return Bill.CreateBill(billRequest.User, billRequest.ItemLines);
        }

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            BillManager billManager = new BillManager();
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) => {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Bill bill = billManager.HandleRequest(message);
                    Console.WriteLine("Total HT: " + bill.TotalHt + " | Total TTC:" + bill.TotalTtc);
                };
                channel.BasicConsume(queue: "task_queue", autoAck: true, consumer: consumer);
            }   
        }
    }
}
