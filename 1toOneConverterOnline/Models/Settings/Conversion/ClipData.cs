using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class ClipData
{
    [XmlAttribute]
    public string? Clip { get; init; }
}