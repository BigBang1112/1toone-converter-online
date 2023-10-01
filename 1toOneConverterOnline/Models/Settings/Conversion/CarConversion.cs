namespace _1toOneConverterOnline.Models.Settings.Conversion;

sealed class CarConversion : Conversion
{
    public Ident? CarMeta { get; init; }

    public override void Convert(Map map)
    {
        map.Challenge.PlayerModel = CarMeta ?? new Ident();
    }
}