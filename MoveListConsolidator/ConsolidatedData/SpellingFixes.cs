using System.Collections.Generic;
using Newtonsoft.Json;

namespace MoveListConsolidator.ConsolidatedData
{
    public class SpellingFixes
    {
        public class MoveFix {
            public string OldName;
            public string NewName;
        }
        public static SpellingFixes ParseJson(string json)
        {
            var result = JsonConvert.DeserializeObject<SpellingFixes>(json);
            return result;
        }

        public List<MoveFix> MoveFixes;
    }
}