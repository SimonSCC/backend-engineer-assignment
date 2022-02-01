using Models.Pokemon;
using Newtonsoft.Json;
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
            ////Uncomment to insert all pokedex entries from json file
            //using (PostGreConn conn = new())
            //    conn.InsertAllPokedexEntries().Wait();

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
                case "post":
                    receiver.Receiver("Post", standardConnectionFac, HandlePostRequest);
                    break;
                case "put":
                    receiver.Receiver("Put", standardConnectionFac, HandlePutRequest);
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

        public void HandlePostRequest(string message)
        {
            PokedexEntry result = JsonConvert.DeserializeObject<PokedexEntry>(message);
            Console.WriteLine($"Posting: \n {result}");
            using (PostGreConn conn = new())
            {
                conn.PostNewPokedex(result).Wait();
            }
        }

        public void HandlePutRequest(string message)
        {
            PokedexEntry result = JsonConvert.DeserializeObject<PokedexEntry>(message);
            Console.WriteLine($"Updating record to: \n{result}");
            using (PostGreConn conn = new())
            {
                conn.UpdatePokedexEntry(result).Wait();
            }
        }
    }
}
