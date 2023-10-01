using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

sealed class Ident
{
    [XmlElement("ID")]
    public Id? Id { get; init; }
    public Id? Collection { get; init; }
    public Id? Author { get; init; }

    public static implicit operator GBX.NET.Ident(Ident ident)
    {
        return new GBX.NET.Ident(ident.Id ?? "", ident.Collection ?? new GBX.NET.Id(), ident.Author ?? "");
    }
}