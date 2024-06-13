using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class Block
{
    public Id? BlockName { get; set; }

    [XmlElement]
    public ElementValue<byte> Rot { get; init; }

    public Int3 Coords { get; set; }

    [XmlElement]
    public ElementValue<int> Flags { get; init; }

    public Id? Author { get; set; }
}