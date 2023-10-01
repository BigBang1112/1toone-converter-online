using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class Id
{
    [XmlAttribute("CollectionID")]
    public uint CollectionId { get; init; }

    [XmlAttribute]
    public string? Content { get; init; }

    public static implicit operator GBX.NET.Id(Id id)
    {
        if (id.Content is not null)
        {
            return new GBX.NET.Id(id.Content);
        }

        return new GBX.NET.Id((int)id.CollectionId);
    }

    public static implicit operator string(Id id)
    {
        return id.Content ?? new GBX.NET.Id((int)id.CollectionId);
    }
}
