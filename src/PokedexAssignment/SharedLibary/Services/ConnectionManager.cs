using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibary.Services
{
    public static class ConnectionManager
    {
        public static string RabbitMQIpAddress = "192.168.0.46";
        public static string PostgreSQLConnString = $"Server=192.168.0.35;Port=5432;Database=PokeDex;User Id=postgres;password=discotek;";
    }

}