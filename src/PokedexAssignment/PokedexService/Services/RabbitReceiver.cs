using Models.Pokemon;
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

                // Async method, that runs the queue to the database, that needs to be inside a try except.
                // If the request can not be made to the database, then wait 5 seconds and then add the rabbitmq request again.

                if (QueueName == "Get")
                    {
                        int.TryParse(message.ToString(), out int result);
                        PokedexEntry entry = new PokedexEntry() { Name = new() { English = "Pokemon1" }, Id = result};
                        rabbit.Producer("GetResponse", entry, factory);
                    }
                };


                channel.BasicConsume(queue: QueueName, true, consumer);

                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
