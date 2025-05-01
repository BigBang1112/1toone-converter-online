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
}
