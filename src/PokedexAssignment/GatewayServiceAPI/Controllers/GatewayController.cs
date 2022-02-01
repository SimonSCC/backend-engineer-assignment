using GatewayServiceAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Pokemon;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SharedLibary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayAPIController : ControllerBase
    {
        private readonly RabbitProducer rabbitProd;
        private readonly RabbitReceiver rabbitReceiver;

        public GatewayAPIController(RabbitProducer rabbitProd, RabbitReceiver rabbitReceiver)
        {
            this.rabbitProd = rabbitProd;
            this.rabbitReceiver = rabbitReceiver;
        }
        [HttpGet]
        public PokedexEntry Get(int id)
        {
            rabbitProd.Producer("Get", id, new ConnectionFactory() { HostName = ConnectionManager.RabbitMQIpAddress });
            PokedexEntry result = JsonConvert.DeserializeObject<PokedexEntry>(rabbitReceiver.Receiver("GetResponse", new ConnectionFactory()
            { HostName = ConnectionManager.RabbitMQIpAddress }));
            return result;
        }


        [HttpPost]
        public void Post(PokedexEntry entry)
        {
            rabbitProd.Producer("Post", entry, new ConnectionFactory() { HostName = ConnectionManager.RabbitMQIpAddress });
        }

        [HttpPut]
        public void Put(PokedexEntry entry)
        {
            rabbitProd.Producer("Put", entry, new ConnectionFactory() { HostName = ConnectionManager.RabbitMQIpAddress });
        }

        [HttpDelete]
        public void Delete(int id)
        {
            rabbitProd.Producer("Delete", id, new ConnectionFactory() { HostName = ConnectionManager.RabbitMQIpAddress });
        }
    }
}
