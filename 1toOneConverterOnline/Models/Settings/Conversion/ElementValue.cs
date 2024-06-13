using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed record ElementValue<T>
{
    [XmlAttribute]
    public required T Value { get; init; }
}