using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class BlockSkinData : BlockToItem
{
    [XmlAttribute]
    public string? SkinRegex { get; init; }
}