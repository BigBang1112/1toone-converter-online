using GBX.NET;
using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class Clip
{
    [XmlAttribute]
    public string? Name { get; init; }

    [XmlAttribute]
    public short X { get; init; }

    [XmlAttribute]
    public short Y { get; init; }

    [XmlAttribute]
    public short Z { get; init; }

    [XmlAttribute]
    public byte Rot { get; init; }

    public Clip GetRelativeToBlock(GBX.NET.Int3 coord, Direction rot)
    {
        coord += rot switch
        {
            Direction.North => new GBX.NET.Int3(X, Y, Z),
            Direction.East => new GBX.NET.Int3(-Z, Y, X),
            Direction.South => new GBX.NET.Int3(-X, Y, -Z),
            Direction.West => new GBX.NET.Int3(Z, Y, -X),
            _ => throw new Exception(),
        };
        var rotB = (byte)((Rot + (int)rot) % 4);

        return new Clip() { Name = Name, X = (short)coord.X, Y = (short)coord.Y, Z = (short)coord.Z, Rot = rotB };
    }

    public static GBX.NET.Int3 GetCoordsFacing(GBX.NET.Int3 coord, byte rot)
    {
        return rot switch
        {
            0 => (coord.X, coord.Y, coord.Z + 1),
            1 => (coord.X - 1, coord.Y, coord.Z),
            2 => (coord.X, coord.Y, coord.Z - 1),
            3 => (coord.X + 1, coord.Y, coord.Z),
            _ => throw new Exception()
        };
    }
}