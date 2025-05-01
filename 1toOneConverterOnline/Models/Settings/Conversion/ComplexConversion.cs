namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class ComplexConversion : Conversion
{
    public List<Conversion> Conversions { get; init; } = [];

    public override void Convert(Map map)
    {
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
