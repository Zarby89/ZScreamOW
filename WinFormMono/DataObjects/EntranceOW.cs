﻿namespace WinFormMono
{

    public struct EntranceOW
    {
        public short
            mapPos,
            mapId;
        public byte entranceId;

        public EntranceOW(short mapId, short mapPos, byte entranceId)
        {
            this.mapPos = mapPos;
            this.mapId = mapId;
            this.entranceId = entranceId;
        }
    }
}