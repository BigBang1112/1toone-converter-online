using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class ItemClipAddConversion : Conversion
{
    public Id? Collection { get; init; }
    public Id? DefaultAuthor { get; init; }

    [XmlElement("ClipBlock")]
    public ClipBlock[]? ClipBlocks { get; init; }

    [XmlElement("SecondaryTerrainClipBlock", IsNullable = false)]
    public ClipBlock[]? SecondaryTerrainClipBlocks { get; init; }

    public ItemData? ClipFiller { get; init; }
    public FlagName? GroundClipFlag { get; init; }
    public MultiPylon GroundClipPylon { get; init; }

    [XmlArray]
    public ClipData[]? ClipItemInfos { get; init; }

    public FlagName? ItemCountStatistic { get; init; }
}