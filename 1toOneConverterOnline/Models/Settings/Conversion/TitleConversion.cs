using GBX.NET.Engines.Game;
using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class TitleConversion : Conversion
{
    [XmlElement("TitleUID")]
    public Id? TitleUid { get; init; }
    
    public override void Convert(Map map)
    {
        var m = map.Challenge;

        m.CreateChunk<CGameCtnChallenge.HeaderChunk03043003>().Version = 11;

        if (TitleUid?.Content is null)
        {
            throw new Exception("TitleConversion does not have a valid title ID");
        }

        m.TitleId = TitleUid.Content;

        var isValidated = m.Kind != CGameCtnChallenge.MapKind.InProgress ? 1 : 0;

        m.Xml = $@"<header type=""map"" exever=""3.3.0"" exebuild=""2019-11-19_18_50"" title=""{m.TitleId}"" lightmap=""7"">" +
            $@"<ident uid=""{m.MapUid}"" name=""{m.MapName}"" author=""{m.AuthorLogin}"" authorzone=""{m.AuthorZone}""/>" +
            $@"<desc envir=""{m.Collection}"" mood=""{m.Decoration?.Id}"" type=""{m.Mode}"" maptype=""{m.MapType}"" mapstyle=""{m.MapStyle}"" validated=""{isValidated}"" nblaps=""{m.NbLaps}"" displaycost=""{m.Cost}"" hasghostblocks=""{(m.HasGhostBlocks ? 1 : 0)}"" />" +
            $@"<times bronze=""{m.BronzeTime?.TotalMilliseconds ?? -1}"" silver=""{m.SilverTime?.TotalMilliseconds ?? -1}"" gold=""{m.GoldTime?.TotalMilliseconds ?? -1}"" authortime=""{m.AuthorTime?.TotalMilliseconds ?? -1}"" authorscore=""{m.AuthorScore}""/></header>";

        m.CreateChunk<CGameCtnChallenge.Chunk03043051>();
    }
}
