using System;
using System.Collections.Generic;
using StockSDK;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StockManager
{
    class StockManager
    {
        private static string filename = "stock.json";
        private List<ItemLine> stock;
        public StockManager()
        {
            stock = new List<ItemLine>();
            stock.Add(new ItemLine(new Item("banane", 0.5f), 5));
            stock.Add(new ItemLine(new Item("kiwi", 1f), 10));
        }
        public void LoadStock(string filename)
        {
            // TODO
        }
        private ItemLine ReserveItem(int quantity, string name)
        {
            ItemLine result = null;
            ItemLine itemLine = stock.Find(item => item.Item.Name == name);
            if (itemLine != null)
            {
                if (itemLine.Quantity > quantity)
                {
                    result = new ItemLine(itemLine.Item, quantity);
                    itemLine.Quantity -= quantity;
                }
                else
                {
                    stock.Remove(itemLine);
                    result = itemLine;
                }

            }
            return result;
        }
        private void ReleaseItem(ItemLine line)
        {
            ItemLine existingItem = stock.Find(item => item.Item.Name == line.Item.Name);
            if (existingItem != null)
            {
                existingItem.Quantity += line.Quantity;
            }
            else
            {
                stock.Add(line);
            }
        }
        public static object HandleRequest(string jsonRequest, StockManager stockManager)
        {
            object result = null;
            JObject request = JObject.Parse(jsonRequest);
            if (request["Item"] != null)
            {
                ItemLine itemLine = request.ToObject<ItemLine>();
                stockManager.ReleaseItem(itemLine);
                result = true;
            }
            else
            {
                result = stockManager.ReserveItem((int)request["quantity"], (string)request["name"]);
            }
            return result;
        }
        static void Main(string[] args)
        {
            StockManager stockManager = new StockManager();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "stock_queue", durable: false,
                  exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "stock_queue",
                  autoAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += (model, ea) =>
                {
                    object response = null;

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        Console.WriteLine(" [.] Waiting: {0}", message);
                        response = HandleRequest(message, stockManager);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                    }
                    finally
                    {
                        Console.WriteLine(response);
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