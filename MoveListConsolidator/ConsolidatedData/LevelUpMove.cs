using Newtonsoft.Json;
namespace MoveListConsolidator.ConsolidatedData
{
    public class LevelUpMove : BaseMove
    {
        [JsonProperty(Order = 5)]
        public uint Level;
    }
}