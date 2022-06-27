using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

[Flags]
public enum MultiRot
{
    [XmlEnum("0")] Zero = 1,
    [XmlEnum("1")] One = 2,
    [XmlEnum("2")] Two = 4,
    [XmlEnum("3")] Three = 8,
    [XmlEnum("All")] All = Three | Two | One | Zero // 0x0000000F
}