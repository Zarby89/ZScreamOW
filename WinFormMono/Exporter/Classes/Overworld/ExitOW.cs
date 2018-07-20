/*
 * Author:  Zarby89
 */


/// <summary>
/// 
/// </summary>
/// 
namespace ZScream_Exporter
{
    public struct ExitOW
    {
        public short
            roomId,
            vramLocation,
            xScroll,
            yScroll,
            playerX,
            playerY,
            cameraX,
            cameraY,
            doorType1,
            doorType2;

        public byte
            mapId,
            unk1,
            unk2;


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
        }
    }
}