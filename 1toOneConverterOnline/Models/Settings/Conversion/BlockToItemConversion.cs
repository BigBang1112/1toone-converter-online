using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class BlockToItemConversion : Conversion
{
    public Id? Collection { get; init; }
    public Id? DefaultAuthor { get; init; }
    public FlagName? SecondaryTerrainFlag { get; init; }
    
    [XmlElement("BlockIgnoreFlag")]
    public FlagName[]? BlockIgnoreFlags { get; init; }
    
    public List<BlockData>? Blocks { get; init; }
    public FlagName? ItemCountStatistic { get; init; }
}