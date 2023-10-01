using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class BlockToItem
{
    [XmlAttribute]
    public string? ItemName { get; init; }

    [XmlAttribute]
    public string? ItemAuthor { get; init; }

    [XmlAttribute]
    public sbyte YOffset { get; init; }

    [XmlAttribute]
    public byte RotOffset { get; init; }

    [XmlAttribute]
    public sbyte XOffset { get; init; }

    [XmlAttribute]
    public sbyte ZOffset { get; init; }

    [XmlAttribute]
    public float SmallYOffset { get; init; }

    [XmlElement("BlockData", typeof(BlockData))]
    [XmlElement("BlockVariantData", typeof(BlockVariantData))]
    [XmlElement("BlockTypeData", typeof(BlockTypeData))]
    [XmlElement("BlockRandomData", typeof(BlockRandomData))]
    [XmlElement("BlockSkinData", typeof(BlockSkinData))]
    public BlockToItem[]? Children { get; init; }

    [XmlElement("Flag")]
    public Flag[]? Flags { get; init; }

    [XmlElement("Clip")]
    public Clip[]? Clips { get; init; }

    [XmlElement("Pylon")]
    public MultiPylon[]? MultiPylons { get; init; }

    public bool ShouldSerializeYOffset() => (uint)YOffset > 0U;
    public bool ShouldSerializeRotOffset() => RotOffset > 0;
    public bool ShouldSerializeXOffset() => (uint)XOffset > 0U;
    public bool ShouldSerializeZOffset() => (uint)ZOffset > 0U;
}