/*
 * Author:  Zarby89
 */


/// <summary>
/// 
/// </summary>
/// 
namespace ZScream_Exporter
{
    public struct ExitOWWhirlpool
    {
        public short
            vramLocation,
            xScroll,
            yScroll,
            playerX,
            playerY,
            cameraX,
            cameraY,
            whirlpoolPos;
            
        public byte
            mapId,
            unk1,
            unk2;


        public ExitOWWhirlpool(byte mapId, short vramLocation, short yScroll, short xScroll, short playerY, short playerX, short cameraY, short cameraX, byte unk1, byte unk2,short whirlpoolPos)
        {
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
            this.whirlpoolPos = whirlpoolPos;
        }
    }
}