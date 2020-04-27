using System;
using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;


namespace UserSDK
{
    public class RpcUser
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueue;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties props;

        public RpcUser()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueue = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueue;

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body.ToArray());
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };
        }

        public string Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: "user_queue",
                basicProperties: props,
                body: messageBytes);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueue,
                autoAck: true);
            Console.WriteLine("[RpcUser requesting ...]");
            return respQueue.Take();
        }
        
        public void Close()
        {
            connection.Close();
        }
    }



    public class User
    {
        public string nom { get; set; } 
        public string prenom { get; set; }
        public string mail { get; set; }
        public string username { get; set; }

        public static User GetUser(string username)
        {

            var rpcUser = new RpcUser();
            var response = rpcUser.Call(username);

            rpcUser.Close();

            try
            {
                return JsonConvert.DeserializeObject<User>(response);
            } catch (Exception)
            {
                Console.WriteLine(response);
                return null;
            }
        }

        override
        public string ToString()
        {
            string desc = "";
            desc += "nom: " + nom;
            desc += "\tprenom: " + prenom;
            desc += "\tmail: " + mail;
            desc += "\tpseudo: " + username;
            return desc;
        }
    }
}
    