using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class BlockToBlockConversion : Conversion
{
    [XmlElement("BlockToBlock")]
    public List<BlockToBlock>? BlockToBlocks { get; init; }
}