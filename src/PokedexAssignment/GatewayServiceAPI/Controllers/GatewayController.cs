using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Pokemon;
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
        public GatewayAPIController()
        {

        }
        [HttpGet]
        public PokedexEntry Get(int id)
        {
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
