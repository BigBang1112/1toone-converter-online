using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

sealed class ClipBlock
{
    [XmlAttribute]
    public string? Content { get; init; }

    [XmlAttribute]
    public ClipMode Mode { get; init; }
}