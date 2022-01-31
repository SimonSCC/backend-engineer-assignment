using Models.Pokemon;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedLibary.Services;
using System;
using System.Text;
using System.Threading;

namespace GatewayServiceAPI.Services
{
    public class RabbitReceiver
    {
        public string Receiver(string QueueName, ConnectionFactory factory)
        {
            RabbitProducer rabbit = new RabbitProducer();
            string result = string.Empty;

            Console.WriteLine("Listening on queue: " + QueueName + "...");

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, e) =>
                {
                    var body = e.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Message: " + message);

                if (QueueName == "GetResponse")
                    {
                        result = message;
                    }
                };


                channel.BasicConsume(queue: QueueName, true, consumer);

                while (string.IsNullOrEmpty(result))
                {
                    Thread.Sleep(3000);
                }
                return result;
            }
        }
    }
}
