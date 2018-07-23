/*
 * Class        :   i_block.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System;
using ZScream_Exporter;

public class i_block : LOZobject, ICloneable
{
    public const int gridbase = Dungeon_constants.BLOCKBASE;

    public i_block(ushort room, int x, int y)
    {
        this.room = room;
        rawX = (byte)x;
        rawY = (byte)y;
    }

    internal i_block(byte byte1, byte byte2, byte byte3, byte byte4)
    {
        room = Conversion.toUShort(byte2, byte1);

        if (room > Dungeon.maxRoomNo)
            throw new ArgumentOutOfRangeException();
        else generateCoordAndLayerFromBytes(byte3, byte4);
    }

    internal void getBytes(out byte[] blockData)
    {
        const byte max = 0x3F;
        if (rawX > max && rawY > max)
            throw new Exception();

        byte byte3, byte4;
        generateBytesFromCoordAndLayer(out byte3, out byte4);

        byte[] temp = BitConverter.GetBytes(room);
        blockData = new byte[] { temp[0], temp[1], byte3, byte4 };
    }

    internal static bool isNulledOut(byte byte1, byte byte2)
    { return Conversion.toUShort(byte2, byte1) == Dungeon.nullRoomVal; }

    public object Clone()
    { return new i_block(room, rawX, rawY); }
}