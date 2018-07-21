/*
 * Author:  Zarby89
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using AsarCLR;
/// <summary>
/// 
/// </summary>
/// 
namespace ZScream_Exporter
{
    public class Importer
    {
        private RichTextBox logTextbox;
        private ProgressBar progressBar;
        private Overworld overworld = new Overworld();
        private byte[] romData;
        public int DataOffset = 0x000000;
        public string path = "";
        public Importer(string path, byte[] romData, string fname = "")
        {
            //this.logTextbox = logTextbox;
            //this.progressBar = progressBar;
            this.romData = romData;
            ROM.DATA = romData;
            ROMStructure.loadDefaultProject();
            this.path = path;

//#warning Test Code to remove on release;
            //TEST CODE
            //path = @"C:\Users\Adamo\Desktop\ProjectDirectory\\";
            //TEST CODE


            //In case the ROM already have data in the 2MB portion this could be changed to be in 3MB or 4MB
            //Will be in the config file later the Editor use 1MB portion you can chose which one ;)
            //IF RANDO public int DataOffset = 0x100000; 
            Import(fname);
        }

        public MapSave[] all_maps = new MapSave[160];
        public EntranceOW[] all_entrancesOW = new EntranceOW[129];
        public ExitOW[] all_exitsOW = new ExitOW[78];
        List<Room_Sprite>[] roomSpritesBeginning = new List<Room_Sprite>[64];
        List<Room_Sprite>[] roomSpritesZelda = new List<Room_Sprite>[143];
        List<Room_Sprite>[] roomSpritesAgahnim = new List<Room_Sprite>[143];
        public void Import(string fname = "")
        {
            RegionId.GenerateRegion();
            ConstantsReader.SetupRegion(RegionId.myRegion, "../../");

            all_maps = new MapSave[160];
            CheckGameTitle();
            LoadOverworldTiles();
            LoadOverworldEntrances();
            LoadOverworldExits();
            LoadOverworldSprites();
            loadOverworldTilesTypes();
            LoadOverworldItems();
            loadOverworldOverlays();
            //progressBar.Value = progressBar.Maximum;
            //WriteLog("All 'Overworld' data saved in ROM successfuly.", Color.Green, FontStyle.Bold);

            try
            {
                //GFX.gfxdata = Compression.DecompressTiles();
                //SaveFileDialog sf = new SaveFileDialog();
                // if (sf.ShowDialog() == DialogResult.OK)
                // {

                //}
                runAsar(path,fname);

            }
            catch (Exception e)
            {
                //WriteLog("Error : " + e.Message.ToString(), Color.Red);
                return;
            }
        }

        public void runAsar(string path, string fname = "")
        {
            string arg2 = "\\ASM\\Main.asm";

            Asar.init();
            

            if (fname == "")
            {
                Console.WriteLine("NoError = " + Asar.patch(path + arg2, ref ROM.DATA));
                Asarerror[] errors = Asar.geterrors();
                foreach (Asarerror e in errors)
                {
                    Console.WriteLine(e.Rawerrdata);
                }
                FileStream fs = new FileStream(path + "//TestROM//working.sfc", FileMode.OpenOrCreate, FileAccess.Write);
                fs.Write(ROM.DATA, 0, ROM.DATA.Length);
                fs.Close();
            }
            else
            {
                Console.WriteLine("NoError = " + Asar.patch(path + arg2, ref ROM.DATA));
                Asarerror[] errors = Asar.geterrors();
                foreach (Asarerror e in errors)
                {
                    Console.WriteLine(e.Rawerrdata);
                }
                Console.WriteLine("NoError "+fname);
                FileStream fs = new FileStream(fname, FileMode.OpenOrCreate, FileAccess.Write);
                fs.Write(ROM.DATA, 0, ROM.DATA.Length);
                fs.Close();
            }


            Asar.close();
            /*var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            //Do not create command propmpt window 
            process.StartInfo.CreateNoWindow = false;

            //Do not use shell execution
            process.StartInfo.UseShellExecute = false;

            //Redirects error and output of the process (command prompt).
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            //start a new process
            process.Start();
            string arg1 = "\"xkas.exe\"";
            string arg2 = "\"ASM\\Main.asm\"";
            string arg3 = "\"TestROM\\working.sfc\"";
            process.StandardInput.WriteLine(@"cd " + path);
            //WriteLog(@"cd " + path, Color.Black);
            process.StandardInput.WriteLine(arg1 + " " + arg2 + " " + arg3);
            //process.StandardInput.WriteLine(arg3);
            process.StandardInput.WriteLine("exit");
            //wait until process is running
            process.WaitForExit();

            //reads output and error of command prompt to string.
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            */

        }

        public byte[] getLargeMaps()
        {
            List<byte> largemaps = new List<byte>();
            for (int i = 0; i < 64; i++)
            {
                if (i > 0)
                    if (all_maps[i - 1].largeMap)
                        if (largemaps.Contains((byte)(i - 1)))
                            continue;

                if (i > 7)
                    if (all_maps[i - 8].largeMap)
                        if (largemaps.Contains((byte)(i - 8)))
                            continue;

                if (i > 8)
                    if (all_maps[i - 9].largeMap)
                        if (largemaps.Contains((byte)(i - 9)))
                            continue;

                if (all_maps[i].largeMap)
                    largemaps.Add((byte)i);
            }

            return largemaps.ToArray();
        }

        bool[] vanillaOverlays = new bool[128]
        {
            false,false,true,false,false,false,false,true,
            false,false,false,false,false,false,false,false,
            false,false,false,true,true,false,false,false,
            true,false,false,true,false,false,false,false,
            false,false,false,false,false,false,false,false,
            false,false,false,true,false,false,false,false,
            true,false,false,false,false,false,false,true,
            false,false,true,true,false,false,false,false,

            true,false,false,true,false,true,false,true,
            false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,
            true,false,false,true,false,false,true,false,
            false,false,true,false,false,false,false,false,
            false,false,false,true,false,false,false,false,
            true,false,false,false,false,false,false,true,
            false,false,false,true,false,false,false,true
        };

        public void loadOverworldOverlays()
        {
            //first one == 0xFF 0xFF for the empty
            int dataPos = ConstantsReader.GetAddress("overlayNewCode"); //empty pointer = overlayNewCode
            int emptyPos = ConstantsReader.GetAddress("overlayNewCode"); ;
            ROM.DATA[dataPos++] = 0xFF;
            ROM.DATA[dataPos++] = 0xFF;
            List<OverlayData>[] overlaysData = JsonConvert.DeserializeObject<List<OverlayData>[]>(File.ReadAllText(path + @"//Overworld//Overlays.json"));
            int pointeraddr = ConstantsReader.GetAddress("overlayPointers"); 
            for (int i = 0;i<128;i++)
            {

                if (vanillaOverlays[i] == true)
                {
                    int snesaddr = Addresses.pctosnes(dataPos);
                    ROM.DATA[pointeraddr + (i * 2) + 1] = (byte)((snesaddr >> 8) & 0xFF);
                    ROM.DATA[pointeraddr + (i * 2)] = (byte)((snesaddr) & 0xFF);
                    // Format: iiiipppp pppppppp iiiiiiii - p = position, i = tile id
                    foreach (OverlayData od in overlaysData[i])
                    {
                        short pos = (short)((((od.y * 64) + od.x)) << 1);
                        byte b1 = (byte)((pos & 0xFF00) >> 8);
                        byte b2 = (byte)((pos & 0x00FF));
                        byte b3 = (byte)((od.tileId & 0xFF00) >> 8);
                        byte b4 = (byte)(od.tileId & 0xFF);
                        ROM.DATA[dataPos++] = b2;
                        ROM.DATA[dataPos++] = b1;
                        ROM.DATA[dataPos++] = b4;
                        ROM.DATA[dataPos++] = b3;
                    }

                    ROM.DATA[dataPos++] = 0xFF;
                    ROM.DATA[dataPos++] = 0xFF;
                }
                else
                {
                    int snesaddr = Addresses.pctosnes(emptyPos);
                    ROM.DATA[pointeraddr + (i * 2) + 1] = (byte)((snesaddr >> 8) & 0xFF);
                    ROM.DATA[pointeraddr + (i * 2)] = (byte)((snesaddr) & 0xFF);
                }
            }


            
            /*ROM.DATA[pointeraddr + (i * 2) + 1] = (byte)((snesaddr >> 8) & 0xFF);
            ROM.DATA[pointeraddr + (i * 2)] = (byte)((snesaddr) & 0xFF);
            foreach (roomPotSave item in roomItems[i])
            {

                //Console.WriteLine(item.x);

                short mapPos = (short)(((item.y << 6) + item.x) << 1);

                byte b1 = (byte)((mapPos >> 8));//1111 1111 0000 0000
                byte b2 = (byte)(mapPos & 0xFF);//0000 0000 1111 1111
                byte b3 = (byte)(item.id);

                ROM.DATA[dataPos++] = b2;
                ROM.DATA[dataPos++] = b1;
                ROM.DATA[dataPos++] = b3;
            }
            ROM.DATA[dataPos++] = 0xFF;
            ROM.DATA[dataPos] = 0xFF;
            if (dataPos >= (0x1F2800 + DataOffset))
            {
                Console.WriteLine("Too many Overworld items !");
                break;
            }
            dataPos++;

        }
                else
                {
                    int snesaddr = Addresses.pctosnes(emptyPointer);
        ROM.DATA[pointeraddr + (i * 2) + 1] = (byte) ((snesaddr >> 8) & 0xFF);
                    ROM.DATA[pointeraddr + (i * 2)] = (byte) ((snesaddr) & 0xFF);
                    //Save Empty Pointer
                }*/
}

            public void LoadOverworldTiles()
        {
            overworld.AssembleMap16Tiles(true, path);
            int palettesOW_Addr = ConstantsReader.GetAddress("overworldMapPalette");
            int spritepalettesOW_Addr = ConstantsReader.GetAddress("overworldSpritePalette");
            int spritesetOW_Addr = ConstantsReader.GetAddress("overworldSpriteset");
            int mapsizeOW_Addr = ConstantsReader.GetAddress("overworldMapSize");
            int messageOW_Addr = ConstantsReader.GetAddress("overworldMessages");
            int blocksetOW_Addr = ConstantsReader.GetAddress("mapGfx");





            for (int i = 0; i < 160; i++)
            {
                all_maps[i] = JsonConvert.DeserializeObject<MapSave>(File.ReadAllText(path + @"//Overworld//Maps//Map" + i.ToString("D3") + ".json"));

                overworld.AllMapTilesFromMap(i, all_maps[i].tiles);
                if (i == 159)
                {
                    string s = "";
                    int tpos = 0;
                    for (int y = 0; y < 16; y++)
                    {
                        for (int x = 0; x < 16; x++)
                        {
                            Tile32 map16 = new Tile32(all_maps[i].tiles[(x * 2), (y * 2)], all_maps[i].tiles[(x * 2) + 1, (y * 2)], all_maps[i].tiles[(x * 2), (y * 2) + 1], all_maps[i].tiles[(x * 2) + 1, (y * 2) + 1]);
                            s += "[" + map16.tile0.ToString("D4") + "," + map16.tile1.ToString("D4") + "," + map16.tile2.ToString("D4") + "," + map16.tile3.ToString("D4") + "] ";
                            tpos++;
                        }
                        s += "\r\n";
                    }
                    //File.WriteAllText("TileDebug2.txt", s);
                }

            }

            for (int i = 0; i < 136; i++)
            {

                //Need to finish importing stuff here !
                ROM.DATA[(0x1EF800 + DataOffset) + i] = all_maps[i].tileTypeSet;


                ROM.DATA[palettesOW_Addr + i] = (byte)(all_maps[i].palette >> 2); // why the >> 2 ¯\_(ツ)_/¯
                ROM.DATA[blocksetOW_Addr + i] = all_maps[i].blockset;
                ROM.DATA[spritesetOW_Addr + i] = all_maps[i].spriteset;
                ROM.DATA[spritepalettesOW_Addr + i] = all_maps[i].sprite_palette;
                ROM.DATA[messageOW_Addr + (i * 2) + 1] = (byte)((all_maps[i].msgid >> 8) & 0xFF);
                ROM.DATA[messageOW_Addr + (i * 2)] = (byte)(all_maps[i].msgid & 0xFF);
            }
            //ROM.DATA[mapsizeOW_Addr + i] = (byte)(all_maps[i].largeMap?0:1);
            byte[] largemaps = getLargeMaps();
            //overworld.createMap32TilesFrom16();
            //overworld.savemapstorom();
            overworld.savemapstoromNEW(all_maps);

            //WriteLog("Overworld tiles data loaded properly", Color.Green);
        }

        public void loadOverworldTilesTypes()
        {
            byte[][] tilesTypes = JsonConvert.DeserializeObject<byte[][]>(File.ReadAllText(path + @"//Overworld//TilesTypes.json"));
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 512; j++)
                {
                    ROM.DATA[(0x1FC000 + DataOffset) + j + (i * 512)] = tilesTypes[i][j];
                }
            }

        }


        public void LoadOverworldItems()
        {
            roomPotSave[] items = JsonConvert.DeserializeObject<roomPotSave[]>(File.ReadAllText(path + @"//Overworld//Items//Items.json"));
            List<roomPotSave>[] roomItems = new List<roomPotSave>[128];

            for (int i = 0; i < 128; i++)
            {
                roomItems[i] = new List<roomPotSave>();
                for (int j = 0; j < items.Length; j++)
                {
                    if (i == items[j].roomMapId)
                    {
                        roomItems[i].Add(items[j]);
                    }
                }
            }

            ROM.DATA[(0x1F1201 + DataOffset)] = 0xFF; ROM.DATA[(0x1F1202 + DataOffset)] = 0xFF;
            short emptyPointer = 0x1201;
            int dataPos = (0x1F1203 + DataOffset);

            int pointeraddr = ConstantsReader.GetAddress("overworldItemsPointers");
            for (int i = 0; i < 128; i++)
            {
                if (roomItems[i].Count != 0)
                {
                    int snesaddr = Addresses.pctosnes(dataPos);
                    ROM.DATA[pointeraddr + (i * 2) + 1] = (byte)((snesaddr >> 8) & 0xFF);
                    ROM.DATA[pointeraddr + (i * 2)] = (byte)((snesaddr) & 0xFF);
                    foreach (roomPotSave item in roomItems[i])
                    {

                        //Console.WriteLine(item.x);

                        short mapPos = (short)(((item.y << 6) + item.x) << 1);

                        byte b1 = (byte)((mapPos >> 8));//1111 1111 0000 0000
                        byte b2 = (byte)(mapPos & 0xFF);//0000 0000 1111 1111
                        byte b3 = (byte)(item.id);

                        ROM.DATA[dataPos++] = b2;
                        ROM.DATA[dataPos++] = b1;
                        ROM.DATA[dataPos++] = b3;
                    }
                    ROM.DATA[dataPos++] = 0xFF;
                    ROM.DATA[dataPos] = 0xFF;
                    if (dataPos >= (0x1F2800 + DataOffset))
                    {
                        Console.WriteLine("Too many Overworld items !");
                        break;
                    }
                    dataPos++;

                }
                else
                {
                    int snesaddr = Addresses.pctosnes(emptyPointer);
                    ROM.DATA[pointeraddr + (i * 2) + 1] = (byte)((snesaddr >> 8) & 0xFF);
                    ROM.DATA[pointeraddr + (i * 2)] = (byte)((snesaddr) & 0xFF);
                    //Save Empty Pointer
                }


            }



        }

        public void LoadOverworldSprites()
        {

            Room_Sprite[][] all_spritesOW = new Room_Sprite[3][];
            all_spritesOW[0] = JsonConvert.DeserializeObject<Room_Sprite[]>(File.ReadAllText(path + @"//Overworld//Sprites//Beginning Sprites.json"));
            all_spritesOW[1] = JsonConvert.DeserializeObject<Room_Sprite[]>(File.ReadAllText(path + @"//Overworld//Sprites//ZeldaRescued Sprites.json"));
            all_spritesOW[2] = JsonConvert.DeserializeObject<Room_Sprite[]>(File.ReadAllText(path + @"//Overworld//Sprites//AgahnimDefeated Sprites.json"));

            //Restore sprites by room //64, 143
            for (int i = 0; i < 64; i++)
            {
                roomSpritesBeginning[i] = new List<Room_Sprite>();
                for (int j = 0; j < all_spritesOW[0].Length; j++)
                {
                    if (all_spritesOW[0][j].roomMapId == i)
                    {
                        roomSpritesBeginning[i].Add(all_spritesOW[0][j]);
                    }
                }
            }

            for (int i = 0; i < 143; i++)
            {
                roomSpritesZelda[i] = new List<Room_Sprite>();
                roomSpritesAgahnim[i] = new List<Room_Sprite>();

                for (int j = 0; j < all_spritesOW[1].Length; j++)
                {
                    if (all_spritesOW[1][j].roomMapId == i)
                    {
                        roomSpritesZelda[i].Add(all_spritesOW[1][j]);
                    }
                }

                for (int j = 0; j < all_spritesOW[2].Length; j++)
                {
                    if (all_spritesOW[2][j].roomMapId == i)
                    {
                        roomSpritesAgahnim[i].Add(all_spritesOW[2][j]);
                    }
                }
            }



            //data location range spritedata = AED length : 2797
            //0x4CB41 starting data location (1st is just a FF for empty pointers)
            //real starting location = 0x4CB42
            //ROM.DATA[0x4CB41] = 0xFF;//Ensure the first data is a return so maps with no sprites can use that
            ROM.DATA[(0x1F0001 + DataOffset)] = 0xFF; ROM.DATA[(0x1F0000 + DataOffset)] = 0xFF;
            int emptyPointer = 0x0000;

            int dataPos = (0x1F0002 + DataOffset);
            //int dataPos = 0x4CB42;
            int beginningPointers = ConstantsReader.GetAddress("overworldSpritesBegining");
            int zeldaPointers = ConstantsReader.GetAddress("overworldSpritesZelda");
            int agahnimPointers = ConstantsReader.GetAddress("overworldSpritesAgahnim");

            for (int i = 0; i < 64; i++)
            {
                if (roomSpritesBeginning[i].Count != 0)
                {
                    int snesaddr = Addresses.pctosnes(dataPos);
                    ROM.DATA[beginningPointers + (i * 2) + 1] = (byte)((snesaddr >> 8) & 0xFF);
                    ROM.DATA[beginningPointers + (i * 2)] = (byte)((snesaddr) & 0xFF);
                    foreach (Room_Sprite spr in roomSpritesBeginning[i])
                    {

                        byte b1 = spr.y;
                        byte b2 = spr.x;
                        byte b3 = spr.id;
                        ROM.DATA[dataPos++] = b1;
                        ROM.DATA[dataPos++] = b2;
                        ROM.DATA[dataPos++] = b3;
                    }
                    //add FF to end the room
                    ROM.DATA[dataPos++] = 0xFF;

                    if (dataPos >= ((0x1F1200 + DataOffset)))
                    {
                        Console.WriteLine("Too many Overworld sprites !");
                        break;
                    }
                }
                else
                {
                    int snesaddr = Addresses.pctosnes(emptyPointer);
                    ROM.DATA[beginningPointers + (i * 2) + 1] = (byte)((snesaddr >> 8) & 0xFF);
                    ROM.DATA[beginningPointers + (i * 2)] = (byte)((snesaddr) & 0xFF);
                }
            }

            for (int i = 0; i < 143; i++)
            {
                if (roomSpritesZelda[i].Count != 0)
                {
                    int snesaddr = Addresses.pctosnes(dataPos);
                    ROM.DATA[zeldaPointers + (i * 2) + 1] = (byte)((snesaddr >> 8) & 0xFF);
                    ROM.DATA[zeldaPointers + (i * 2)] = (byte)((snesaddr) & 0xFF);
                    foreach (Room_Sprite spr in roomSpritesZelda[i])
                    {

                        byte b1 = spr.y;
                        byte b2 = spr.x;
                        byte b3 = spr.id;
                        ROM.DATA[dataPos++] = b1;
                        ROM.DATA[dataPos++] = b2;
                        ROM.DATA[dataPos++] = b3;
                    }
                    //add FF to end the room
                    ROM.DATA[dataPos++] = 0xFF;

                    if (dataPos >= (0x1F1200 + DataOffset))
                    {
                        Console.WriteLine("Too many Overworld sprites ! (Zelda)");
                        break;
                    }
                }
                else
                {
                    int snesaddr = Addresses.pctosnes(emptyPointer);
                    ROM.DATA[zeldaPointers + (i * 2) + 1] = (byte)((snesaddr >> 8) & 0xFF);
                    ROM.DATA[zeldaPointers + (i * 2)] = (byte)((snesaddr) & 0xFF);
                }
            }


            for (int i = 0; i < 143; i++)
            {
                if (roomSpritesAgahnim[i].Count != 0)
                {

                    int snesaddr = Addresses.pctosnes(dataPos);
                    ROM.DATA[agahnimPointers + (i * 2) + 1] = (byte)((snesaddr >> 8) & 0xFF);
                    ROM.DATA[agahnimPointers + (i * 2)] = (byte)((snesaddr) & 0xFF);
                    foreach (Room_Sprite spr in roomSpritesAgahnim[i])
                    {

                        byte b1 = spr.y;
                        byte b2 = spr.x;
                        byte b3 = spr.id;
                        ROM.DATA[dataPos++] = b1;
                        ROM.DATA[dataPos++] = b2;
                        ROM.DATA[dataPos++] = b3;
                    }
                    //add FF to end the room
                    ROM.DATA[dataPos++] = 0xFF;

                    if (dataPos >= (0x1F1200 + DataOffset))
                    {
                        Console.WriteLine("Too many Overworld sprites ! (Agah) room : " + i);
                        break;
                    }
                }
                else
                {

                    int snesaddr = Addresses.pctosnes(emptyPointer);
                    ROM.DATA[agahnimPointers + (i * 2) + 1] = (byte)((snesaddr >> 8) & 0xFF);
                    ROM.DATA[agahnimPointers + (i * 2)] = (byte)((snesaddr) & 0xFF);
                }
            }
        }

        public void LoadOverworldEntrances()
        {
            EntranceOW[] holes = JsonConvert.DeserializeObject<EntranceOW[]>(File.ReadAllText(path + @"//Overworld//Entrances//Holes.json"));
            all_entrancesOW = JsonConvert.DeserializeObject<EntranceOW[]>(File.ReadAllText(path + @"//Overworld//Entrances//Entrances.json"));
            for (int i = 0; i < all_entrancesOW.Length; i++)
            {
                ROM.DATA[ConstantsReader.GetAddress("OWEntranceMap") + (i * 2) + 1] = (byte)((all_entrancesOW[i].mapId >> 8) & 0xFF);
                ROM.DATA[ConstantsReader.GetAddress("OWEntranceMap") + (i * 2)] = (byte)((all_entrancesOW[i].mapId) & 0xFF);

                ROM.DATA[ConstantsReader.GetAddress("OWEntrancePos") + (i * 2) + 1] = (byte)((all_entrancesOW[i].mapPos >> 8) & 0xFF);
                ROM.DATA[ConstantsReader.GetAddress("OWEntrancePos") + (i * 2)] = (byte)((all_entrancesOW[i].mapPos) & 0xFF);

                ROM.DATA[ConstantsReader.GetAddress("OWEntranceEntranceId") + i] = (byte)((all_entrancesOW[i].entranceId) & 0xFF);
            }

            for (int i = 0; i < holes.Length; i++)
            {

                ROM.DATA[ConstantsReader.GetAddress("OWHoleArea") + (i * 2) + 1] = (byte)((holes[i].mapId >> 8) & 0xFF);
                ROM.DATA[ConstantsReader.GetAddress("OWHoleArea") + (i * 2)] = (byte)((holes[i].mapId) & 0xFF);

                ROM.DATA[ConstantsReader.GetAddress("OWHolePos") + (i * 2) + 1] = (byte)((holes[i].mapPos >> 8) & 0xFF);
                ROM.DATA[ConstantsReader.GetAddress("OWHolePos") + (i * 2)] = (byte)((holes[i].mapPos) & 0xFF);

                ROM.DATA[ConstantsReader.GetAddress("OWHoleEntrance") + i] = (byte)((holes[i].entranceId) & 0xFF);
            }
            //WriteLog("Overworld Entrances data loaded properly", Color.Green);
        }

        public void LoadOverworldExits()
        {
            all_exitsOW = JsonConvert.DeserializeObject<ExitOW[]>(File.ReadAllText(path + @"//Overworld//Entrances//Exits.json"));
            for (int i = 0; i < 78; i++)
            {

                ROM.DATA[ConstantsReader.GetAddress("OWExitMapId") + (i)] = (byte)((all_exitsOW[i].mapId) & 0xFF);

                ROM.DATA[ConstantsReader.GetAddress("OWExitXScroll") + (i * 2) + 1] = (byte)((all_exitsOW[i].xScroll >> 8) & 0xFF);
                ROM.DATA[ConstantsReader.GetAddress("OWExitXScroll") + (i * 2)] = (byte)((all_exitsOW[i].xScroll) & 0xFF);

                ROM.DATA[ConstantsReader.GetAddress("OWExitYScroll") + (i * 2) + 1] = (byte)((all_exitsOW[i].yScroll >> 8) & 0xFF);
                ROM.DATA[ConstantsReader.GetAddress("OWExitYScroll") + (i * 2)] = (byte)((all_exitsOW[i].yScroll) & 0xFF);

                ROM.DATA[ConstantsReader.GetAddress("OWExitXCamera") + (i * 2) + 1] = (byte)((all_exitsOW[i].cameraX >> 8) & 0xFF);
                ROM.DATA[ConstantsReader.GetAddress("OWExitXCamera") + (i * 2)] = (byte)((all_exitsOW[i].cameraX) & 0xFF);

                ROM.DATA[ConstantsReader.GetAddress("OWExitYCamera") + (i * 2) + 1] = (byte)((all_exitsOW[i].cameraY >> 8) & 0xFF);
                ROM.DATA[ConstantsReader.GetAddress("OWExitYCamera") + (i * 2)] = (byte)((all_exitsOW[i].cameraY) & 0xFF);

                ROM.DATA[ConstantsReader.GetAddress("OWExitVram") + (i * 2) + 1] = (byte)((all_exitsOW[i].vramLocation >> 8) & 0xFF);
                ROM.DATA[ConstantsReader.GetAddress("OWExitVram") + (i * 2)] = (byte)((all_exitsOW[i].vramLocation) & 0xFF);

                ROM.DATA[ConstantsReader.GetAddress("OWExitRoomId") + (i * 2) + 1] = (byte)((all_exitsOW[i].roomId >> 8) & 0xFF);
                ROM.DATA[ConstantsReader.GetAddress("OWExitRoomId") + (i * 2)] = (byte)((all_exitsOW[i].roomId) & 0xFF);

                ROM.DATA[ConstantsReader.GetAddress("OWExitXPlayer") + (i * 2) + 1] = (byte)((all_exitsOW[i].playerX >> 8) & 0xFF);
                ROM.DATA[ConstantsReader.GetAddress("OWExitXPlayer") + (i * 2)] = (byte)((all_exitsOW[i].playerX) & 0xFF);

                ROM.DATA[ConstantsReader.GetAddress("OWExitYPlayer") + (i * 2) + 1] = (byte)((all_exitsOW[i].playerY >> 8) & 0xFF);
                ROM.DATA[ConstantsReader.GetAddress("OWExitYPlayer") + (i * 2)] = (byte)((all_exitsOW[i].playerY) & 0xFF);

                ROM.DATA[ConstantsReader.GetAddress("OWExitDoorType1") + (i * 2) + 1] = (byte)((all_exitsOW[i].doorType1 >> 8) & 0xFF);
                ROM.DATA[ConstantsReader.GetAddress("OWExitDoorType1") + (i * 2)] = (byte)((all_exitsOW[i].doorType1) & 0xFF);

                ROM.DATA[ConstantsReader.GetAddress("OWExitDoorType2") + (i * 2) + 1] = (byte)((all_exitsOW[i].doorType2 >> 8) & 0xFF);
                ROM.DATA[ConstantsReader.GetAddress("OWExitDoorType2") + (i * 2)] = (byte)((all_exitsOW[i].doorType2) & 0xFF);



            }
            //WriteLog("Overworld Exits data loaded properly", Color.Green);
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
                    //WriteLog("Unknown Game Title : Using US as default", Color.Orange);
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