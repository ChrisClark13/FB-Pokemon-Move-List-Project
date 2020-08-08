namespace MoveListConsolidator.RawData
{
    public class SerebiiLevelUpMove : RawDataLevelUpMove
    {
        public string Level;
        public override uint LevelValue => uint.TryParse(Level, out uint val) ? val : 1;
    }
}