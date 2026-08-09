namespace _1toOneConverterOnline.Models.Settings.Conversion;

public sealed class PylonAddConversion : Conversion
{
    public Id Collection { get; init; }
    public Id DefaultAuthor { get; init; }

    public Id SinglePylon { get; init; } //Left One
    public Id DoublePylon { get; init; }
    public Id ForcedPylon { get; init; }

    public FlagName HeightFlag { get; init; }

    public override void Convert(Map map)
    {
        int itemCount = 0;

        var pylonMap = new SortedSet<Pylon>[65, 65, 4];

        foreach (var pylon in map.GetPylons(PylonType.Forced))
        {
            var pylonField = pylonMap[pylon.X, pylon.Z, pylon.Rot] ??= new SortedSet<Pylon>(Pylon.GetComparer());
            pylonField.Add(pylon);
        }

        foreach (var pylon in map.GetPylons(PylonType.Top))
        {
            var pylonField = pylonMap[pylon.X, pylon.Z, pylon.NormalizedRot] ??= new SortedSet<Pylon>(Pylon.GetComparer());
            pylonField.Add(pylon);
        }

        foreach (var pylon in map.GetPylons(PylonType.Bottom))
        {
            var pylonField = pylonMap[pylon.X, pylon.Z, pylon.NormalizedRot];
            pylonField?.Add(pylon);
        }

        foreach (var pylon in map.GetPylons(PylonType.Prevent))
        {
            var pylonField = pylonMap[pylon.X, pylon.Z, pylon.NormalizedRot];
            pylonField?.Add(pylon);
        }

        foreach (var pylonField in pylonMap)
        {
            if (pylonField != null)
            {
                bool hasPlacedForced = false;
                byte? topLeft = null, topRight = null;

                foreach (var pylonInfo in pylonField)
                {
                    if (pylonInfo.Type == PylonType.Forced)
                    {
                        if (!hasPlacedForced)
                        {
                            itemCount += PlaceForced(map, pylonInfo);
                            hasPlacedForced = true;
                        }
                        continue;
                    }

                    switch (pylonInfo.Type, pylonInfo.Pos)
                    {
                        case (PylonType.Prevent, PylonPosition.Both):
                            topLeft = topRight = null;
                            break;
                        case (PylonType.Prevent, PylonPosition.Left):
                            topLeft = null;
                            break;
                        case (PylonType.Prevent, PylonPosition.Right):
                            topLeft = null;
                            break;
                        case (PylonType.Top, PylonPosition.Both):
                            topLeft ??= (byte)pylonInfo.Y;
                            topRight ??= (byte)pylonInfo.Y;
                            break;
                        case (PylonType.Top, PylonPosition.Left):
                            topLeft ??= (byte)pylonInfo.Y;
                            break;
                        case (PylonType.Top, PylonPosition.Right):
                            topRight ??= (byte)pylonInfo.Y;
                            break;
                        case (PylonType.Bottom, PylonPosition.Both):
                            itemCount += PlaceBoth(map, pylonInfo, topLeft, topRight);
                            goto nextPylon;
                        case (PylonType.Bottom, PylonPosition.Left):
                            itemCount += PlaceLeft(map, pylonInfo, topLeft);
                            goto nextPylon;
                        case (PylonType.Bottom, PylonPosition.Right):
                            itemCount += PlaceRight(map, pylonInfo, topRight);
                            goto nextPylon;
                    }
                }

            nextPylon:;
            }
        }
    }

    private int PlaceForced(Map map, Pylon pylon)
    {
        var pos = ((byte)pylon.X, (byte)pylon.Z);
        byte y = (byte)pylon.Y;
        var groundHeight = (byte)map.BaseHeight;

        return PlaceForcedPylons(map, pos, (y, groundHeight), pylon.Rot, PylonPosition.Both);
    }

