using GatewayServiceAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Pokemon;
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

        public GatewayAPIController(RabbitProducer rabbitProd)
        {
            this.rabbitProd = rabbitProd;

        }
        [HttpGet]
        public PokedexEntry Get(int id)
        {
            rabbitProd.Producer("Get", id.ToString(), new RabbitMQ.Client.ConnectionFactory() { HostName = "192.168.0.46" });
            return new() { Type = new() { "testPoke" } };
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
