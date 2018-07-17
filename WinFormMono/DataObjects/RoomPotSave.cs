using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormMono
{
    public class RoomPotSave
    {
        public byte x, y, id;
        public bool bg2 = false;
        public ushort roomMapId;
        public RoomPotSave(byte id, ushort roomMapId, byte x, byte y, bool bg2)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.bg2 = bg2;
            this.roomMapId = roomMapId;
        }
    }
    public static class ItemsNames
    {
        public static string[] name = new string[]
{
                "Nothing","Rupee","RockCrab","Bee","Random","Bomb","Heart ","Blue Rupee",
                "Key","Arrow","Bomb ","Heart  ","Magic","Big Magic","Chicken","Green Soldier","AliveRock?","Blue Soldier",
                "Ground Bomb"," Heart","Fairy","Heart","Nothing ","Hole","Warp","Staircase","Bombable","Switch" };

    }
}