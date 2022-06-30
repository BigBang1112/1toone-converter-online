using GBX.NET.Engines.Game;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class BlockClearConversion : Conversion
{
    public override void Convert(Map map)
    {
        map.Challenge.ClearBlocks();
    }
}