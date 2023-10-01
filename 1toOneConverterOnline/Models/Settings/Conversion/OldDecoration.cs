using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

sealed class OldDecoration
{
    [XmlAttribute]
    public string? Name { get; init; }
    public Int3 GridOffset { get; init; }
}