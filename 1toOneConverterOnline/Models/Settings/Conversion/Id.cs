using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class Id
{
    [XmlAttribute("CollectionID")]
    public uint CollectionId { get; init; }

    [XmlAttribute]
    public string? Content { get; init; }
}
