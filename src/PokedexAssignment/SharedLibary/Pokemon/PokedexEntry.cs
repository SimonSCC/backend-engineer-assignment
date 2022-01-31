using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Pokemon
{
    public class PokedexEntry
    {
        public int Id { get; set; }
        public Name Name { get; set; }
        public List<string> Type { get; set; }
        public PokemonBase Base { get; set; }
    }
}
