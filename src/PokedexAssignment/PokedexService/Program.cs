using PokedexService.DataAccess;
using PokedexService.Services;
using RabbitMQ.Client;
using SharedLibary.Services;
using System;
using System.Threading;

namespace PokedexService
{
    class Program
    {
        static void Main(string[] args)
        {
            //Uncomment to insert all pokedex entries from json file
            //using (PostGreConn conn = new())
            //conn.InsertAllPokedexEntries();

            Console.WriteLine("Starting pokedex service");
            Thread.Sleep(12000);
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = ConnectionManager.RabbitMQIpAddress
            };
            RabbitReceiver receiver = new();
            receiver.Receiver("Get", factory);
        }
    }
}
