using GBX.NET;
using GBX.NET.Engines.Game;
using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class BlockToItemConversion : Conversion
{
    private readonly Dictionary<string, BlockData> blockDictionary = [];

    public Id? Collection { get; init; }
    public Id? DefaultAuthor { get; init; }
    public FlagName? SecondaryTerrainFlag { get; init; }

    [XmlElement("BlockIgnoreFlag")]
    public FlagName[]? BlockIgnoreFlags { get; init; }

    public List<BlockData>? Blocks { get; init; }
    public FlagName? ItemCountStatistic { get; init; }

    public override void Convert(Map map)
    {
        int itemCount = 0;

        if (Blocks is null)
        {
            return;
        }

        foreach (var block in Blocks)
        {
            if (!string.IsNullOrEmpty(block.BlockName))
            {
                _ = blockDictionary.TryAdd(block.BlockName, block);
            }

            foreach (var altName in block.AltNames ?? [])
            {
                if (!string.IsNullOrEmpty(altName.BlockName))
                {
                    _ = blockDictionary.TryAdd(altName.BlockName, block);
                }
            }
        }

        foreach (var block in map.Challenge.GetBlocks().Where(x => !x.IsClip))
        {
            // bool isSecondaryTerrain = this.SecondaryTerrainFlag != null && file.TestFlag(this.SecondaryTerrainFlag.Name, (int) tuple.x, (int) tuple.z);

            if (ConvertBlock(map, block))
            {
                itemCount++;
            }
        }

        map.CoveredCoords ??= GetCoveredCoords(map).ToHashSet();
    }

    private bool ConvertBlock(Map map, CGameCtnBlock block)
    {
        if (!blockDictionary.TryGetValue(block.Name, out BlockData? blockData))
        {
            return false;
        }

        if (map.CoveredCoords is not null && map.CoveredCoords.Contains(block.Coord with { Y = 0 })) //  + (0, blockData.YOffset, 0) // TODO wrong for flying blocks!
        {
            return false;
        }

        var blockSize = new Int2(
            blockData.BlockXSize is 0 ? 1 : blockData.BlockXSize,
            blockData.BlockZSize is 0 ? 1 : blockData.BlockZSize);

        PlaceItem(map, block, blockData, blockSize);

        // TBD
        return true;
    }

    private void PlaceItem(Map map, CGameCtnBlock block, BlockToItem blockData, Int2 blockSize, GBX.NET.Int3 posOffset = default, int rotOffset = default, float smallYOffset = default)
    {
        posOffset += new GBX.NET.Int3(blockData.XOffset, blockData.YOffset, blockData.ZOffset);
        smallYOffset += blockData.SmallYOffset;

        if (blockData.ItemName is not null)
        {
            var ident = new GBX.NET.Ident(blockData.ItemName,
                Collection ?? throw new Exception($"{nameof(BlockToItemConversion)}: Collection missing."),
                blockData.ItemAuthor ?? DefaultAuthor ?? "");

            var absolutePosition = (block.Coord + (0, posOffset.Y, 0)) * map.GridSize + map.GridOffset + (0, smallYOffset, 0);
            var pitchYawRoll = new GBX.NET.Vec3(((int)block.Direction + blockData.RotOffset + rotOffset) % 4 * -MathF.PI / 2, 0, 0);

            var blockSizeForRotation = new Vec2(blockSize.X - 1, blockSize.Y - 1);

            float xOffset;
            float zOffset;

            switch (block.Direction)
            {
                case Direction.North:
                    xOffset = posOffset.X;
                    zOffset = posOffset.Z;
                    break;
                case Direction.East:
                    xOffset = -posOffset.Z + blockSizeForRotation.Y;
                    zOffset = posOffset.X;
                    break;
                case Direction.South:
                    xOffset = -posOffset.X + blockSizeForRotation.X;
                    zOffset = -posOffset.Z + blockSizeForRotation.Y;
                    break;
                case Direction.West:
                    xOffset = posOffset.Z;
                    zOffset = -posOffset.X + blockSizeForRotation.X;
                    break;
                default:
                    throw new Exception();
            }

            absolutePosition += (xOffset, 0, zOffset) * map.GridSize;

            map.Challenge.PlaceAnchoredObject(ident, absolutePosition, pitchYawRoll);
        }

        if (blockData.Children is not null)
        {
            var randomBlocks = default(List<BlockRandomData>);

            foreach (var b in blockData.Children.OfType<BlockRandomData>())
            {
                randomBlocks ??= [];
                randomBlocks.Add(b);
            }

            if (randomBlocks is not null)
            {
                var b = randomBlocks[Random.Shared.Next(randomBlocks.Count)];

                PlaceItem(map, block, b, blockSize, posOffset, blockData.RotOffset + rotOffset, smallYOffset);

                return;
            }

            var skinBlocks = default(List<BlockSkinData>);

            foreach (var b in blockData.Children.OfType<BlockSkinData>())
            {
                skinBlocks ??= [];
                skinBlocks.Add(b);
            }

            if (skinBlocks is not null)
            {
                var noSkin = skinBlocks.FirstOrDefault(x => x.SkinRegex is null) ?? skinBlocks.First();

                PlaceItem(map, block, noSkin, blockSize, posOffset, blockData.RotOffset + rotOffset, smallYOffset);

                return;
            }

            foreach (var b in blockData.Children)
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
                }

                PlaceItem(map, block, b, blockSize, posOffset, blockData.RotOffset + rotOffset, smallYOffset);
            }
        }
    }

    /*private virtual ItemInfo GetItemInfo(Identifier identifier)
    {
        if (!TestBlock(identifier))
            return null;

        ItemInfo result = null;
        if (childrenList.Count == 0)
            result = new ItemInfo(ItemName, ItemAuthor, RotOffset, (XOffset, YOffset, ZOffset));
        else
        {
            foreach (var child in Children)
            {
                result = child.GetItemInfo(identifier);
                if (result != null)
                {
                    result.ItemAuthor ??= ItemAuthor;
                    result.RotOffset += RotOffset;
                    result.Offset.x += XOffset;
                    result.Offset.y += YOffset;
                    result.Offset.z += ZOffset;
                    break;
                }
            }
        }

        if (result != null)
        {
            if (Flags != null)
                result.Flags.AddRange(Flags);
            if (Clips != null)
                result.Clips.AddRange(Clips);
            if (MultiPylons != null)
            {
                foreach (var multipylon in MultiPylons)
                {
                    result.Pylons.AddRange(multipylon.GetPylons().AsEnumerable());
                }
            }
        }

        return result;
    }*/

    private IEnumerable<GBX.NET.Int3> GetCoveredCoords(Map map)
    {
        var blockUnits = new Dictionary<CGameCtnBlock, GBX.NET.Int3[]>();
        var zoneBlocks = new HashSet<CGameCtnBlock>();

        foreach (var block in map.Challenge.GetBlocks())
        {
            if (!blockDictionary.TryGetValue(block.Name, out var blockData))
            {
                continue;
            }

            var units = RecurseFlags(block, blockData)?
                .Where(x => x.Name == "NoGround")
                .Select(x => new GBX.NET.Int3(x.X, x.Y, x.Z))
                .ToArray() ?? [];

            if (units.Length != 0)
            {
                blockUnits[block] = units;
            }
        }

        foreach (var block in map.Challenge.GetBlocks())
        {
            if (!blockUnits.TryGetValue(block, out var units))
            {
                continue;
            }

            if (units.Length == 1)
            {
                yield return block.Coord with { Y = 0 }; // TODO wrong for flying blocks!
                continue;
            }

            var rotatedUnits = new GBX.NET.Int3[units.Length];

            // Determine minimum X and Z after rotation.
            var minX = int.MaxValue;
            var minZ = int.MaxValue;
            for (int i = 0; i < units.Length; i++)
            {
                var rotated = RotateUnit(units[i], block.Direction);
                rotatedUnits[i] = rotated;
                if (rotated.X < minX) minX = rotated.X;
                if (rotated.Z < minZ) minZ = rotated.Z;
            }

            // Adjust positions so the minimum X and Z become 0.
            foreach (var rotated in rotatedUnits)
            {
                yield return (block.Coord + new GBX.NET.Int3(rotated.X - minX, rotated.Y, rotated.Z - minZ)) with { Y = 0 }; // TODO wrong for flying blocks!
            }
        }
    }

    private static GBX.NET.Int3 RotateUnit(GBX.NET.Int3 unit, Direction direction) => direction switch
    {
        Direction.East => new GBX.NET.Int3(-unit.Z, unit.Y, unit.X),
        Direction.South => new GBX.NET.Int3(-unit.X, unit.Y, -unit.Z),
        Direction.West => new GBX.NET.Int3(unit.Z, unit.Y, -unit.X),
        _ => unit,
    };

    private static IEnumerable<Flag> RecurseFlags(CGameCtnBlock block, BlockToItem blockData)
    {
        foreach (var flag in blockData.Flags ?? [])
        {
            yield return flag;
        }

        foreach (var b in blockData.Children ?? [])
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
            }

            foreach (var flag in RecurseFlags(block, b))
            {
                yield return flag;
            }
        }
    }
}