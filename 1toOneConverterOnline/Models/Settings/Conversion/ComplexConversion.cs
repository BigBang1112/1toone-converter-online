using GBX.NET.Engines.Game;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class ComplexConversion : Conversion
{
    public List<Conversion>? Conversions { get; init; }

    public override void Convert(Map map)
    {
        if (Conversions is null)
        {
            return;
        }

        foreach (var conversion in Conversions)
        {
            try
            {
                conversion.Convert(map);
            }
            catch (NotImplementedException)
            {

            }
        }
    }
}
