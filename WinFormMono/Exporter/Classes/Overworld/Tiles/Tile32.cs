﻿/*
 * Author:  Zarby89
 */

/// <summary>
/// 
/// </summary>
/// 
namespace ZScream_Exporter
{
    public struct Tile32
    {
        //[0,1]
        //[2,3]
        public ushort tile0, tile1, tile2, tile3;
        public Tile32(ushort tile0, ushort tile1, ushort tile2, ushort tile3)
        {
            this.tile0 = tile0;
            this.tile1 = tile1;
            this.tile2 = tile2;
            this.tile3 = tile3;
        }
    }
}