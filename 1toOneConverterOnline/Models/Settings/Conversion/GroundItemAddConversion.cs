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
        if (GroundItem?.ItemName is not null)
        {
            for (var x = 0; x < map.Challenge.Size.X; x++)
            {
                for (var z = 0; z < map.Challenge.Size.Z; z++)
                {
                    if (map.CoveredCoords is null || map.CoveredCoords.Contains(new(x, 0, z)))
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