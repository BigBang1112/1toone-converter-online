using GBX.NET.Engines.Game;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class MultiBlockAddConversion : Conversion
{
    public Block? NewBlock { get; init; }
    public ElementValue<byte> XStep { get; init; }
    public ElementValue<byte> YValue { get; init; }
    public ElementValue<byte> ZStep { get; init; }

    public override void Convert(Map map)
    {
        throw new NotImplementedException();
    }
}