using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class FlagName
{
    [XmlAttribute]
    public string? Name { get; init; }
}
