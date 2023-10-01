using System.Xml.Serialization;

namespace _1toOneConverterOnline.Models.Settings.Conversion;

public readonly struct Vec3
{
    [XmlAttribute]
    public float X { get; init; }

    [XmlAttribute]
    public float Y { get; init; }

    [XmlAttribute]
    public float Z { get; init; }

    public static implicit operator GBX.NET.Vec3(Vec3 vec3)
    {
        return new GBX.NET.Vec3(vec3.X, vec3.Y, vec3.Z);
    }
}