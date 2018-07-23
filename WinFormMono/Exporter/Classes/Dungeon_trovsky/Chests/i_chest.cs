/*
 * Class        :   i_chest.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System;
using ZScream_Exporter;

public class i_chest
{
    /*
     * B??? ???r    rrrr rrrr    iiii iiii
     * 
     * B = is big key
     * i = item
     * r = room
     */
    private ushort MYroom;
    private ushort room
    {
        get => MYroom;
        set
        {
            if (value <= Dungeon.maxRoomNo)
                MYroom = value;
            else throw new ArgumentOutOfRangeException();
        }
    }

    public byte item { get; set; }

    public i_chest(byte byte1, byte byte2, byte byte3)
    {
        room = getRoom(byte1, byte2);
        item = byte3;
    }

    internal static ushort getRoom(byte byte1, byte byte2)
    {
        ushort number = Conversion.toUShort(byte2, byte1);
        return (ushort)(number & 0x1FF);
    }

    public byte[] getBytes()
    {
        byte[] data = BitConverter.GetBytes(room);
        return new byte[] { data[1], data[0], item };
    }
}
