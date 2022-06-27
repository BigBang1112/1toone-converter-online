using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public struct Vec3
{
    [XmlAttribute]
    public float X { get; init; }

    [XmlAttribute]
    public float Y { get; init; }

    [XmlAttribute]
    public float Z { get; init; }
}