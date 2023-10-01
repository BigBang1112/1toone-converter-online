using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class BlockRandomData : BlockToItem
{
    private byte? variant;

    [XmlAttribute]
    public byte Variant
    {
        get => variant.GetValueOrDefault();
        init => variant = value;
    }
}