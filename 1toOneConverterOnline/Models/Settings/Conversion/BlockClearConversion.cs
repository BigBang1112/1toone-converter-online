namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class BlockClearConversion : Conversion
{
    public override void Convert(Map map)
    {
        map.Challenge.ClearBlocks();
    }
}