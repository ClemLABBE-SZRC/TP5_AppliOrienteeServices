using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UserSDK;
using System.IO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace UserManager
{
    class UserManager
    {
        static List<User> users = null;

        public UserManager()
        {
            if (users == null)
            {
                users = new List<User>();
            }
            loadList();
        }

        private void loadList()
        {
            users = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText("users.json"));
        }

        internal static string GetUser(string username)
        {
            foreach (User user in users)
            {
                if (user.username == username)
                {
                    return JsonConvert.SerializeObject(user);
                }
            }
            return "No such user founded.";
        }
    }

    public class RpcUserManager
    {
        public static void Main()
        {
            new UserManager();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "user_queue", durable: false,
                  exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "user_queue",
                  autoAck: false, consumer: consumer);
                Console.WriteLine("[RpcUserManagerAwaiting RpcUser requests ...]");

                consumer.Received += (model, ea) =>
                {
                    string response = null;
                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        Console.WriteLine("[UserManager responding ...]");
                        response = UserManager.GetUser(message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" /!\\ " + e.Message);
                        response = null;
                    }
                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response.ToString());
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