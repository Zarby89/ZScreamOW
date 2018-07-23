/*
 * Class        :   i_roomHeader.cs
 * Author       :   Trovsky
 * Description  :   Puzzledude's "VITAL DUNGEON HEX DATA"
 *                  used as reference
 */

using System;

public struct i_roomHeader
{
    /* The header in 14 bytes long. The headers are compressed
     * by overlapping the data on one another. In other words,
     * room header storing is obnoxious and if you're going to
     * expand one thing in your rom, do the room headers.
     */
    /*
     * ------ 14 bytes ------
     *      
     * byte00:
     *      aaab bb-l
     *      a = BG2 in Hyrule Magic
     *      b = Collision in Hyrule Magic
     *      - = Unused
     *      l = Lights out transition
     * byte01:
     *      --bb bbbb
     *      - = Unused
     *      b = ???
     *      
     *      Note: bbbbbb is transformed to bbbbbb00,
     *      thus making it a multiple of 4.
     * byte02:
     *      GFX # in Hyrule Magic
     * byte03:
     *      Value + 0x40 is the sprite number in Hyrule Magic
     * byte04:
     *      Effect in Hyrule Magic
     * byte05:
     *      Tag1 in Hyrule Magic
     * byte06:
     *      Tag2 in Hyrule Magic
     * byte07:
     *      aabbccdd
     *      
     *      ???
     * byte08:
     *      xxxx xxaa
     *      
     *      x = unused
     *      a = something
     * byte09:
     *      ???
     * byte10:
     *      ???
     * byte11:
     *      ???
     * byte12:
     *      ???
     * byte13:
     *      ???
     * byte14:
     *      ???
     */

    public byte
        BG2,
        Collision,
        GFXNumber,
        SpriteNumber,
        Tag1,
        Tag2,
        Effect;

    public exit_struct[] exit;
    public bool areLightsOut;
    private const byte spriteOffset = 0x40;
    internal const byte
        maxAndMask1 = 0x07,
        maxAndMask2 = 0x3F,
        maxAndMask3 = 0x03,
        maxAndMask4 = 0x01,
        numberOfPlanesAndExits = 5;

    internal i_roomHeader(byte[] b)
    {
        BG2 = (byte)((b[0] >> 5) & maxAndMask1);
        Collision = (byte)((b[0] >> 2) & maxAndMask1);
        areLightsOut = Convert.ToBoolean(b[0] & maxAndMask4);
        GFXNumber = b[2];

        //if (b[3] + spriteOffset > byte.MaxValue)
        //throw new Exception();

        SpriteNumber = (byte)(b[3] + spriteOffset);

        Effect = (byte)(b[4] & maxAndMask1);
        Tag1 = (byte)(b[5] & maxAndMask2);
        Tag2 = (byte)(b[6] & maxAndMask2);

        exit = new exit_struct[numberOfPlanesAndExits]
        {
            new exit_struct(b[0x9], (byte)((b[7] >> 6) & maxAndMask3)),
            new exit_struct(b[0xA], (byte)((b[7] >> 4) & maxAndMask3)),
            new exit_struct(b[0xB], (byte)((b[7] >> 2) & maxAndMask3)),
            new exit_struct(b[0xC], (byte)((b[7] >> 0) & maxAndMask3)),
            new exit_struct(b[0xD], (byte)((b[8] >> 0) & maxAndMask3))
        };
    }
}

public enum exitNames
{
    HoleOrWarp,
    Staircase1,
    Staircase2,
    Staircase3OrDoor1,
    Staircase4OrDoor2
}

public struct exit_struct
{
    public byte room, plane;
    public const byte maxValue = byte.MaxValue;

    internal exit_struct(byte room, byte plane)
    {
        if (plane <= i_roomHeader.maxAndMask3)
            this.plane = plane;
        else throw new ArgumentOutOfRangeException();
        this.room = room;
    }
}
