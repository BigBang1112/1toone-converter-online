using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public class MultiPylon
{
    [XmlAttribute]
    public PylonType Type { get; init; }

    [XmlAttribute]
    public PylonPosition Pos { get; init; }

    [XmlAttribute]
    public short X { get; init; }

    [XmlAttribute]
    public short Y { get; init; }

    [XmlAttribute]
    public short Z { get; init; }

    [XmlAttribute]
    public bool Optional { get; init; }

    [XmlAttribute]
    public MultiRot Rot { get; init; } = MultiRot.All;

    public IEnumerable<Pylon> GetPylons()
    {
        if ((Rot & MultiRot.Zero) != 0)
            yield return new Pylon { Pos = Pos, Type = Type, X = X, Y = Y, Z = Z, Rot = 0 };
        if ((Rot & MultiRot.One) != 0)
            yield return new Pylon { Pos = Pos, Type = Type, X = X, Y = Y, Z = Z, Rot = 1 };
        if ((Rot & MultiRot.Two) != 0)
            yield return new Pylon { Pos = Pos, Type = Type, X = X, Y = Y, Z = Z, Rot = 2 };
        if ((Rot & MultiRot.Three) != 0)
            yield return new Pylon { Pos = Pos, Type = Type, X = X, Y = Y, Z = Z, Rot = 3 };
    }

    public static MultiRot GetRot(byte rot) => (MultiRot)(1 << rot);
}