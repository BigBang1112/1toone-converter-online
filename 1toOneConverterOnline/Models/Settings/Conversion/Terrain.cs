using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

sealed class Terrain
{
    public TerrainBlock[]? ConvexBlocks { get; init; }
    public TerrainBlock[]? WallBlocks { get; init; }
    public TerrainBlock[]? ConcaveBlocks { get; init; }

    [XmlAttribute]
    public int Height { get; init; }

    [XmlAttribute]
    public bool SecondaryTerrain { get; init; }
}