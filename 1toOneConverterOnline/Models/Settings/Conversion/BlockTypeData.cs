using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class BlockTypeData : BlockToItem
{
    private BlockType? type;

    [XmlAttribute("Type")]
    public BlockType TypeOfBlock
    {
        get => type.GetValueOrDefault();
        init => type = value;
    }

    public bool ShouldSerializeTypeOfBlock() => type.HasValue;
}