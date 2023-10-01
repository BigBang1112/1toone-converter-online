using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

sealed class FlagName
{
    [XmlAttribute]
    public string? Name { get; init; }
}
