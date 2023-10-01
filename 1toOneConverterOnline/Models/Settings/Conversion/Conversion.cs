using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

[XmlRoot]
[XmlInclude(typeof(BlockAddConversion))]
[XmlInclude(typeof(BlockClearConversion))]
[XmlInclude(typeof(BlockToBlockConversion))]
[XmlInclude(typeof(BlockToItemConversion))]
[XmlInclude(typeof(CarConversion))]
[XmlInclude(typeof(EnviConversion))]
[XmlInclude(typeof(GridDefineConversion))]
[XmlInclude(typeof(GroundItemAddConversion))]
[XmlInclude(typeof(ItemClipAddConversion))]
[XmlInclude(typeof(MultiBlockAddConversion))]
[XmlInclude(typeof(PylonAddConversion))]
[XmlInclude(typeof(TerrainMappingConversion))]
[XmlInclude(typeof(TitleConversion))]
public abstract class Conversion
{
    public abstract void Convert(Map map);
}
