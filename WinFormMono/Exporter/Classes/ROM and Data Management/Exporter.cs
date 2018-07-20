﻿/*
 * Author: Zarby89
 */

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
/// <summary>
/// 
/// </summary>
/// 
namespace ZScream_Exporter
{
    public class Exporter
    {

        private RichTextBox logTextbox;
        private ProgressBar progressBar;
        private Overworld overworld = new Overworld();
        private byte[] romData;
        string path;
        string rompath;
        public Exporter(byte[] romData, string path,string rompath)
        {
            this.path = path;
            this.rompath = rompath;
            this.romData = romData;
            ROM.DATA = romData;
            ROMStructure.loadDefaultProject();
            Export();
        }

        public RoomSave[] all_rooms = new RoomSave[296];
        public MapSave[] all_maps = new MapSave[160];

        public void Export()
        {
            RegionId.GenerateRegion();
            ConstantsReader.SetupRegion(RegionId.myRegion, "../../");

            


            all_rooms = new RoomSave[296];
            all_maps = new MapSave[160];
            CheckGameTitle();
            LoadDungeonsRooms();
            LoadOverworldTiles();
            LoadOverworldMaps();

            TextData.readAllText();
            LoadedProjectStatistics.texts = TextData.messages.Count;
            SaveJson s = new SaveJson(path,rompath,all_rooms, all_maps, null, TextData.messages.ToArray(), overworld);
        }

        public void LoadDungeonsRooms()
        {
            int objCount = 0,
                chestCount = 0,
                itemCount = 0,
                blockCount = 0,
                torchCount = 0,
                pitsCount = 0,
                spritesCount = 0,
                roomCount = 0;

            for (int i = 0; i < 296; i++)
            {
                try
                {
                    all_rooms[i] = new RoomSave((short)i);
                    objCount += all_rooms[i].tilesObjects.Count;
                    chestCount += all_rooms[i].chest_list.Count;
                    itemCount += all_rooms[i].pot_items.Count;
                    blockCount += all_rooms[i].blocks.Count;
                    torchCount += all_rooms[i].torches.Count;
                    pitsCount += all_rooms[i].damagepit ? 1 : 0;
                    spritesCount += all_rooms[i].sprites.Count;
                    if (all_rooms[i].tilesObjects.Count != 0)
                    {
                        roomCount++;
                    }
                }
                catch (Exception e)
                {
                    //WriteLog("Error : " + e.Message.ToString(), Color.Red);
                    return;
                }
            }/*
            LoadedProjectStatistics.blocksRooms = blockCount;
            LoadedProjectStatistics.chestsRooms = chestCount;
            LoadedProjectStatistics.chestsRoomsLength = ((ROM.DATA[ConstantsReader.GetAddress("chests_length_pointer") + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("chests_length_pointer")])) / 3;
            LoadedProjectStatistics.blocksRoomsLength = ((short)((ROM.DATA[ConstantsReader.GetAddress("blocks_length") + 1] << 8) + ROM.DATA[ConstantsReader.GetAddress("blocks_length")])) / 4;
            LoadedProjectStatistics.torchesRoomsLength = 86;//(ROM.DATA[ConstantsReader.GetAddress("torches_length_pointer + 1] << 8) + ROM.DATA[ConstantsReader.GetAddress("torches_length_pointer];
            LoadedProjectStatistics.entrancesRooms = 132;
            LoadedProjectStatistics.itemsRooms = itemCount;
            LoadedProjectStatistics.pitsRooms = pitsCount;
            LoadedProjectStatistics.pitsRoomsLength = (ROM.DATA[ConstantsReader.GetAddress("pit_count")] / 2);
            LoadedProjectStatistics.torchesRooms = torchCount;
            LoadedProjectStatistics.usedRooms = roomCount;
            LoadedProjectStatistics.spritesRooms = spritesCount;
            LoadedProjectStatistics.objectsRooms = objCount;
            WriteLog("All dungeon rooms data loaded properly : ", Color.Green);*/
        }

        public void LoadOverworldTiles()
        {
            try
            {
                GFX.gfxdata = Compression.DecompressTiles();
                overworld.AssembleMap16Tiles();
                overworld.AssembleMap32Tiles();

                overworld.DecompressAllMapTiles(); //need to change
                overworld.createMap32TilesFrom16(); //need to change
                //WriteLog("Overworld tiles data loaded properly", Color.Green);
            }
            catch (Exception e)
            {
                //WriteLog("Error : " + e.Message.ToString(), Color.Red);
            }
        }

        public void LoadOverworldMaps()
        {
            ushort[,] unusedTiles = new ushort[32, 32];
            int mapCount = 0;
            for (int i = 0; i < 160; i++)
            {
                try
                {
                    all_maps[i] = new MapSave((short)i, overworld);
                    //TODO: Remove that and find a way to compare the tiles arrays
                    if (i >= 131 && i <= 146)
                        continue;
                    mapCount++;

                }
                catch (Exception e)
                {
                    //WriteLog("Error : " + e.Message.ToString(), Color.Red);
                    return;
                }
            }
            LoadedProjectStatistics.usedMaps = mapCount;
            LoadedProjectStatistics.tiles32Maps = overworld.tiles32count;
            LoadedProjectStatistics.itemsMaps = -1;
            LoadedProjectStatistics.spritesMaps = -1;
            LoadedProjectStatistics.overlaysMaps = -1;
            LoadedProjectStatistics.entrancesMaps = -1;
            LoadedProjectStatistics.exitsMaps = -1;
            LoadedProjectStatistics.holesMaps = -1;
            LoadedProjectStatistics.whirlpoolMaps = -1;
            //WriteLog("Overworld maps data loaded properly", Color.Green);
        }

        public void CheckGameTitle()
        {
            RegionId.GenerateRegion();

            string output = "";
            switch (RegionId.myRegion)
            {
                case (int)RegionId.Region.Japan:
                    output = "Japan";
                    goto PrintRegion;
                case (int)RegionId.Region.USA:
                    output = "US";
                    goto PrintRegion;
                case (int)RegionId.Region.German:
                    output = "German";
                    goto PrintRegion;
                case (int)RegionId.Region.France:
                    output = "France";
                    goto PrintRegion;
                case (int)RegionId.Region.Europe:
                    output = "Europe";
                    goto PrintRegion;
                case (int)RegionId.Region.Canada:
                    output = "Canada";
                    goto PrintRegion;
                default:
                   // WriteLog("Unknown Game Title : Using US as default", Color.Orange);
                    break;

                    PrintRegion:
                    //WriteLog("Region Detected : " + output, Color.Green);
                    break;
            }
        }

        public void WriteLog(string line, Color col, FontStyle fs = FontStyle.Regular)
        {
            Font f = new Font(logTextbox.Font, fs);
            string text = line + "\r\n";
            logTextbox.AppendText(text);
            logTextbox.Select((logTextbox.Text.Length - text.Length) + 1, text.Length);
            logTextbox.SelectionColor = col;
            logTextbox.SelectionFont = f;
            logTextbox.Refresh();
        }
    }
}