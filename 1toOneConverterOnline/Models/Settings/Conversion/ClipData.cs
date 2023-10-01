using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

sealed class ClipData
{
    [XmlAttribute]
    public string? Clip { get; init; }
}