using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class BlockToBlock
{
    [XmlAttribute]
    public string? OldName { get; init; }

    [XmlAttribute]
    public string? NewName { get; init; }

    [XmlAttribute]
    public byte YOffset { get; init; }
}