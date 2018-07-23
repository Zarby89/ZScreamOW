/*
 * Class        :   i_sprite.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System;
public class i_sprite : LOZobject, ICloneable
{
    public const int gridbase = Dungeon_constants.SPRITEBASE;

    public byte spriteId;
    public bool isSpriteSort { get; set; }
    public const int bytesPerEntry = 3;

    public i_sprite(bool isSpriteSort, byte spriteId, int x, int y, bool isLayer2)
    {
        this.isSpriteSort = isSpriteSort;
        this.spriteId = spriteId;
        rawX = (byte)x;
        rawY = (byte)y;
        this.isLayer2 = isLayer2;
    }

    /* byte 0                           byte 1
     * l1 -- -- y5   y4 y3 y2 y1        -- -- -- x5   x4 x3 x2 x1
     * 
     * * - = Unused bit
     * * x = X coordinate bit
     * * y = Y coordinate bit
     * * l = Layer number bit
     * 
     * Xcoordinate  = x6 x5 x4 x3 x2 x1
     * Ycoordinate  = y6 y5 y4 y3 y2 y1
     * layer        = l1
     */

    private const int
        mask1 = 0x80,
        mask2 = 0x01,
        maskCoord = 0x1F,
        shift = 7;

    internal i_sprite(byte byte1, byte byte2, byte byte3, byte byte4)
    {
        isSpriteSort = Convert.ToBoolean(byte1 & mask2);
        generateCoordAndLayerFromBytes(byte2, byte3);
        spriteId = byte4;
    }

    protected new void generateCoordAndLayerFromBytes(byte byte1, byte byte2)
    {
        isLayer2 = Convert.ToBoolean(byte1 & mask2 >> shift);
        rawY = (byte)(byte1 & maskCoord);
        rawX = (byte)(byte2 & maskCoord);
    }

    protected new void generateBytesFromCoordAndLayer(out byte byte1, out byte byte2)
    {
        byte1 = (byte)((Convert.ToByte(isLayer2) << shift) | rawY);
        byte2 = rawX;
    }


    internal bool Equals(i_sprite s)
    { return isLayer2 == s.isLayer2 && rawX == s.rawX && rawY == s.rawY && spriteId == s.spriteId; }

    internal byte[] getBytes(out byte byte1)
    {
        byte byte2, byte3;
        generateBytesFromCoordAndLayer(out byte2, out byte3);

        byte1 = Convert.ToByte(isSpriteSort);
        return new byte[]
        {
            byte2,
            byte3,
            spriteId
        };
    }

    public object Clone()
    { return new i_sprite(isSpriteSort, spriteId, rawX, rawY, isLayer2); }

}
