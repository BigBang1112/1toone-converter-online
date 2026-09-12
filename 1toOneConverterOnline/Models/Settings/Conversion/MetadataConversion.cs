using GBX.NET.Engines.Game;
using GBX.NET.Engines.Script;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class MetadataConversion : Conversion
{
    public override void Convert(Map map)
    {
        var metadata = new CScriptTraitsMetadata();
        metadata.CreateChunk<CScriptTraitsMetadata.Chunk11002000>().Version = 5;

        metadata.Declare("MadeWith1toOneConverterOnline", true);
        metadata.Declare("Unbitn_1toOne_IsConverted", true);
        metadata.Declare("Unbitn_1toOne_ConvertedAt", DateTime.UtcNow.ToString("s"));
        metadata.Declare("Unbitn_1toOne_Phase", "Stable");
        metadata.Declare("Unbitn_1toOne_Environment", map.Environment);
        metadata.Declare("Unbitn_1toOne_OriginalAuthorLogin", map.Challenge.AuthorLogin);
        metadata.Declare("Unbitn_1toOne_OriginalMapUid", map.Challenge.MapUid);

        map.Challenge.ScriptMetadata = metadata;
        map.Challenge.CreateChunk<CGameCtnChallenge.Chunk03043044>();

        map.Challenge.GenerateMapUid();
    }
}
