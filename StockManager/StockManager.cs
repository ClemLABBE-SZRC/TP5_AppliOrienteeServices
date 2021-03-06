﻿using System;
using System.Collections.Generic;
using StockSDK;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace StockManager
{
    class StockManager
    {
        private static string filename = @"stock.json";
        private List<ItemLine> stock;
        public StockManager()
        {
            stock = new List<ItemLine>();
            LoadStock();
        }
        public void LoadStock()
        {
            JArray products = JArray.Parse(File.ReadAllText(filename));
            foreach (JObject prod in products)
            {
                try
                {
                    stock.Add(new ItemLine(
                        new Item((string)prod["name"], (float)prod["price"]),
                        (int)prod["quantity"])
                    );
                }
                catch (Exception)
                {
                    Console.WriteLine("Fail to load line: {}", prod.ToString());
                }
            }
        }
        private ItemLine ReserveItem(int quantity, string name)
        {
            ItemLine result = null;
            ItemLine itemLine = stock.Find(item => item.Item.Name == name);
            if (itemLine != null && quantity > 0)
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
        private bool ReleaseItem(ItemLine line)
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
            return true;
        }
        public object HandleRequest(string jsonRequest)
        {
            object result = null;
            JObject request = JObject.Parse(jsonRequest);
            if (request["Item"] != null)
            {
                ItemLine itemLine = request.ToObject<ItemLine>();
                bool release = ReleaseItem(itemLine);
                if (release)
                {
                    Console.WriteLine(" [.] Release product: {0}", itemLine);
                }
                result = release;
            }
            else
            {
                result = ReserveItem((int)request["quantity"], (string)request["name"]);
                if (result != null)
                {
                    Console.WriteLine(" [.] Reserve product: {0}", result);
                }
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
                        response = stockManager.HandleRequest(message);
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