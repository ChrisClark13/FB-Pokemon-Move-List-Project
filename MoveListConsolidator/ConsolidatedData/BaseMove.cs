using Newtonsoft.Json;

namespace MoveListConsolidator.ConsolidatedData
{
    public class BaseMove
    {
        [JsonProperty(Order = -5)]
        public string Name;
    }
}