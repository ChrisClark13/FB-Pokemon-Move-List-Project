using System.Collections.Generic;
using Newtonsoft.Json;

namespace MoveListConsolidator.ConsolidatedData
{
    public class SpellingFixes
    {
        public class SpellingFix {
            public string OldName;
            public string NewName;
            public uint DexNum = 0;
        }
        public static SpellingFixes ParseJson(string json)
        {
            var result = JsonConvert.DeserializeObject<SpellingFixes>(json);
            return result;
        }

        public List<SpellingFix> MoveFixes;
        public List<SpellingFix> FormFixes;
    }
}