using GatewayServiceAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Pokemon;
using Newtonsoft.Json;
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
            rabbitProd.Producer("Get", id, new RabbitMQ.Client.ConnectionFactory() { HostName = ConnectionManager.RabbitMQIpAddress });
            PokedexEntry result = JsonConvert.DeserializeObject<PokedexEntry>(rabbitReceiver.Receiver("GetResponse", new RabbitMQ.Client.ConnectionFactory()
            { HostName = ConnectionManager.RabbitMQIpAddress }));
            return result;
        }


        [HttpPost]
        public bool Post(PokedexEntry entry)
        {
            return false;
        }

        [HttpPut]
        public bool Put(PokedexEntry entry)
        {
            return true;
        }

        [HttpDelete]
        public bool Delete(int id)
        {
            return true;
        }
    }
}
