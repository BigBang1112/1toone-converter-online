using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

readonly struct MultiPylon
{
    [XmlAttribute]
    public PylonType Type { get; init; }

    [XmlAttribute]
    public PylonPosition Pos { get; init; }

    [XmlAttribute]
    public short X { get; init; }

    [XmlAttribute]
    public short Y { get; init; }

    [XmlAttribute]
    public short Z { get; init; }

    [XmlAttribute]
    public bool Optional { get; init; }

    [XmlAttribute]
    public MultiRot Rot { get; init; }
}