/*
 * Class        :   i_item.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System;

public class i_item : LOZobject, ICloneable
{
    public const int gridbase = Dungeon_constants.BLOCKBASE;

    public const int maxItem = 76;

    private const int
        specialItem_mask = 0x80,
        item_mask = 0x7F,
        mask2 = 0xE,
        shift = 1;

    public byte itemID { get; set; }

    public bool isSpecialItem { get; set; }

    public new bool isLayer2
    {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
    }

    internal i_item(byte byte1, byte byte2, byte byte3)
    {
        generateCoordAndLayerFromBytes(byte1, byte2);

        isSpecialItem = Convert.ToBoolean(byte3 & specialItem_mask);

        if (!isSpecialItem)
            itemID = (byte)(byte3 & item_mask);
        else itemID = (byte)((byte3 & item_mask) >> shift);
    }

    public i_item(int x, int y, byte itemID, bool isSpecialItem)
    {
        rawX = (byte)x;
        rawY = (byte)y;
        this.itemID = (byte)(itemID & item_mask);
        this.isSpecialItem = isSpecialItem;
    }

    public object Clone()
    { return new i_item(rawX, rawY, itemID, isSpecialItem); }

    public byte[] getBytes()
    {
        throw new NotImplementedException();
        /*byte byte1, byte2;
        generateBytesFromCoordAndLayer(out byte1, out byte2);

        return new byte[]
        {
            byte1,
            byte2,
            itemID
        };*/
    }
}
