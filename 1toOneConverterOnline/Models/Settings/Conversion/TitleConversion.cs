using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class TitleConversion : Conversion
{
    [XmlElement("TitleUID")]
    public Id? TitleUid { get; init; }
}
