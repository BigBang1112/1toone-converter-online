using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class ClipData : ItemData
{
    [XmlAttribute]
    public string? Clip { get; init; }
}