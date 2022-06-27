using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class ClipBlock
{
    [XmlAttribute]
    public string? Content { get; init; }

    [XmlAttribute]
    public ClipMode Mode { get; init; }
}