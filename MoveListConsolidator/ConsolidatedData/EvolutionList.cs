using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MoveListConsolidator.ConsolidatedData
{
    public class EvolutionList
    {
        public class Evolution
        {
            public string Name;
            public uint DexNum;
            public string Form;

            public string EvolvesIntoName;
            public uint EvolvesIntoDexNum;
            public string EvolvesIntoForm;
        }

        public static EvolutionList ParseJson(string json)
        {
            var result = JsonConvert.DeserializeObject<EvolutionList>(json);
            AllNonTransferableMoves.AddRange(result.NonTransferableMoves.Where(m => !AllNonTransferableMoves.Contains(m)));
            return result;
        }

        public List<string> NonTransferableMoves;
        public static List<string> AllNonTransferableMoves = new List<string>();
        public List<Evolution> Evolutions;
    }
}