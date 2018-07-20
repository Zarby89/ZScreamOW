/*
 * Author:  Zarby89
 */


namespace WinFormMono
{
    /// <summary>
    /// 
    /// </summary>
    public class ExitOW
    {

        public byte
            mapId,
            unk1,
            unk2,
            doorXEditor,
            doorYEditor;

        public short
            vramLocation,
            roomId,
            xScroll,
            yScroll,
            playerX,
            playerY,
            cameraX,
            cameraY,
            doorType1,
            doorType2;



        public bool isAutomatic = true;


        public ExitOW(short roomId, byte mapId, short vramLocation, short yScroll, short xScroll, short playerY, short playerX, short cameraY, short cameraX, byte unk1, byte unk2, short doorType1, short doorType2)
        {
            this.roomId = roomId;
            this.mapId = mapId;
            this.vramLocation = vramLocation;
            this.xScroll = xScroll;
            this.yScroll = yScroll;
            this.playerX = playerX;
            this.playerY = playerY;
            this.cameraX = cameraX;
            this.cameraY = cameraY;
            this.unk1 = unk1;
            this.unk2 = unk2;
            this.doorType1 = doorType1;
            this.doorType2 = doorType2;

            if (doorType1 != 0)
            {
                int p = (doorType1 & 0x7FFF) >> 1;
                doorXEditor = (byte)(p % 64);
                doorYEditor = (byte)(p >> 6);
            }
            if (doorType2 != 0)
            {
                int p = (doorType2 & 0x7FFF) >> 1;
                doorXEditor = (byte)(p % 64);
                doorYEditor = (byte)(p >> 6);
            }

        }

        public void updateMapStuff(byte mapId, JsonData jsonData, Map16[] allmaps)
        {
            this.mapId = mapId;

            int large = 256;
            int mapid = mapId;
            if (mapId < 128)
            {
                large = jsonData.mapdata[mapId].largeMap ? 768 : 256;
                if (allmaps[mapId].parentMapId != mapId)
                {
                    mapid = allmaps[mapId].parentMapId;
                }
            }

            //if map is large, large = 768, otherwise 256

            //mapx, mapy = "super map" position on the grid *512
            int mapx = (mapId & 7) << 9;
            int mapy = ((mapId & 56) << 6);
            if (isAutomatic)
            {
                xScroll = (short)(playerX - 134);
                yScroll = (short)(playerY - 78);

                if (xScroll < mapx) { xScroll = (short)((mapx)); }
                if (yScroll < mapy) { yScroll = (short)((mapy)); }

                if (xScroll > mapx + large) { xScroll = (short)((mapx) + large); }
                if (yScroll > (mapy + large) + 30) { yScroll = (short)(((mapy) + large) + 30); }

                cameraX = (short)(playerX);
                cameraY = (short)(playerY + 19);

                if (cameraX < mapx + 127) { cameraX = (short)(mapx + 127); }
                if (cameraY < mapy + 111) { cameraY = (short)(mapy + 111); }

                if (cameraX > mapx + 127 + large) { cameraX = (short)(mapx + 127 + large); }
                if (cameraY > mapy + 143 + large) { cameraY = (short)(mapy + 143 + large); }

            }
            short vramXScroll = (short)(xScroll - mapx);
            short vramYScroll = (short)(yScroll - mapy);



            vramLocation = (short)(((vramYScroll & 0xFFF0) << 3) | ((vramXScroll & 0xFFF0) >> 3));

        }
    }
}