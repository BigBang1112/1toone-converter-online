namespace _1toOneConverterOnline.Models.Settings.Conversion;

sealed class MinimalItem
{
    public Ident? Meta { get; init; }
    public Vec3 Rot { get; init; }
    public Int3 BlockCoords { get; init; }
    public Vec3 ItemCoords { get; init; }
}