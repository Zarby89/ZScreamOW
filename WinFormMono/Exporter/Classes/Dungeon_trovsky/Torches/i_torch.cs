/*
 * Class        :   i_torch.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System;
public class i_torch : LOZobject, ICloneable
{
    public const int gridbase = Dungeon_constants.BLOCKBASE;

    internal i_torch(byte byte1, byte byte2)
    { generateCoordAndLayerFromBytes(byte1, byte2); }

    public i_torch(int x, int y, bool isLayer2)
    {
        rawX = (byte)x;
        rawY = (byte)y;
        this.isLayer2 = isLayer2;
    }

    internal void getBytes(out byte byte1, out byte byte2)
    { generateBytesFromCoordAndLayer(out byte1, out byte2); }

    public override ushort room
    {
        get { throw new NotSupportedException(); }
        protected set { throw new NotSupportedException(); }
    }

    public object Clone()
    { return new i_torch(rawX, rawY, isLayer2); }
}
