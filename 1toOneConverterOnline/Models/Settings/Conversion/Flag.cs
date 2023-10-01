using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class Flag
{
    [XmlAttribute]
    public string? Name { get; init; }

    [XmlAttribute]
    public short X { get; init; }

    [XmlAttribute]
    public byte Y { get; init; }

    [XmlAttribute]
    public short Z { get; init; }
}