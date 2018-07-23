using System;
public abstract class LOZobject
{
    private ushort myRoom;
    public bool isLayer2;


    /* byte 1                           byte 2
     * -- -- l1 y6   y5 y4 y3 y2        y1 x6 x5 x4   x3 x2 x1 --
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
    protected void generateCoordAndLayerFromBytes(byte byte1, byte byte2)
    {
        rawX = (byte)((byte1 & 0x7E) >> 1);
        rawY = (byte)(((byte2 & 0x1F) << 1) | ((byte1 & 0x88) >> 7));
        isLayer2 = (((byte2 & 0x20) == 0x00) ? false : true);
    }

    protected void generateBytesFromCoordAndLayer(out byte byte1, out byte byte2)
    {
        byte1 = (byte)(((rawX << 1) & 0x7E) | ((rawY << 7) & 0x80));
        byte2 = (byte)(((rawY >> 1) & 0x1F) | (Convert.ToByte(isLayer2) << 5));
    }

    public byte rawX { get; set; }
    public byte rawY { get; set; }
    public virtual ushort room
    {
        get { return myRoom; }
        protected set
        {
            if (value <= Dungeon.maxRoomNo)
                myRoom = value;
            else throw new ArgumentOutOfRangeException();
        }
    }
}
