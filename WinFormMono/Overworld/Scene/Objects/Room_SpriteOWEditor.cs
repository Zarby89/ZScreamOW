/*
 * Author:  Zarby89
 */

using System;
/// <summary>
/// 
/// </summary>
public class Room_SpriteOWEditor
{
    public int x, y;
    public byte gameX, gameY;
    public byte subtype, id;
    public byte overlord;
    public string name;
    public ushort roomMapId;
    public Room_SpriteOWEditor(byte id, int x, int y, ushort roomMapId, byte overlord, byte subtype)
    {
        this.id = id;
        this.x = x;
        this.y = y;
        this.name = Sprites_Names.name[id];
        this.overlord = overlord;
        this.subtype = subtype;
        this.roomMapId = roomMapId;
    }

    public void updateMapStuff(short mapId)
    {
        this.roomMapId = (ushort)mapId;
        int mx = (mapId - ((mapId / 8) * 8));
        int my = ((mapId / 8));

        gameX = (byte)((x - (mx * 512)) / 16);
        gameY = (byte)((y - (my * 512)) / 16);
        //Console.WriteLine("X:{0} Y:{1}", gameX, gameY);
            
    }
}
