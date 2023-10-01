using GBX.NET.Engines.Game;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

sealed class BlockClearConversion : Conversion
{
    public override void Convert(Map map)
    {
        map.Challenge.ClearBlocks();
    }
}