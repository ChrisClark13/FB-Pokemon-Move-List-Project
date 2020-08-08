using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace MoveListConsolidator.RawData
{
    public class VeekunPokemon : RawDataPokemon
    {
        public List<VeekunLevelUpMove> LevelUpMoves;
        public List<RawDataMove> EggMoves;
        public List<RawDataMove> TutorMoves;
        public List<RawDataMove> MachineMoves;

        private string cachedForm = null;
        public string Form
        {
            get
            {
                if (!string.IsNullOrEmpty(cachedForm))
                    return cachedForm;

                if (Link.Contains("?form="))
                {
                    var form = Regex.Match(Link, @"\?form=(.*)$").Groups[1].Captures[0].Value;

                    var chars = form.ToCharArray();
                    chars[0] = char.ToUpperInvariant(chars[0]);
                    cachedForm = new string(chars);
                    return cachedForm;
                }

                cachedForm = "Normal";
                return cachedForm;
            }
        }
    }
}