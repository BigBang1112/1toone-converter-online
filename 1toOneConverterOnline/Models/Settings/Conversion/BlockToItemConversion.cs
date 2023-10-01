using GBX.NET;
using GBX.NET.Engines.Game;
using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class BlockToItemConversion : Conversion
{
    private Dictionary<string, BlockData>? blockDictionary;

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

        if (map.Challenge.Blocks is null)
        {
            throw new Exception("Blocks == null");
        }

        if (Blocks is null)
        {
            return;
        }

        blockDictionary = new Dictionary<string, BlockData>();

        foreach (var block in Blocks)
        {
            if (string.IsNullOrEmpty(block.BlockName))
            {
                continue;
            }

            _ = blockDictionary.TryAdd(block.BlockName, block);
        }

        foreach (var block in map.Challenge.Blocks)
        {
            /*if (BlockIgnoreFlags != null)
            {
                foreach (var blockIgnoreFlag in BlockIgnoreFlags)
                {
                    if (file.TestFlag(blockIgnoreFlag.Name, block.Coords.X, block.Coords.Z))
                        goto nextBlock; // D: Goto?!? What a maniac.
                }
            }*/


            //BlockIgnoreFlags.Any(x => x.Name == block.Flags);

            if (ConvertBlock(map, block))
            {
                itemCount++;
            }
        }
    }

    private bool ConvertBlock(Map map, CGameCtnBlock block)
    {
        if (blockDictionary is null)
        {
            return false;
        }

        if (!blockDictionary.TryGetValue(block.Name, out BlockData? blockData))
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

    private void PlaceItem(Map map, CGameCtnBlock block, BlockToItem blockData, Int2 blockSize, GBX.NET.Int3 posOffset = default, int rotOffset = default)
    {
        posOffset = new GBX.NET.Int3(blockData.XOffset, blockData.YOffset, blockData.ZOffset) + posOffset;

        if (blockData.ItemName is not null)
        {
            var ident = new GBX.NET.Ident(blockData.ItemName,
                Collection ?? throw new Exception($"{nameof(BlockToItemConversion)}: Collection missing."),
                blockData.ItemAuthor ?? DefaultAuthor ?? "");

            var absolutePosition = (block.Coord + posOffset) * map.GridSize + map.GridOffset + (0, blockData.SmallYOffset, 0);
            var pitchYawRoll = new GBX.NET.Vec3(((int)block.Direction + blockData.RotOffset + rotOffset) % 4 * -MathF.PI / 2, 0, 0);

            var blockSizeForRotation = new Vec2(blockSize.X - 1, blockSize.Y - 1);

            switch (block.Direction)
            {
                case Direction.East:
                    absolutePosition -= (blockSizeForRotation.Y * map.GridSize.X, 0, 0);
                    break;
                case Direction.South:
                    absolutePosition += (blockSizeForRotation.X * map.GridSize.X, 0, blockSizeForRotation.Y * map.GridSize.Z);
                    break;
                case Direction.West:
                    absolutePosition += (0, 0, blockSizeForRotation.X * map.GridSize.X);
                    break;
            }

            map.Challenge.PlaceAnchoredObject(ident, absolutePosition, pitchYawRoll);
        }

        if (blockData.Children is not null)
        {
            var randomBlocks = default(List<BlockRandomData>);

            foreach (var b in blockData.Children.OfType<BlockRandomData>())
            {
                randomBlocks ??= new();
                randomBlocks.Add(b);
            }

            if (randomBlocks is not null)
            {
                var b = randomBlocks[Random.Shared.Next(randomBlocks.Count)];

                PlaceItem(map, block, b, blockSize, posOffset, blockData.RotOffset + rotOffset);

                return;
            }

            foreach (var b in blockData.Children)
            {
                if (b is BlockVariantData variantData && variantData.Variant != block.Variant)
                {
                    continue;
                }

                PlaceItem(map, block, b, blockSize, posOffset, blockData.RotOffset + rotOffset);
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
}