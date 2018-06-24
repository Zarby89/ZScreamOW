using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormMono
{
    public class MapSave
    {
        public ushort[,] tiles = new ushort[32, 32]; //all map tiles (short values) 0 to 1024 from left to right
        public bool largeMap = false;
        public byte spriteset = 0;
        public short index = 0;
        public byte palette = 0;
        public byte sprite_palette = 0;
        public byte blockset = 0;
        public string name = "";
        public short msgid;
        public Room_Sprite[] sprites;
        public RoomPotSave[] items;
        public MapSave()
        {


        }
    }
}
