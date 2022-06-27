using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public enum BlockType
{
    [XmlEnum("Air")] Air,
    [XmlEnum("Gnd")] Ground, // Ground
    [XmlEnum("Pri")] GroundPrimary, // GroundPrimary
    [XmlEnum("Sec")] GroundSecondary, // GroundSecondary
}