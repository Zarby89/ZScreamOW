﻿/*
 * Author:  Zarby89
 */

/// <summary>
/// 
/// </summary>
/// 
namespace ZScream_Exporter
{
    public struct ChestData
    {
        public bool bigChest;
        public byte itemIn;
        public ChestData(byte itemIn, bool bigChest)
        {
            this.itemIn = itemIn;
            this.bigChest = bigChest;
        }
    }
}