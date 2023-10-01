using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

enum BlockType
{
    [XmlEnum("Air")] Air,
    [XmlEnum("Gnd")] Ground, // Ground
    [XmlEnum("Pri")] GroundPrimary, // GroundPrimary
    [XmlEnum("Sec")] GroundSecondary, // GroundSecondary
}