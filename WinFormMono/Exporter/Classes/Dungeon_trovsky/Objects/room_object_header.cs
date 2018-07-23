/*
 * Class        :   room_object_header.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System.Collections.Generic;

public class room_object_header
{
    public byte floor1 { get; set; }
    public byte floor2 { get; set; }
    public byte layout { get; set; }

    private const byte
        mask1 = 0x0F,
        layoutMask = 0x07,
        shift1 = 4,
        shift_layout = 2;

    public room_object_header()
    {
        layers = new layer[object_data.layerNo];
    }

    public void generateFromBytes(byte byte1, byte byte2)
    {
        floor1 = (byte)((byte1 >> shift1) & mask1);
        floor2 = (byte)(byte1 & mask1);
        layout = (byte)((byte2 >> shift_layout) & layoutMask);
    }

    public layer[] layers;
}

public struct layer
{
    public List<i_object_type1> type1;
    public List<i_object_type2> type2;
}
