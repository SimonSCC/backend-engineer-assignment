using PokedexService.Services;
using RabbitMQ.Client;
using System;
using System.Threading;

namespace PokedexService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting pokedex service");
            Thread.Sleep(12000);
            ConnectionFactory factory = new ConnectionFactory
            {
                //Uri = new Uri("amqp://guest:guest@localhost:15672"), //amqp is the protocol hosted on port 5672 in docker. like website using https protocol https://
                HostName = "192.168.0.46"

                //Uri = new Uri("amqp://guest:guest@rabbitmq:15672"),
            };
            RabbitReceiver receiver = new();
            receiver.Receiver("Get", factory);
        }
    }
}
