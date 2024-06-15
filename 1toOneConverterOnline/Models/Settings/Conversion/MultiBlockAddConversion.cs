using GBX.NET;
using GBX.NET.Engines.Game;
using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class MultiBlockAddConversion : Conversion
{
    public Block? NewBlock { get; init; }

    [XmlElement]
    public ElementValue<byte> XStep { get; init; }
    [XmlElement]
    public ElementValue<byte> YValue { get; init; }
    [XmlElement]
    public ElementValue<byte> ZStep { get; init; }

    public override void Convert(Map map)
    {
        map.Challenge.Chunks.Remove<CGameCtnChallenge.Chunk0304300F>();
        map.Challenge.Chunks.Remove<CGameCtnChallenge.Chunk03043013>();
        map.Challenge.Chunks.Create<CGameCtnChallenge.Chunk0304301F>().Version = 6;

        if (NewBlock is null || NewBlock.BlockName is null || map.Challenge.Blocks is null)
        {
            return;
        }

        for (var x = 1; x <= map.Challenge.Size.X; x += XStep.Value)
        {
            for (int z = 1; z <= map.Challenge.Size.Z; z += ZStep.Value)
            {
                map.Challenge.Blocks.Add(new CGameCtnBlock
                {
                    Name = NewBlock.BlockName,
                    Direction = (Direction)NewBlock.Rot.Value,
                    Coord = new(x, YValue.Value, z),
                    Flags = NewBlock.Flags.Value,
                    Author = NewBlock.Author?.Content
                });
            }
        }
    }
}