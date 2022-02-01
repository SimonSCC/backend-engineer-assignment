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
            pr.RunProgram(Environment.GetEnvironmentVariable("VAR1"));


        }

        private void RunProgram(string variable)
        {
            rabbitProd = new RabbitProducer();

            Console.WriteLine("Starting pokedex service");
            Thread.Sleep(12000);
            standardConnectionFac = new ConnectionFactory
            {
                HostName = ConnectionManager.RabbitMQIpAddress
            };
            RabbitReceiver receiver = new();


            switch (variable)
            {
                case "get":
                    receiver.Receiver("Get", standardConnectionFac, HandleGetRequest);
                    break;
                case "delete":
                    receiver.Receiver("Delete", standardConnectionFac, HandleDeleteRequest);
                    break;
                default:
                    Console.WriteLine("No argument was provided. Please provide which queue program should handle!");
                    break;
            }
        }

        

        private void HandleDeleteRequest(string message)
        {
            int.TryParse(message.ToString(), out int result);
            using (PostGreConn conn = new())
            {
                conn.DeletePokedexEntryById(result).Wait();
            }
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
