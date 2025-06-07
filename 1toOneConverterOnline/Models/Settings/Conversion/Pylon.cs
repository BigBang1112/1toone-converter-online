using GBX.NET;
using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class Pylon : IEquatable<Pylon>
{
    [XmlAttribute]
    public PylonType Type;
    [XmlAttribute]
    public PylonPosition Pos;
    [XmlAttribute]
    public short X;
    [XmlAttribute]
    public short Y;
    [XmlAttribute]
    public short Z;
    [XmlAttribute]
    public byte Rot;
    [XmlAttribute]
    public bool Optional;

    internal byte NormalizedRot
    {
        get
        {
            return Rot switch
            {
                1 => 1,
                2 => 0,
                _ => throw new Exception()
            };
        }
    }

    public Pylon Normalize()
    {
        switch (Rot)
        {
            case 0:
                Z += 1;
                Rot = 2;
                break;
            case 1:
            case 2:
                return this;
            case 3:
                X += 1;
                Rot = 1;
                break;
            default:
                throw new Exception();
        }

        switch (Pos)
        {
            case PylonPosition.Left:
                Pos = PylonPosition.Right;
                break;
            case PylonPosition.Right:
                Pos = PylonPosition.Left;
                break;
        }

        return this;
    }

    public Pylon GetRelativeToBlock(GBX.NET.Int3 coord, Direction rot)
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

        return new Pylon() { Type = Type, Pos = Pos, X = (short)coord.X, Y = (short)coord.Y, Z = (short)coord.Z, Rot = rotB };
    }

    public static IComparer<Pylon> GetComparer() => new PylonComparer();

    class PylonComparer : IComparer<Pylon>
    {
        public int Compare(Pylon p1, Pylon p2)
        {
            //Primary criteria: Height, from top to bottom
            int result = p2.Y.CompareTo(p1.Y);

            //Seondary criteria: Type None -> Prevent -> Top -> Bottom
            if (result == 0)
                result = p1.Type.CompareTo(p2.Type);

            return result;
        }
    }

    public bool IsLeftPosition() => Pos != PylonPosition.Right;

    public bool IsRightPosition() => Pos != PylonPosition.Left;

    public override bool Equals(object obj)
    {
        return Equals(obj as Pylon);
    }

    public bool Equals(Pylon other)
    {
        return other != null &&
            Type == other.Type &&
            Pos == other.Pos &&
            X == other.X &&
            Y == other.Y &&
            Z == other.Z &&
            Rot == other.Rot;
    }

    public override int GetHashCode()
    {
        var hashCode = 334360292;
        hashCode = hashCode * -1521134295 + Type.GetHashCode();
        hashCode = hashCode * -1521134295 + Pos.GetHashCode();
        hashCode = hashCode * -1521134295 + X.GetHashCode();
        hashCode = hashCode * -1521134295 + Y.GetHashCode();
        hashCode = hashCode * -1521134295 + Z.GetHashCode();
        hashCode = hashCode * -1521134295 + Rot.GetHashCode();
        return hashCode;
    }

    public bool ShouldSerializePos() => Pos != PylonPosition.Both;
}
