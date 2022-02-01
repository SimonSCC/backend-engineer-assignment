using Models.Pokemon;
using PokedexService.DataAccess;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedLibary.Services;
using System;
using System.Text;
using System.Threading;

namespace PokedexService.Services
{
    public class RabbitReceiver
    {
        public void Receiver(string QueueName, ConnectionFactory factory)
        {
            RabbitProducer rabbit = new RabbitProducer();

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

                if (QueueName == "Get")
                    {
                        int.TryParse(message.ToString(), out int result);
                        using (PostGreConn conn = new())
                        {
                            rabbit.Producer("GetResponse", conn.GetPokedexEntryById(result), factory);
                        }
                    }
                };


                channel.BasicConsume(queue: QueueName, true, consumer);

                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
