using System.Collections.Generic;
using Newtonsoft.Json;

namespace MoveListConsolidator.ConsolidatedData
{
    public class BasePokemonList
    {
        public List<BasePokemon> Pokemon;

        public static BasePokemonList ParseJson(string json)
        {
            return JsonConvert.DeserializeObject<BasePokemonList>(json);
        }
    }
}