using GBX.NET.Engines.Game;
using TmEssentials;

namespace _1toOneConverterOnline.Models;

/// <summary>
/// Map info that won't get modified by conversion.
/// </summary>
public sealed class MapBasicInfo
{
    public string? AuthorName { get; }
    public TimeInt32? AuthorTime { get; }
    public string? Environment { get; }
    public string? Mood { get; }
    public string? Car { get; }
    public int? BlockCount { get; }

    public MapBasicInfo(CGameCtnChallenge map)
    {
        AuthorName = map.AuthorNickname is null ? map.AuthorLogin : TextFormatter.Deformat(map.AuthorNickname);
        AuthorTime = map.AuthorTime;
        Environment = map.Collection?.ToString();
        Mood = map.Decoration?.Id;

        Car = map.PlayerModel?.Id switch
        {
            "American" => "DesertCar",
            "SnowCar" => "SnowCar",
            "Rally" => "RallyCar",
            "SpeedCar" or "IslandCar" => "IslandCar",
            "BayCar" => "BayCar",
            "StadiumCar" => "StadiumCar",
            "CoastCar" => "CoastCar",
            "" => "Default",
            _ => map.PlayerModel?.Id,
        };

        BlockCount = map.Blocks?.Count;
    }
}
