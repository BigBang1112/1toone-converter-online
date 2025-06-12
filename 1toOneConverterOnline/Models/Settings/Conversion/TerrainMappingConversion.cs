using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class TerrainMappingConversion : Conversion
{
    public FlagName? HeightFlag { get; init; }
    public FlagName? SecondaryTerrainFlag { get; init; }
    public FlagName? SecondaryTerrainBlock { get; init; }

    [XmlElement]
    public ElementValue<byte> BaseHeight { get; init; }
    
    [XmlElement(ElementName = "Terrain", IsNullable = false)]
    public Terrain[]? Terrains;

    public override void Convert(Map map)
    {
        map.BaseHeight = BaseHeight.Value;

        if (SecondaryTerrainBlock is null)
        {
            return;
        }

        foreach (var block in map.Challenge.GetBlocks().Where(x => x.Name == SecondaryTerrainBlock.Name))
        {
            map.TerrainModifiers.Add(block.Coord with { Y = 0 });
        }
    }
}