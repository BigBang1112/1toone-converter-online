using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public readonly struct Int3
{
    [XmlAttribute]
    public int X { get; init; }
    
    [XmlAttribute]
    public int Y { get; init; }

    [XmlAttribute]
    public int Z { get; init; }

    public static implicit operator GBX.NET.Int3(Int3 int3)
    {
        return new GBX.NET.Int3(int3.X, int3.Y, int3.Z);
    }

    public static implicit operator GBX.NET.Byte3(Int3 int3)
    {
        return new GBX.NET.Byte3((byte)int3.X, (byte)int3.Y, (byte)int3.Z);
    }
}