using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedLibary.Services
{
    public class RabbitProducer
    {
        /// <summary>
        /// Produces with object
        /// </summary>
        /// <param name="QueueName"></param>
        /// <param name="obj"></param>
        /// <param name="factory"></param>
        public void Producer(string QueueName, object obj, ConnectionFactory factory)
        {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(   
                QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);


            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));

            channel.BasicPublish("", QueueName, null, body);
        }
    }
}
