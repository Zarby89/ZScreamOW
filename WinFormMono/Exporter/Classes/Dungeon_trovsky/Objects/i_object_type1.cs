/*
 * Class        :   i_object_type1.cs
 * Author       :   Zarby89, Trovsky
 * Description  :   
 */

using System;

public sealed class i_object_type1
{
    private int subtypeNo;
    public ushort oid { get; set; }
    public byte posX { get; set; }
    public byte posY { get; set; }
    public byte sizeX { get; set; }
    public byte sizeY { get; set; }
    public byte sizeXY { get; set; }

    private byte posMask = 0x3F;
    public i_object_type1(byte b1, byte b2, byte b3)
    {
        //Code mostly by Zarby 

        if (b3 >= 0xF8) //subtype3 (scalable x,y) //  && b3 < 0xFC
            subtypeNo = 3;
        else if (b1 >= 0xFC)
            subtypeNo = 2;
        else subtypeNo = 1;

        switch (subtypeNo)
        {
            case 1:
                //3rd byte = object
                //yyyy yycc xxxx xxaa
                oid = b3;
                posX = (byte)((b1 & 0xFC) >> 2);
                posY = (byte)((b2 & 0xFC) >> 2);
                sizeX = (byte)((b1 & 0x03));
                sizeY = (byte)((b2 & 0x03));
                sizeXY = (byte)(((sizeX << 2) + sizeY));
                break;
            case 2:
                //subtype2 (not scalable? )
                oid = (ushort)((b3 & 0x3F) + 0x100);

                posX = (byte)(((b2 & 0xF0) >> 4) + ((b1 & 0x3) << 4));
                posY = (byte)(((b2 & 0x0F) << 2) + ((b3 & 0xC0) >> 6));

                sizeX = sizeY = sizeXY = 0;
                break;
            case 3:
                //subtype3 (scalable x,y) //  && b3 < 0xFC
                oid = (ushort)((b3 << 4) | 0x80 + (((b2 & 0x03) << 2) + ((b1 & 0x03))));
                posX = (byte)((b1 >> 2) & posMask);
                posY = (byte)((b2 >> 2) & posMask);
                sizeX = (byte)((b1 & 0x03));
                sizeY = (byte)((b2 & 0x03));
                sizeXY = (byte)(((sizeX << 2) + sizeY));
                break;
            default:
                throw new Exception();
        }
    }
}
