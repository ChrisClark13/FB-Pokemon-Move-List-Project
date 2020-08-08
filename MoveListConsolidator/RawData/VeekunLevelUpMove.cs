using System;
using System.Linq;

namespace MoveListConsolidator.RawData
{
    public class VeekunLevelUpMove : RawDataLevelUpMove
    {
        public string Level1 = "";
        public string Level2 = "";
        public string Level3 = "";
        public string Level4 = "";
        public string Level5 = "";
        public string Level6 = "";
        public string Level7 = "";
        public string Level8 = "";
        public string Level9 = "";
        public string Level10 = "";
        public string Level11 = "";
        public string Level12 = "";
        public string Level13 = "";

        private uint? _cachedLevel;
        public override uint LevelValue
        {
            get
            {
                if (_cachedLevel.HasValue)
                    return _cachedLevel.Value;

                _cachedLevel = new string[] {Level1, Level2, Level3, Level4, Level5, Level6, Level7, Level8, Level9, Level10, Level11, Level12, Level13}
                    .Where(lvl => !string.IsNullOrEmpty(lvl))
                    .Select(lvl => {
                        uint.TryParse(lvl, out uint val);
                        return Math.Max(val, 1);
                    }).Min();

                return _cachedLevel.Value;
            }
        }
    }
}