using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class EnviConversion : Conversion
{
    [XmlElement("NewDeco")]
    public NewDecoration[]? NewDecos { get; init; }
    public Int3 MapSize { get; init; }
}