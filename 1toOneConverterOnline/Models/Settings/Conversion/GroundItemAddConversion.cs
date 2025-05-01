using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class GroundItemAddConversion : Conversion
{
    public Id? Collection { get; init; }
    public Id? DefaultAuthor { get; init; }

    [XmlElement("BlockIgnoreFlag", IsNullable = false)]
    public FlagName[]? BlockIgnoreFlags { get; init; }

    public FlagName? HeightFlag { get; init; }
    public ItemData? GroundItem { get; init; }

    public override void Convert(Map map)
    {
        /*var x1 = map.Challenge.Size.X;
        var z1 = map.Challenge.Size.Z;
        var flagArray = new bool[64, 64];
        foreach (var blockIgnoreFlag in BlockIgnoreFlags ?? [])
        {
            for (int x2 = 0; x2 < 64; ++x2)
            {
                for (int z2 = 0; z2 < 64; ++z2)
                {
                    if (file.TestFlag(blockIgnoreFlag.Name, x2, z2))
                        flagArray[x2, z2] = true;
                }
            }
        }
        for (byte x3 = 0; (uint)x3 < x1; ++x3)
        {
            for (byte z3 = 0; (uint)z3 < z1; ++z3)
            {
                if (!flagArray[(int)x3, (int)z3])
                {
                    byte num = 0;
                    if (this.HeightFlag != null)
                        num = file.GetFlag(this.HeightFlag.Name, x3, z3);
                    this.GroundItem.GetItemInfo(new Identifier((string)null, 0U, false, (string)null)).PlaceAt(file, (x3, num, z3), (byte)0, chunk2, this.Collection, this.DefaultAuthor.Content);
                }
            }
        }*/

        var occupiedGroundCoords = map.Challenge.Blocks!
            .Where(block => block.IsGround)
            .Select(block => block.Coord with { Y = 0 })
            .ToHashSet();

        if (GroundItem?.ItemName is not null)
        {
            for (var x = 0; x < map.Challenge.Size.X; x++)
            {
                for (var z = 0; z < map.Challenge.Size.Z; z++)
                {
                    if (occupiedGroundCoords.Contains(new(x, 0, z)))
                    {
                        continue;
                    }

                    var ident = new GBX.NET.Ident(GroundItem.ItemName,
                        Collection ?? throw new Exception($"{nameof(BlockToItemConversion)}: Collection missing."),
                        GroundItem.ItemAuthor ?? DefaultAuthor ?? "");

                    var absolutePosition = new GBX.NET.Vec3(x, map.BaseHeight, z) * map.GridSize + map.GridOffset + (0, GroundItem.SmallYOffset, 0);
                    var pitchYawRoll = new GBX.NET.Vec3(GroundItem.RotOffset % 4 * -MathF.PI / 2, 0, 0);

                    map.Challenge.PlaceAnchoredObject(ident, absolutePosition, pitchYawRoll);
                }
            }
        }
    }
}