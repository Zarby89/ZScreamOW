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
        public RoomPotSave(byte id, byte x, byte y, bool bg2)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.bg2 = bg2;
        }
    }
}
