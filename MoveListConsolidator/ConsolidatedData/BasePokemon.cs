using Newtonsoft.Json;

namespace MoveListConsolidator.ConsolidatedData
{
    public class BasePokemon
    {
        [JsonProperty(Order = -5)]
        public string Name = "POKEMON NAME";
        [JsonProperty(Order = -4)]
        public uint DexNum = 0;
    }
}