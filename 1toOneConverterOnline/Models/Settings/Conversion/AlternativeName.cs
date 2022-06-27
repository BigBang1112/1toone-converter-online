using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class AlternativeName
{
    [XmlAttribute]
    public string? BlockName { get; init; }

    [XmlIgnore]
    public bool? PriSecTerrain { get; init; }

    [XmlAttribute]
    public bool SecondaryTerrain
    {
        get => PriSecTerrain.GetValueOrDefault();
        init => PriSecTerrain = value;
    }
}
