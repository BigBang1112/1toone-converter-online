using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class Ident
{
    [XmlElement("ID")]
    public Id? Id { get; init; }
    public Id? Collection { get; init; }
    public Id? Author { get; init; }
}