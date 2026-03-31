using GBX.NET.Engines.Game;
using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class EnviConversion : Conversion
{
    [XmlElement("NewDeco")]
    public NewDecoration[]? NewDecos { get; init; }
    
    public Int3 MapSize { get; init; }

    public override void Convert(Map map)
    {
        if (map.Challenge.Decoration is null)
        {
            throw new Exception($"{nameof(EnviConversion)}: Map has no decoration.");
        }

        if (NewDecos is null)
        {
            throw new Exception($"{nameof(EnviConversion)}: NewDecos is missing.");
        }

        var mood = map.Challenge.Decoration.Id;

        var decoFound = false;

        var newDeco = default(NewDecoration);
        var oldDeco = default(OldDecoration);

        foreach (var newDecoItem in NewDecos)
        {
            if (newDecoItem.OldDeco is null)
            {
                continue;
            }

            foreach (var oldDecoItem in newDecoItem.OldDeco)
            {
                if (oldDecoItem.Name == mood)
                {
                    newDeco = newDecoItem;
                    oldDeco = oldDecoItem;
                    decoFound = true;
                    break;
                }
            }

            if (decoFound)
            {
                break;
            }
        }

        if (!decoFound || newDeco is null || oldDeco is null)
        {
            // TODO: default setting maybe if very bored
            throw new Exception($"{nameof(EnviConversion)}: No decoration found for mood " + mood);
        }

        if (newDeco.Deco?.Collection is null)
        {
            throw new Exception($"{nameof(EnviConversion)}: Deco missing or incorrectly defined.");
        }

        var gridOffset = oldDeco.GridOffset;

        map.Challenge.Decoration = newDeco.Deco;
        map.Challenge.MapInfo = map.Challenge.MapInfo with { Collection = newDeco.Deco.Collection };
        map.Challenge.Size = MapSize;

        foreach (var block in map.Challenge.GetBlocks())
        {
            block.Coord += gridOffset;
        }

        if (map.Challenge.AnchoredObjects is not null)
        {
            foreach (var item in map.Challenge.AnchoredObjects)
            {
                item.AbsolutePositionInMap += gridOffset * map.GridSize;
            }
        }

        map.Challenge.AnchoredObjects ??= [];
        var itemsChunk = map.Challenge.CreateChunk<CGameCtnChallenge.Chunk03043040>();

        if (newDeco.WarpItems is not null)
        {
            foreach (var item in newDeco.WarpItems)
            {
                map.PlaceMinimalItem(item);
            }
        }

        map.Challenge.ThumbnailPosition += gridOffset * map.GridSize;

        TweakClip(map, gridOffset, map.Challenge.ClipIntro);
        TweakClipTriggers(map, gridOffset, map.Challenge.ClipGroupInGame);
        TweakClipTriggers(map, gridOffset, map.Challenge.ClipGroupEndRace);
    }

    private static void TweakClipTriggers(Map map, Int3 gridOffset, CGameCtnMediaClipGroup? clipGroup)
    {
        if (clipGroup is null)
        {
            return;
        }

        foreach (var (clip, trigger) in clipGroup.Clips)
        {
            if (trigger.Coords is null or { Count: 0 })
            {
                continue;
            }

            for (var i = 0; i < trigger.Coords.Count; i++)
            {
                var coord = trigger.Coords[i];
                trigger.Coords[i] = coord + gridOffset;
            }

            TweakClip(map, gridOffset, clip);
        }
    }

    private static void TweakClip(Map map, Int3 gridOffset, CGameCtnMediaClip? clip)
    {
        if (clip is null)
        {
            return;
        }

        foreach (var block in clip.Tracks.SelectMany(x => x.Blocks))
        {
            switch (block)
            {
                case CGameCtnMediaBlockCameraCustom cameraCustomBlock:
                    foreach (var key in cameraCustomBlock.Keys ?? [])
                    {
                        key.Position += gridOffset * map.GridSize;
                    }
                    break;
                case CGameCtnMediaBlockCameraPath cameraPathBlock:
                    foreach (var key in cameraPathBlock.Keys ?? [])
                    {
                        key.Position += gridOffset * map.GridSize;
                    }
                    break;
                case CGameCtnMediaBlockGhost { GhostModel: not null } ghostBlock:

                    // offset all clip ghosts once gbx.net can modify samples
                    break;
            }
        }
    }
}