using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class TerrainMappingConversion : Conversion
{
    public FlagName? HeightFlag { get; init; }
    public FlagName? SecondaryTerrainFlag { get; init; }

    [XmlElement]
    public ElementValue<byte> BaseHeight { get; init; }
    
    [XmlElement(ElementName = "Terrain", IsNullable = false)]
    public Terrain[]? Terrains;
}