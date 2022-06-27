using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class FlagName
{
    [XmlAttribute]
    public string? Name { get; init; }
}