    public int PlaceBoth(Map file, Pylon bottom, byte? topLeft, byte? topRight)
    {
        int itemCount = 0;

        var pos = ((byte)bottom.X, (byte)bottom.Z);
        var rot = bottom.Rot;

        switch (topLeft, topRight)
        {
            case (byte topL, byte topR):
                if (topL == topR)
                {
                    itemCount += PlacePylons(file, pos, (topL, (byte)bottom.Y), rot, PylonPosition.Both);
                }
                else if (topL > topR)
                {
                    itemCount += PlacePylons(file, pos, (topL, topR), rot, PylonPosition.Left);
                    itemCount += PlacePylons(file, pos, (topR, (byte)bottom.Y), rot, PylonPosition.Both);
                }
                else
                {
                    itemCount += PlacePylons(file, pos, (topR, topL), rot, PylonPosition.Right);
                    itemCount += PlacePylons(file, pos, (topL, (byte)bottom.Y), rot, PylonPosition.Both);
                }
                break;
            case (byte topL, null):
                itemCount += PlacePylons(file, pos, (topL, (byte)bottom.Y), rot, PylonPosition.Left);
                break;
            case (null, byte topR):
                itemCount += PlacePylons(file, pos, (topR, (byte)bottom.Y), rot, PylonPosition.Right);
                break;
        }

        return itemCount;
    }

    internal int PlaceRight(Map map, Pylon bottom, byte? topRight)
    {
        var pos = ((byte)bottom.X, (byte)bottom.Z);
        var rot = bottom.Rot;

        if (topRight is byte top)
        {
            return PlacePylons(map, pos, (top, (byte)bottom.Y), rot, PylonPosition.Right);
        }

        return 0;
    }

    internal int PlaceLeft(Map map, Pylon bottom, byte? topLeft)
    {
        var pos = ((byte)bottom.X, (byte)bottom.Z);
        var rot = bottom.Rot;

        if (topLeft is byte top)
        {
            return PlacePylons(map, pos, (top, (byte)bottom.Y), rot, PylonPosition.Left);
        }

        return 0;
    }

    private int PlacePylons(Map map, (byte x, byte z) pos, (byte top, byte bottom) y, byte rot, PylonPosition pylonPos)
    {
        var itemName = pylonPos == PylonPosition.Both ? DoublePylon : SinglePylon;

        for (byte i = y.bottom; i < y.top; i++)
        {
            //Place Pylon
            var item = map.Challenge.PlaceAnchoredObject(
                new GBX.NET.Ident(itemName, Collection, DefaultAuthor),
                map.ConvertPylonCoords((pos.x, i, pos.z), rot),
                ConvertRot(rot, pylonPos));
            item.BlockUnitCoord = new GBX.NET.Byte3(pos.x, i, pos.z);
        }

        return y.top - y.bottom;
    }

    private int PlaceForcedPylons(Map map, (byte x, byte z) pos, (byte top, byte bottom) y, byte rot, PylonPosition pylonPos)
    {
        for (byte i = y.bottom; i < y.top; i++)
        {
            //Place Forced Pylon
            var item = map.Challenge.PlaceAnchoredObject(
                new GBX.NET.Ident(ForcedPylon, Collection, DefaultAuthor),
                map.ConvertCoords((pos.x, i, pos.z)),
                ConvertRot(rot));
            item.BlockUnitCoord = new GBX.NET.Byte3(pos.x, i, pos.z);
        }

        return y.top - y.bottom;
    }

    private static GBX.NET.Vec3 ConvertRot(byte rot, PylonPosition pylonPos)
    {
        if (pylonPos == PylonPosition.Left)
        {
            rot += 2;
            rot %= 4;
        }
        return ConvertRot(rot);
    }

    private const float Yaw0 = 0;
    private const float Yaw1 = -1.57079632679489f;
    private const float Yaw2 = -3.14159265358979f;
    private const float Yaw3 = 1.57079632679489f;
    private static GBX.NET.Vec3 ConvertRot(byte rot)
    {
        return rot switch
        {
            0 => new GBX.NET.Vec3(Yaw0, 0, 0),
            1 => new GBX.NET.Vec3(Yaw1, 0, 0),
            2 => new GBX.NET.Vec3(Yaw2, 0, 0),
            3 => new GBX.NET.Vec3(Yaw3, 0, 0),
            _ => throw new Exception(),
        };
    }
}