using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormMono
{

    //That class is used by the editor to make it easier to move them on the map, will be converted back
    //to a EntranceOW on save :)
    public class EntranceOWEditor
    {
        public int x;
        public int y;
        public short mapPos;
        public byte entranceId;
        public short mapId;
        //mapId might be useless but we will need it to check if the entrance is in the darkworld or lightworld
        public EntranceOWEditor(int x, int y, byte entranceId, short mapId, short mapPos)
        {
            this.x = x;
            this.y = y;
            this.entranceId = entranceId;
            this.mapId = mapId;
            this.mapPos = mapPos;
            //

        }

        public void updateMapStuff(short mapId)
        {
            this.mapId = mapId;
            int mx = (mapId - ((mapId / 8) * 8));
            int my = ((mapId / 8));

            byte xx = (byte)((x - (mx * 512)) / 16);
            byte yy = (byte)((y - (my * 512)) / 16);

            mapPos = (short)((((yy) << 6) | (xx & 0x3F)) << 1);
            //Console.WriteLine(xx + ", " +yy+ ", " +mapPos);

        }

    }
}
