namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class BlockAddConversion : Conversion
{
    public List<Block>? ExtraBlocks { get; init; }

    public override void Convert(Map map)
    {
        if (ExtraBlocks is null)
        {
            return;
        }
        
        foreach (var block in ExtraBlocks)
        {
            //map.Blocks.Add(block);
        }
    }
}