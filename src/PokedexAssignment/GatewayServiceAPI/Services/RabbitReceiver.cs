using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayServiceAPI.Services
{
    public class RabbitReceiver
    {
        //public void Receiver(string QueueName, ConnectionFactory factory)
        //{
        //    RabbitProducer rabbit = new RabbitProducer();

        //    Console.WriteLine("Listening on queue: " + QueueName + "...");

        //    using (var connection = factory.CreateConnection())
        //    using (var channel = connection.CreateModel())
        //    {
        //        channel.QueueDeclare(
        //            queue: QueueName,
        //            durable: true,
        //            exclusive: false,
        //            autoDelete: false,
        //            arguments: null);

        //        var consumer = new EventingBasicConsumer(channel);
        //        consumer.Received += (sender, e) =>
        //        {
        //            var body = e.Body.ToArray();
        //            var message = Encoding.UTF8.GetString(body);
        //            Console.WriteLine(message);

        //            // Async method, that runs the queue to the database, that needs to be inside a try except.
        //            // If the request can not be made to the database, then wait 5 seconds and then add the rabbitmq request again.

        //            if (QueueName == "Get")
        //            {
        //                rabbit.Producer("GetResponse", , factory); //Adds the unsuccesful insertion's object back into rabbitmq

                       
        //            }
        //            else if (QueueName == "Payment" || QueueName == "Invoice" || QueueName == "OrderGenerated" || QueueName == "OrderReady")
        //            {
        //                int productOrderId = JsonSerializer.Deserialize<int>(message);
        //                if (!dataAccess.ChangeOrderStatus(productOrderId, QueueName))
        //                {
        //                    rabbit.Producer(QueueName, productOrderId, factory); //Adds the unsuccesful insertion's object back into rabbitmq
        //                    Thread.Sleep(10000);
        //                }
        //                else
        //                {
        //                    rabbit.Producer("EmailSend", productOrderId, factory);
        //                }
        //            }
        //            else if (QueueName == "EmailSend")
        //            {
        //                int productOrderId = JsonSerializer.Deserialize<int>(message);

        //                emailHandler.CheckOrderComplete(productOrderId);
        //            }
        //        };


        //        channel.BasicConsume(queue: QueueName, true, consumer);

        //        Thread.Sleep(Timeout.Infinite);

        //    }
        //}
    }
}
