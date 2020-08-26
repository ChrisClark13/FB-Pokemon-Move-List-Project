using System.Collections.Generic;
using Newtonsoft.Json;

namespace MoveListConsolidator.ConsolidatedData
{
    public class Move : BaseMove
    {
        public class FormSource
        {
            public string Form;
            public List<string> Sources = new List<string>();
        }

        public List<string> Forms = new List<string>();

        #if !DEBUG
        [JsonIgnore]
        #endif
        public List<FormSource> FormSources = new List<FormSource>();
    }
}