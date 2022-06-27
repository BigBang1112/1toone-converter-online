using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class GroundItemAddConversion : Conversion
{
    public Id? Collection { get; init; }
    public Id? DefaultAuthor { get; init; }

    [XmlElement("BlockIgnoreFlag", IsNullable = false)]
    public FlagName[]? BlockIgnoreFlags { get; init; }

    public FlagName? HeightFlag { get; init; }
    public ItemData? GroundItem { get; init; }
}