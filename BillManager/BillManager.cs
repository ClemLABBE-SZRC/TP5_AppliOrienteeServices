using System;
using Newtonsoft.Json;
using BillSDK;
using StockSDK;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;

namespace BillManager
{
    class BillManager
    {
        public Bill HandleRequest(string jsonRequest)
        {
            BillRequest billRequest = JsonConvert.DeserializeObject<BillRequest>(jsonRequest);
            List<BillLine> billLines = new List<BillLine>();
            foreach (ItemLine line in billRequest.ItemLines)
            {
                billLines.Add(new BillLine(line.Item, line.Quantity));
            }
            return new Bill(billRequest.User, billLines);
        }

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            BillManager billManager = new BillManager();
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "bill_queue", durable: false,
                  exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "bill_queue",
                  autoAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += (model, ea) =>
                {
                    Bill response = null;

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        response = billManager.HandleRequest(message);
                        Console.WriteLine($"[.] Create bill. Total HT: {response.TotalHt} | Total TTC: {response.TotalTtc}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                    }
                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                          basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                          multiple: false);
                    }
                };

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
