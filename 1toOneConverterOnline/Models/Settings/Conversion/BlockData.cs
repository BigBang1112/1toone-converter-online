using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class BlockData : BlockToItem
{
    [XmlElement("AltName")]
    public AlternativeName[]? AltNames { get; init; }

    [XmlAttribute]
    public string? BlockName { get; init; }

    [XmlAttribute]
    public byte BlockXSize { get; init; }

    [XmlAttribute]
    public byte BlockZSize { get; init; }
}