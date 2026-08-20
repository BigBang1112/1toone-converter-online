using GBX.NET.Engines.Game;
using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class MediaTrackerConversion : Conversion
{
    [XmlElement]
    public ElementValue<int> OffsetY { get; init; }

    [XmlElement]
    public ElementValue<int> SmallOffsetY { get; init; }

    public override void Convert(Map map)
    {
        map.Challenge.Chunks.Remove<CGameCtnChallenge.Chunk03043021>();
        map.Challenge.Chunks.Create<CGameCtnChallenge.Chunk03043049>();

        // Y-offset/upscaled clip triggers don't detect collision with player
        var scaleY = 1; // SmallOffsetY.Value == 0 ? 1 : (int)(map.GridSize.Y / SmallOffsetY.Value);
        map.Challenge.ClipTriggerSize = (3, scaleY, 3);

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

        var scale = map.Challenge.ClipTriggerSize * (1, (int)(map.GridSize.Y / 8), 1);

        // Y-offset/upscaled clip triggers don't detect collision with player
        var triggerYOffset = 0; // SmallOffsetY.Value == 0 ? 0 : (int)(map.GridSize.Y / SmallOffsetY.Value) - 1;

        foreach (var (clip, trigger) in clipGroup.Clips)
        {
            if (trigger.Coords is null or { Count: 0 })
            {
                continue;
            }

            var extraCoordsPerBase = (scale.X * scale.Y * scale.Z) - 1;
            var newCoords = new List<GBX.NET.Int3>(trigger.Coords.Count * extraCoordsPerBase);

            for (var i = 0; i < trigger.Coords.Count; i++)
            {
                var coord = trigger.Coords[i];

                var baseX = coord.X * scale.X;
                var baseY = coord.Y * scale.Y + OffsetY.Value + triggerYOffset * scale.Y;
                var baseZ = coord.Z * scale.Z;

                trigger.Coords[i] = new GBX.NET.Int3(baseX, baseY, baseZ);

                for (var x = 0; x < scale.X; x++)
                {
                    for (var y = 0; y < scale.Y; y++)
                    {
                        for (var z = 0; z < scale.Z; z++)
                        {
                            if (x == 0 && y == 0 && z == 0)
                            {
                                continue;
                            }

                            newCoords.Add(new GBX.NET.Int3(baseX + x, baseY + y, baseZ + z));
                        }
                    }
                }
            }

            trigger.Coords.AddRange(newCoords);

            TweakClip(map, clip);
        }
    }

    private void TweakClip(Map map, CGameCtnMediaClip? clip)
    {
        if (clip is null)
        {
            return;
        }

        var unitOffsetY = (OffsetY.Value - map.BaseHeight) * map.GridSize.Y + SmallOffsetY.Value;

        foreach (var block in clip.Tracks.SelectMany(x => x.Blocks))
        {
            switch (block)
            {
                case CGameCtnMediaBlockCameraGame cameraGameBlock:
                    if (cameraGameBlock.GameCamOld.HasValue)
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

                    foreach (var sample in ghostBlock.GhostModel.SampleData.Samples)
                    {
                        sample.Position = sample.Position with { Y = sample.Position.Y + unitOffsetY };
                    }
                    break;
                case CGameCtnMediaBlockCameraCustom cameraCustomBlock:
                    foreach (var key in cameraCustomBlock.Keys ?? [])
                    {
                        if (key.Anchor == -1)
                        {
                            key.Position = key.Position with { Y = key.Position.Y + unitOffsetY };
                        }

                        /*if (key.Interpolation == CGameCtnMediaBlockCameraCustom.Interpolation.Hermite)
                        {
                            key.Interpolation = CGameCtnMediaBlockCameraCustom.Interpolation.Linear;
                        }*/
                    }
                    break;
                case CGameCtnMediaBlockCameraPath cameraPathBlock:
                    foreach (var key in cameraPathBlock.Keys ?? [])
                    {
                        if (key.Anchor == -1)
                        {
                            key.Position = key.Position with { Y = key.Position.Y + unitOffsetY };
                        }
                    }
                    break;
            }
        }
    }
}