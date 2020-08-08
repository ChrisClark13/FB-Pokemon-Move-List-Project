using System.Collections.Generic;
using Newtonsoft.Json;

namespace MoveListConsolidator.RawData
{
    public class SerebiiPokemonList
    {
        public List<SerebiiPokemon> Pokemon;

        public static SerebiiPokemonList ParseJson(string json)
        {
            return JsonConvert.DeserializeObject<SerebiiPokemonList>(json);
        }
    }
}