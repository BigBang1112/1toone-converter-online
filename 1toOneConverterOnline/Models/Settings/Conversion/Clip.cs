using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class Clip
{
    [XmlAttribute]
    public string? Name { get; init; }

    [XmlAttribute]
    public short X { get; init; }

    [XmlAttribute]
    public short Y { get; init; }

    [XmlAttribute]
    public short Z { get; init; }

    [XmlAttribute]
    public byte Rot { get; init; }
}