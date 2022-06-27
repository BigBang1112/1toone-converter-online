using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public struct Int3
{
    [XmlAttribute]
    public int X { get; init; }
    
    [XmlAttribute]
    public int Y { get; init; }

    [XmlAttribute]
    public int Z { get; init; }
}