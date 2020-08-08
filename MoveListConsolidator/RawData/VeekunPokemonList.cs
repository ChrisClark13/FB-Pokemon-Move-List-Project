using System.Collections.Generic;
using Newtonsoft.Json;

namespace MoveListConsolidator.RawData
{
    public class VeekunPokemonList
    {
        public static VeekunPokemonList ParseJson(string json)
        {
            return JsonConvert.DeserializeObject<VeekunPokemonList>(json);
        }

        public List<VeekunPokemon> Pokemon;
    }
}