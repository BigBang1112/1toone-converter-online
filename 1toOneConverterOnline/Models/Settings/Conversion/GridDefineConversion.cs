namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class GridDefineConversion : Conversion
{
    public Vec3 GridSize { get; init; }
    public Vec3 GridOffset { get; init; }

    public override void Convert(Map map)
    {
        map.GridSize = new GBX.NET.Vec3(GridSize.X, GridSize.Y, GridSize.Z);
        map.GridOffset = new GBX.NET.Vec3(GridOffset.X, GridOffset.Y, GridOffset.Z);
    }
}