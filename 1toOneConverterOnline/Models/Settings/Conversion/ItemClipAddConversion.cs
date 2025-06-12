using GBX.NET;
using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class ItemClipAddConversion : Conversion
{
    public Id? Collection { get; init; }
    public Id? DefaultAuthor { get; init; }

    [XmlElement("ClipBlock")]
    public ClipBlock[]? ClipBlocks { get; init; }

    [XmlElement("SecondaryTerrainClipBlock", IsNullable = false)]
    public ClipBlock[]? SecondaryTerrainClipBlocks { get; init; }

    public ItemData? ClipFiller { get; init; }
    public FlagName? GroundClipFlag { get; init; }
    public MultiPylon GroundClipPylon { get; init; }

    [XmlArray]
    public ClipData[]? ClipItemInfos { get; init; }

    public FlagName? ItemCountStatistic { get; init; }

    public override void Convert(Map map)
    {
        var itemCount = 0;

        var clipList = new List<(ClipData clipItemInfo, byte rot)>[128, map.Challenge.Size.Y, 128];

        foreach (var clipItemInfo in ClipItemInfos ?? [])
        {
            var clips = map.GetClips(clipItemInfo.Clip ?? "");
            if (clips is not null)
            {
                foreach (var clip in clips)
                {
                    var list = clipList[clip.X, clip.Y, clip.Z] ??= [];
                    list.Add((clipItemInfo, clip.Rot));
                }
            }
        }

        foreach (var block in map.Challenge.GetBlocks())
        {
            var clipBlock = ClipBlocks?.FirstOrDefault(x => x.Content == block.Name)
                ?? SecondaryTerrainClipBlocks?.FirstOrDefault(x => x.Content == block.Name);

            //Test if block is clip and collect the needed metadata
            var isSecondaryTerrain = map.TerrainModifiers.Contains(block.Coord with { Y = 0 });

            /*if (block.Name.Equals(ClipBlocks))
                isSecondaryTerrain = false;
            else if (SecondaryTerrainClipBlock != null && block.BlockName.Equals(SecondaryTerrainClipBlock))
                isSecondaryTerrain = true;
            else
                continue;*/ //Block is not a Clip, check next block.
            if (clipBlock is not null)
            {

            }
            else
            {
                continue;
            }

            //Block is Clip
            //Get all clips in this position
            var positionedClips = clipList[block.Coord.X, block.Coord.Y, block.Coord.Z];
            if (positionedClips is null)
            {
                continue;
            }
            var placedClips = new bool[4];

            //Get Clips by rotation
            var rotatedClips = from clip in positionedClips
                               group clip.clipItemInfo by clip.rot into g
                               select g;

            foreach (var rotation in rotatedClips)
            {
                var rot = rotation.Key;
                var neighbourCoords = Clip.GetCoordsFacing(block.Coord, rot);

                var neighbourClips = clipList[neighbourCoords.X, neighbourCoords.Y, neighbourCoords.Z];

                foreach (var clipItemInfo in rotation)
                {
                    /*if (neighbourClips is not null && neighbourClips.Exists(x => x.rot == (rot + 2) % 4 && x.clipItemInfo.Clip == clipItemInfo.Clip))
                    {
                        //clip is connected, try next clip
                        continue;
                    }*/

                    foreach (var b in clipItemInfo.Children ?? [])
                    {
                        if (b is BlockVariantData variantData && variantData.Variant != block.Variant)
                        {
                            continue;
                        }

                        if (b is BlockTypeData typeData)
                        {
                            if (typeData.TypeOfBlock == BlockType.Air && block.IsGround)
                            {
                                continue;
                            }

                            if (typeData.TypeOfBlock is BlockType.Ground or BlockType.GroundPrimary or BlockType.GroundSecondary && !block.IsGround)
                            {
                                continue;
                            }

                            if (typeData.TypeOfBlock is BlockType.GroundPrimary && isSecondaryTerrain)
                            {
                                continue;
                            }

                            if (typeData.TypeOfBlock is BlockType.GroundSecondary && !isSecondaryTerrain)
                            {
                                continue;
                            }
                        }

                        if (b.ItemName is null)
                        {
                            continue;
                        }

                        var ident = new GBX.NET.Ident(b.ItemName,
                            Collection ?? throw new Exception($"{nameof(ItemClipAddConversion)}: Collection missing."),
                            b.ItemAuthor ?? DefaultAuthor ?? "");

                        var posOffset = new GBX.NET.Int3(b.XOffset, b.YOffset, b.ZOffset);
                        var absolutePosition = (block.Coord + (0, posOffset.Y, 0)) * map.GridSize + map.GridOffset + (0, b.SmallYOffset, 0);
                        var pitchYawRoll = new GBX.NET.Vec3((rot + b.RotOffset /*+ rotOffset*/) % 4 * -MathF.PI / 2, 0, 0);

                        int xOffset;
                        int zOffset;

                        switch ((Direction)rot)
                        {
                            case Direction.North:
                                xOffset = posOffset.X;
                                zOffset = posOffset.Z;
                                break;
                            case Direction.East:
                                xOffset = -posOffset.Z;
                                zOffset = posOffset.X;
                                break;
                            case Direction.South:
                                xOffset = -posOffset.X;
                                zOffset = -posOffset.Z;
                                break;
                            case Direction.West:
                                xOffset = posOffset.Z;
                                zOffset = -posOffset.X;
                                break;
                            default:
                                throw new Exception();
                        }

                        absolutePosition += (xOffset, 0, zOffset) * map.GridSize;

                        //clip is unconnected, items must be placed
                        map.Challenge.PlaceAnchoredObject(ident, absolutePosition, pitchYawRoll);

                        placedClips[rot] = true;

                        itemCount++;
                    }
                }
            }

            if (block.IsGround && clipBlock.Mode != ClipMode.ForceAir)
            {
                //Place filler items
                for (byte rot = 0; rot < 4; rot++)
                {
                    if (placedClips[rot])
                    {
                        continue;
                    }

                    foreach (var b in ClipFiller?.Children ?? [])
                    {
                        if (b is BlockVariantData variantData && variantData.Variant != block.Variant)
                        {
                            continue;
                        }

                        if (b is BlockTypeData typeData)
                        {
                            if (typeData.TypeOfBlock == BlockType.Air && block.IsGround)
                            {
                                continue;
                            }

                            if (typeData.TypeOfBlock is BlockType.Ground or BlockType.GroundPrimary or BlockType.GroundSecondary && !block.IsGround)
                            {
                                continue;
                            }

                            if (typeData.TypeOfBlock is BlockType.GroundPrimary && isSecondaryTerrain)
                            {
                                continue;
                            }

                            if (typeData.TypeOfBlock is BlockType.GroundSecondary && !isSecondaryTerrain)
                            {
                                continue;
                            }
                        }

                        if (b?.ItemName is null)
                        {
                            continue;
                        }

                        var ident = new GBX.NET.Ident(b.ItemName,
                            Collection ?? throw new Exception($"{nameof(ItemClipAddConversion)}: Collection missing."),
                            b.ItemAuthor ?? DefaultAuthor ?? "");

                        var posOffset = new GBX.NET.Int3(b.XOffset, b.YOffset, b.ZOffset);
                        var absolutePosition = (block.Coord + (0, posOffset.Y, 0)) * map.GridSize + map.GridOffset + (0, b.SmallYOffset, 0);
                        var pitchYawRoll = new GBX.NET.Vec3((rot + b.RotOffset /*+ rotOffset*/) % 4 * -MathF.PI / 2, 0, 0);

                        map.Challenge.PlaceAnchoredObject(ident, absolutePosition, pitchYawRoll);
                    }
                }

                //file.SetFlag(new Flag(GroundClipFlag, block.Coords.X, block.Coords.Z));
                //file.AddPylons(GroundClipPylon.GetPylons().AsEnumerable(), block.Coords.X, block.Coords.Y, block.Coords.Z, 0);
            }
        }
    }
}