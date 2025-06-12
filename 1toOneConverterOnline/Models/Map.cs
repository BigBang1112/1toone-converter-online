using GBX.NET;
using GBX.NET.Engines.Game;

namespace _1toOneConverterOnline.Models;

public sealed class Map
{
    public CGameCtnChallenge Challenge { get; init; }

    public Vec3 GridSize { get; set; }
    public Vec3 GridOffset { get; set; }
    public int BaseHeight { get; set; }
    public string Environment { get; set; } = "";

    internal HashSet<Int3>? CoveredCoords { get; set; }
    internal Dictionary<string, HashSet<Settings.Conversion.Clip>> Clips { get; } = [];
    internal Dictionary<Settings.Conversion.PylonType, HashSet<Settings.Conversion.Pylon>> Pylons { get; } = [];
    internal HashSet<Int3> TerrainModifiers { get; set; } = [];

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

    public void AddPylon(Settings.Conversion.Pylon pylon)
    {
        if (!Pylons.ContainsKey(pylon.Type))
        {
            Pylons.Add(pylon.Type, []);
        }

        Pylons[pylon.Type].Add(pylon.Normalize());
    }

    public void AddPylons(IEnumerable<Settings.Conversion.Pylon> pylonList, Int3 coord, Direction rot)
    {
        foreach (var pylon in pylonList)
        {
            AddPylon(pylon.GetRelativeToBlock(coord, rot));
        }
    }

    public IEnumerable<Settings.Conversion.Pylon> GetPylons(Settings.Conversion.PylonType type)
    {
        return Pylons.TryGetValue(type, out HashSet<Settings.Conversion.Pylon>? value) ? value : [];
    }

    public Vec3 ConvertCoords((byte x, byte y, byte z) coords)
    {
        return new Vec3(
            coords.x * GridSize.X + GridOffset.X,
            coords.y * GridSize.Y + GridOffset.Y,
            coords.z * GridSize.Z + GridOffset.Z
        );
    }

    public Vec3 ConvertPylonCoords((byte x, byte y, byte z) coords, byte rot)
    {
        return rot switch
        {
            2 => new Vec3(
                 coords.x * GridSize.X + GridOffset.X,
                 coords.y * GridSize.Y + GridOffset.Y,
                (coords.z - 0.5f) * GridSize.Z + GridOffset.Z
            ),
            1 => new Vec3(
                (coords.x - 0.5f) * GridSize.X + GridOffset.X,
                 coords.y * GridSize.Y + GridOffset.Y,
                 coords.z * GridSize.Z + GridOffset.Z
            ),
            _ => throw new Exception()
        };
    }
}
