using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class TerrainBlock
{
    [XmlAttribute]
    public string? BlockName { get; init; }

    [XmlAttribute]
    public byte Variant { get; init; }
}