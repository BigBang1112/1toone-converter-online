using GBX.NET;
using GBX.NET.Engines.Game;

namespace _1toOneConverterOnline.Models;

public sealed class Map
{
    public CGameCtnChallenge Challenge { get; init; }

    public Vec3 GridSize { get; set; }
    public Vec3 GridOffset { get; set; }
    public int BaseHeight { get; set; }

    internal HashSet<Int3>? CoveredCoords { get; set; }
    internal Dictionary<string, HashSet<Settings.Conversion.Clip>> Clips { get; } = [];

    public Map(CGameCtnChallenge challenge)
    {
        Challenge = challenge;
    }

    public void PlaceMinimalItem(Settings.Conversion.MinimalItem item)
    {
        if (item.Meta is null)
        {
            return;
        }

        var anchoredObject = Challenge.PlaceAnchoredObject(item.Meta, item.ItemCoords, item.Rot);
        anchoredObject.BlockUnitCoord = (Byte3)item.BlockCoords;
    }

    public void AddClip(Settings.Conversion.Clip clip)
    {
        var name = clip.Name ?? "";

        if (!Clips.TryGetValue(name, out HashSet<Settings.Conversion.Clip>? value))
        {
            value = [];
            Clips.Add(name, value);
        }

        value.Add(clip);
    }

    public void AddClips(IEnumerable<Settings.Conversion.Clip> clipList, Int3 coord, Direction rot)
    {
        foreach (var clip in clipList)
        {
            AddClip(clip.GetRelativeToBlock(coord, rot));
        }
    }

    public IEnumerable<Settings.Conversion.Clip> GetClips(string name)
    {
        return Clips.TryGetValue(name, out HashSet<Settings.Conversion.Clip>? value) ? value : [];
    }
}
