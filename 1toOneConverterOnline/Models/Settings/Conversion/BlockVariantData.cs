using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class BlockVariantData : BlockToItem
{
    private byte? variant;

    [XmlAttribute]
    public byte Variant
    {
        get => variant.GetValueOrDefault();
        init => variant = value;
    }

    public bool ShouldSerializeVariant() => variant.HasValue;
}