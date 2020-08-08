namespace MoveListConsolidator.RawData
{
    public abstract class RawDataPokemon
    {
        public string Link;
        public string DexNum;
        public uint DexNumValue => uint.TryParse(DexNum, out uint val) ? val : 0;
    }
}