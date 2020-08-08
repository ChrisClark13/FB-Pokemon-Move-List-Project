using System.Collections.Generic;

namespace MoveListConsolidator.RawData
{
    public class SerebiiPokemon : RawDataPokemon
    {
        public class AltForm {
            public string Form;
            public List<SerebiiLevelUpMove> LevelUpMoves;
        }
        public List<SerebiiLevelUpMove> LevelUpMoves;
        public List<SerebiiMove> EggMoves;
        public List<SerebiiMove> TutorMoves;
        public List<SerebiiMove> MachineMoves;

        //Alt form data
        public List<SerebiiLevelUpMove> AlolanFormLevelUpMoves;
        public List<SerebiiLevelUpMove> GalarianFormLevelUpMoves;
        public List<AltForm> AltForms;
    }
}