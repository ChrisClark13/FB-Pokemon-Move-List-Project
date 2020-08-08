using System.Collections.Generic;
using Newtonsoft.Json;

using MoveListConsolidator.ConsolidatedData;

namespace MoveListConsolidator
{
    public class AlternateFormList
    {
        public class AltFormEntry
        {
            public string Name;
            public uint DexNum;
            public string DefaultForm = "Normal";
            public List<string> AltForms;
        }

        /// Forms that have the same move set, plus one additional move.
        public class AltFormWithSingleAdditionalMove
        {
            public string Name;
            public uint DexNum;
            public string DefaultForm = "Normal";
            public class FormWithMove {
                public string Form;
                public string Move;
            }
            public List<FormWithMove> Forms;
        }

        public static AlternateFormList ParseJson(string json)
        {
            return JsonConvert.DeserializeObject<AlternateFormList>(json);
        }

        public List<BasePokemon> AlolaForms;
        public List<BasePokemon> GalarianForms;
        public List<AltFormEntry> AltForms;
        // Forms that have the same move set, plus one additional move.
        // Currently I know of: Rotom and Cosplay Pikachu
        public List<AltFormWithSingleAdditionalMove> AltFormsWithSingleAdditionalMove;
    }
}