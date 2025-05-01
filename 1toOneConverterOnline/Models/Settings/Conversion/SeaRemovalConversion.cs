using GBX.NET.Engines.Game;
using GBX.NET;
using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class SeaRemovalConversion : Conversion
{
    [XmlElement]
    public ElementValue<byte> XStep { get; init; }
    [XmlElement]
    public ElementValue<byte> YValue { get; init; }
    [XmlElement]
    public ElementValue<byte> ZStep { get; init; }

    public override void Convert(Map map)
    {
        map.Challenge.Chunks.Create<CGameCtnChallenge.Chunk03043043>();
        map.Challenge.Chunks.Create<CGameCtnChallenge.Chunk03043048>();

        if (map.Challenge.Blocks is null)
        {
            return;
        }

        var noSea = map.Challenge.Blocks
            .Where(block => block.Name is "BayHarbor" or "BayEsplanade")
            .Select(x => (x.Coord.X, x.Coord.Z))
            .ToHashSet();

        map.Challenge.BakedBlocks = [];
        map.Challenge.ZoneGenealogy = [];

        for (var x = 0; x < map.Challenge.Size.X; x += XStep.Value)
        {
            for (int z = 0; z < map.Challenge.Size.Z; z += ZStep.Value)
            {
                map.Challenge.BakedBlocks.Add(new CGameCtnBlock
                {
                    Name = "Sea",
                    Coord = new(x, YValue.Value, z),
                    IsGround = true,
                    DecalVariant = -1,
                    DecalIntensity = 1,
                });

                var isSea = !noSea.Contains((x, z));

                var zoneGenealogy = new CGameCtnZoneGenealogy
                {
                    CurrentZoneId = isSea ? "Sea" : "Land",
                    ZoneIds = ["VoidToSea"]
                };
                zoneGenealogy.CreateChunk<CGameCtnZoneGenealogy.Chunk0311D002>();
                map.Challenge.ZoneGenealogy.Add(zoneGenealogy);
            }
        }
    }
}