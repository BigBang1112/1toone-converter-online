using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public readonly record struct ElementValue<T>
{
    [XmlAttribute]
    public required T Value { get; init; }
}