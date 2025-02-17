﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Pokemon
{
    public class PokemonBase
    {
        public int Id { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        [JsonProperty("Sp. Attack")]
        public int SpAttack { get; set; }
        [JsonProperty("Sp. Defense")]
        public int SpDefense { get; set; }
        public int Speed { get; set; }
    }
}
