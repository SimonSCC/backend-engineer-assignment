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
        private RabbitProducer rabbitProd;
        private ConnectionFactory standardConnectionFac;
        static void Main(string[] args)
        {
            //Uncomment to insert all pokedex entries from json file
            //using (PostGreConn conn = new())
            //conn.InsertAllPokedexEntries();
            Program pr = new();
            pr.RunProgram();


        }

        private void RunProgram()
        {
            rabbitProd = new RabbitProducer();

            Console.WriteLine("Starting pokedex service");
            Thread.Sleep(12000);
            standardConnectionFac = new ConnectionFactory
            {
                HostName = ConnectionManager.RabbitMQIpAddress
            };
            RabbitReceiver receiver = new();
            receiver.Receiver("Get", standardConnectionFac, HandleGetRequest);
            receiver.Receiver("Delete", standardConnectionFac, HandleDeleteRequest);
        }

        

        private void HandleDeleteRequest(string message)
        {
            
        }

        public void HandleGetRequest(string message)
        {
            int.TryParse(message.ToString(), out int result);
            using (PostGreConn conn = new())
            {
                rabbitProd.Producer("GetResponse", conn.GetPokedexEntryById(result), standardConnectionFac);
            }
        }
    }
}
