using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class OldDecoration
{
    [XmlAttribute]
    public string? Name { get; init; }
    public Vec3 GridOffset { get; init; }
}