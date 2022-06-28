using GBX.NET.Engines.Game;
using TmEssentials;

namespace _1toOneConverterOnline.Models;

public class MapItem
{
    private string? thumbnailBase64;

    public string FileName { get; }
    public int FileSize { get; }
    public DateTimeOffset LastModified { get; }

    public CGameCtnChallenge? Map { get; set; }
    public string? Description { get; set; }
    public bool Selected { get; set; }

    public string? Name => Map?.MapName is null ? null : TextFormatter.Deformat(Map.MapName);
    public string? AuthorName => Map?.AuthorNickname is null ? Map?.AuthorLogin : TextFormatter.Deformat(Map.AuthorNickname);

    public string? EnvironmentModern => Map?.Collection.ToString() switch
    {
        "Alpine" => "Snow",
        "Speed" => "Desert",
        _ => Map?.Collection,
    };

    public string? ThumbnailBase64
    {
        get
        {
            if (thumbnailBase64 is null && Map?.Thumbnail is not null)
            {
                thumbnailBase64 = Convert.ToBase64String(Map.Thumbnail);
            }

            return thumbnailBase64;
        }
    }

    public MapItem(string fileName, int fileSize, DateTimeOffset lastModified)
    {
        FileName = fileName;
        FileSize = fileSize;
        LastModified = lastModified;
    }
}
