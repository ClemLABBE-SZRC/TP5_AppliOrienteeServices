using System;
using System.Text.Json;

namespace BillManager
{
    class BillManager
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) => {

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                   
                
                    Console.WriteLine(" [x] Done");
                };
                channel.BasicConsume(queue: "task_queue", autoAck: true, consumer: consumer);
            }   
        }
    }
}
