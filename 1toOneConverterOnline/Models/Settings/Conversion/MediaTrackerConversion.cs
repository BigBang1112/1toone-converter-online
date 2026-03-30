using GBX.NET.Engines.Game;
using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class MediaTrackerConversion : Conversion
{
    [XmlElement]
    public ElementValue<int> OffsetY { get; init; }

    public override void Convert(Map map)
    {
        map.Challenge.Chunks.Remove<CGameCtnChallenge.Chunk03043021>();
        map.Challenge.Chunks.Create<CGameCtnChallenge.Chunk03043049>();

        TweakClip(map, map.Challenge.ClipIntro);
        TweakClipTriggers(map, map.Challenge.ClipGroupInGame);
        TweakClipTriggers(map, map.Challenge.ClipGroupEndRace);
    }

    private void TweakClipTriggers(Map map, CGameCtnMediaClipGroup? clipGroup)
    {
        if (clipGroup is null)
        {
            return;
        }

        var yScale = (int)(map.GridSize.Y / 8);

        foreach (var (clip, trigger) in clipGroup.Clips)
        {
            if (trigger.Coords is null or { Count: 0 })
            {
                continue;
            }

            for (var i = 0; i < trigger.Coords.Count; i++)
            {
                var coord = trigger.Coords[i];
                trigger.Coords[i] = new GBX.NET.Int3(coord.X, coord.Y * yScale + OffsetY.Value, coord.Z);
            }

            var newCoords = new List<GBX.NET.Int3>();
            foreach (var coord in trigger.Coords)
            {
                for (var i = 1; i < yScale; i++)
                {
                    newCoords.Add(coord with { Y = coord.Y + i });
                }
            }

            trigger.Coords.AddRange(newCoords);

            TweakClip(map, clip);
        }
    }

    private static void TweakClip(Map map, CGameCtnMediaClip? clip)
    {
        if (clip is null)
        {
            return;
        }

        foreach (var block in clip.Tracks.SelectMany(x => x.Blocks))
        {
            switch (block)
            {
                case CGameCtnMediaBlockCameraGame cameraGameBlock:
                    // TODO once GameCamOld is nullable, check if it isnt null, and map that into chunk 0x003
                    if (cameraGameBlock.Chunks.Get<CGameCtnMediaBlockCameraGame.Chunk03084000>() is not null
                     || cameraGameBlock.Chunks.Get<CGameCtnMediaBlockCameraGame.Chunk03084001>() is not null)
                    {
                        cameraGameBlock.Chunks.Remove<CGameCtnMediaBlockCameraGame.Chunk03084000>();
                        cameraGameBlock.Chunks.Remove<CGameCtnMediaBlockCameraGame.Chunk03084001>();
                        cameraGameBlock.CreateChunk<CGameCtnMediaBlockCameraGame.Chunk03084003>();

                        cameraGameBlock.GameCamId = cameraGameBlock.GameCamOld switch
                        {
                            CGameCtnMediaBlockCameraGame.EGameCamOld.Internal => "Internal",
                            _ => "<Default>"
                        };
                    }
                    break;
                case CGameCtnMediaBlockGhost { GhostModel: not null } ghostBlock:
                    var vehicle = ghostBlock.GhostModel.PlayerModel;

                    if (vehicle is null)
                    {
                        continue;
                    }

                    ghostBlock.GhostModel.PlayerModel = vehicle with
                    {
                        Id = vehicle.Id + ".Item.Gbx",
                        Collection = map.Challenge.Collection ?? new GBX.NET.Id(),
                        Author = "florenzius",
                    };
                    break;
            }
        }
    }
}