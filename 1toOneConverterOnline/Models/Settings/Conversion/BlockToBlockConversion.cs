using GBX.NET.Engines.Game;
using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class BlockToBlockConversion : Conversion
{
    [XmlElement("BlockToBlock")]
    public List<BlockToBlock>? BlockToBlocks { get; init; }

    public override void Convert(Map map)
    {
        if (map.Challenge.Blocks is null)
        {
            return;
        }

        foreach (var block in map.Challenge.Blocks)
        {
            /*string content = block.BlockName.Content;
            if (this._blockDict.ContainsKey(content))
            {
                BlockToBlock blockToBlock = this._blockDict[content];
                block.BlockName = new GBXLBS(blockToBlock.NewName);
                block.Coords.Y += (byte)(uint)blockToBlock.YOffset;
            }*/
        }
    }
}