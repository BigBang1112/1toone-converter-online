using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class NewDecoration
{
    [XmlElement]
    public OldDecoration[]? OldDeco { get; init; }
    public Ident? Deco { get; init; }

    [XmlElement("WarpItem", IsNullable = false)]
    public MinimalItem[]? WarpItems { get; init; }
    
    public Int3? MapSize { get; init; }

    public bool ShouldSerializeMapSize()
    {
        return MapSize.HasValue;
    }
}