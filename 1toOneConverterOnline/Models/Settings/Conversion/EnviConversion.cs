using GBX.NET.Engines.Game;
using System.Numerics;
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
        /*if (map.Challenge.ThumbnailRotationMatrix.HasValue)
        {
            Matrix4x4.Decompose(map.Challenge.ThumbnailRotationMatrix.Value, out _, out Quaternion rotationQ, out _);
            map.Challenge.ThumbnailPitchYawRoll = QuaternionToEuler(rotationQ);
            map.Challenge.RemoveChunk<CGameCtnChallenge.Chunk03043027>();
            map.Challenge.RemoveChunk<CGameCtnChallenge.Chunk03043028>();
            map.Challenge.CreateChunk<CGameCtnChallenge.Chunk03043036>();
        }*/

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
                        if (key.Anchor == -1)
                        {
                            key.Position += gridOffset * map.GridSize;
                        }
                    }
                    break;
                case CGameCtnMediaBlockCameraPath cameraPathBlock:
                    foreach (var key in cameraPathBlock.Keys ?? [])
                    {
                        if (key.Anchor == -1)
                        {
                            key.Position += gridOffset * map.GridSize;
                        }
                    }
                    break;
                case CGameCtnMediaBlockGhost { GhostModel: not null } ghostBlock:

                    foreach (var sample in ghostBlock.GhostModel.SampleData.Samples)
                    {
                        sample.Position += gridOffset * map.GridSize;
                    }

                    break;
            }
        }
    }

    private static void GetLocFreeVal(Matrix4x4 matrix, out Vector3 position, out Vector3 rotation)
    {
        position = new Vector3(matrix.M41, matrix.M42, matrix.M43);

        float m31 = Math.Clamp(matrix.M31, -1f, 1f);

        float pitch = (float)Math.Asin(m31);
        float cosPitch = (float)Math.Cos(pitch);

        float roll, yaw;

        if (Math.Abs(cosPitch) <= 0.005f)
        {
            roll = 0f;
            yaw = (float)Math.Atan2(matrix.M12, matrix.M22);
        }
        else
        {
            roll = (float)Math.Atan2(-matrix.M32, matrix.M33);
            yaw = (float)Math.Atan2(-matrix.M21, matrix.M11);
        }

        rotation = new Vector3(roll, pitch, yaw);
    }
}