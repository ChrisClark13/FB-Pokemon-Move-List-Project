using System.Collections.Generic;

namespace MoveListConsolidator.RawData
{
    public class SerebiiMove : RawDataMove
    {
        public class FormEntry {
            public string Form;
        }

        public List<FormEntry> Forms;
    }
}