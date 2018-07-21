/*
 * Author:  Zarby89
 */

using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ZScream_Exporter
{
    /// <summary>
    /// Writes the ROM's data to JSON files.
    /// </summary>
    class SaveJson
    {
        //ROM.DATA is a base rom loaded to get basic information it can either be JP1.0 or US1.2
        //can still use it for load but must not be used 
        RoomSave[] all_rooms;
        MapSave[] all_maps;
        Entrance[] entrances;
        string[] texts;
        Overworld overworld;
        public SaveJson(string path,string romPath,RoomSave[] all_rooms, MapSave[] all_maps, Entrance[] entrances, string[] texts, Overworld overworld)
        {
            this.all_rooms = all_rooms;
            this.all_maps = all_maps;
            this.entrances = entrances;
            this.texts = texts;
            this.overworld = overworld;
            //TODO : Change Header location to be dynamic instead of static
            getLargeMaps();

            //ZipArchive zipfile = new ZipArchive(new FileStream("PROJECTFILE.zip", FileMode.Open), ZipArchiveMode.Create);

            string ProjectDirectorySlash = path+"\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //File.WriteAllText(ProjectDirectorySlash + "Main.cfg", writeProjectConfig());
            string[] text = new string[1];
            text[0] = "";
            //File.WriteAllText(ProjectDirectorySlash + "Project.zscr", writeProjectConfig());
            File.WriteAllText(ProjectDirectorySlash + "Project.zscr", JsonConvert.SerializeObject(text));
            Task[] tasks = new Task[20];

            tasks[0] = Task.Run(() => { writeRooms(ProjectDirectorySlash); });
            tasks[1] = Task.Run(() => { writeEntrances(ProjectDirectorySlash); });
            tasks[2] = Task.Run(() => { writeOverworldEntrances(ProjectDirectorySlash); });
            tasks[3] = Task.Run(() => { writeOverworldExits(ProjectDirectorySlash); });
            tasks[4] = Task.Run(() => { writeOverworldHoles(ProjectDirectorySlash); });
            tasks[5] = Task.Run(() => { writeText(ProjectDirectorySlash); });
            tasks[6] = Task.Run(() => { writePalettes(ProjectDirectorySlash); });
            tasks[7] = Task.Run(() => { writeGfx(ProjectDirectorySlash); });
            tasks[8] = Task.Run(() => { writeOverworldTiles16(ProjectDirectorySlash); });
            tasks[9] = Task.Run(() => { writeOverworldMaps(ProjectDirectorySlash); });
            tasks[10] = Task.Run(() => { writeOverworldConfig(ProjectDirectorySlash); });
            tasks[11] = Task.Run(() => { writeOverworldGroups(ProjectDirectorySlash); });
            tasks[12] = Task.Run(() => { writeOverworldGroups2(ProjectDirectorySlash); });
            tasks[13] = Task.Run(() => { writeOverworldSpriteset(ProjectDirectorySlash); });
            tasks[14] = Task.Run(() => { writeOverworldSprites(ProjectDirectorySlash); });
            tasks[15] = Task.Run(() => { writeOverworldItems(ProjectDirectorySlash); });
            tasks[16] = Task.Run(() => { writeOverworldTilesType(ProjectDirectorySlash); });
            tasks[17] = Task.Run(() => { writeOverlays(ProjectDirectorySlash); });
            tasks[18] = Task.Run(() => { writeASMstuff(ProjectDirectorySlash,romPath); });
            tasks[19] = Task.Run(() => { writeTransportstuff(ProjectDirectorySlash); });
            

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i].Wait();
            }

        }

        public void writeTransportstuff(string path)
        {

            if (!Directory.Exists(path + "Overworld"))
            {
                Directory.CreateDirectory(path + "Overworld");
            }
            if (!Directory.Exists(path + "Overworld//Entrances"))
            {
                Directory.CreateDirectory(path + "Overworld//Entrances");
            }

            ExitOWWhirlpool[] whirlpool = new ExitOWWhirlpool[0x08];
            for (int i = 0; i < 0x08; i++)
            {
                short[] e = new short[13];
                e[0] = (byte)((ROM.DATA[ConstantsReader.GetAddress("OWExitMapIdWhirlpool") + i]));
                e[1] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitVramWhirlpool") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitVramWhirlpool") + (i * 2)]));
                e[2] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitYScrollWhirlpool") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitYScrollWhirlpool") + (i * 2)]));
                e[3] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitXScrollWhirlpool") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitXScrollWhirlpool") + (i * 2)]));
                e[4] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitYPlayerWhirlpool") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitYPlayerWhirlpool") + (i * 2)]));
                e[5] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitXPlayerWhirlpool") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitXPlayerWhirlpool") + (i * 2)]));
                e[6] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitYCameraWhirlpool") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitYCameraWhirlpool") + (i * 2)]));
                e[7] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitXCameraWhirlpool") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitXCameraWhirlpool") + (i * 2)]));
                e[8] = (byte)((ROM.DATA[ConstantsReader.GetAddress("OWExitUnk1Whirlpool") + i]));
                e[9] = (byte)((ROM.DATA[ConstantsReader.GetAddress("OWExitUnk2Whirlpool") + i]));
                e[10] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWWhirlpoolPosition") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWWhirlpoolPosition") + (i * 2)]));
                ExitOWWhirlpool eo = (new ExitOWWhirlpool((byte)e[0], e[1], e[2], e[3], e[4], e[5], e[6], e[7], (byte)e[8], (byte)e[9],e[10]));
                whirlpool[i] = eo;
            }

            File.WriteAllText(path + "Overworld//Entrances//Whirlpool.json", JsonConvert.SerializeObject(whirlpool));
        }

        public void writeASMstuff(string path, string rompath)
        {
            if (!Directory.Exists(path + "ASM"))
            {
                Directory.CreateDirectory(path + "ASM");
            }
            
            File.Copy("Resources\\EditorCore.asm", path + "ASM\\EditorCore.asm", true);
            File.Copy("Resources\\Main.asm", path + "ASM\\Main.asm", true);
            File.Copy("Resources\\Readme.txt", path + "ASM\\Readme.txt", true);
            File.Copy("Resources\\xkas.exe", path + "xkas.exe", true);

            if (!Directory.Exists(path + "TestROM"))
            {
                Directory.CreateDirectory(path + "TestROM");
            }

            File.Copy(rompath, path + "TestROM\\test.sfc", true);


        }

        public void writeOverworldTilesType(string path)
        {

            byte[][] types = new byte[16][];
            int addr = (ConstantsReader.GetAddress("overworldTilesType"));
            for (int j = 0; j < 16; j++)
            {
                types[j] = new byte[512];
                for (int i = 0; i < 512; i++)
                {

                    types[j][i] = ROM.DATA[addr + i];
                }
            }
            File.WriteAllText(path + "Overworld//TilesTypes.json", JsonConvert.SerializeObject(types));

        }

        public void writeOverworldItems(string path)
        {
            if (!Directory.Exists(path + "Overworld//Items"))
            {
                Directory.CreateDirectory(path + "Overworld//Items");
            }

            List<roomPotSave> items = new List<roomPotSave>();

            for (int i = 0; i < 128; i++)
            {
                int addr = (ConstantsReader.GetAddress("overworldItemsBank") << 16) +
                            (ROM.DATA[ConstantsReader.GetAddress("overworldItemsPointers") + (i * 2) + 1] << 8) +
                            (ROM.DATA[ConstantsReader.GetAddress("overworldItemsPointers") + (i * 2)]);

                addr = Addresses.snestopc(addr);

                if (all_maps[i].largeMap == true)
                {
                    if (mapParent[i] != (byte)i)
                    {
                        continue;
                    }
                }
                while (true)
                {
                    byte b1 = ROM.DATA[addr];
                    byte b2 = ROM.DATA[addr + 1];
                    byte b3 = ROM.DATA[addr + 2];
                    if (b1 == 0xFF && b2 == 0xFF)
                    {
                        break;
                    }

                    int p = (((b2 & 0x1F) << 8) + b1) >> 1;

                    int x = p % 64;
                    int y = p >> 6;

                    items.Add(new roomPotSave(b3, (ushort)i, (byte)x, (byte)y, false));
                    addr += 3;
                }
            }

            File.WriteAllText(path + "Overworld//Items/Items.json", JsonConvert.SerializeObject(items));


        }





        List<OverlayData>[] overlaysDatas = new List<OverlayData>[128];
        public void writeOverlays(string path)
        {
            for (int index = 0; index < 128; index++)
            {
                overlaysDatas[index] = new List<OverlayData>();
                //overlayPointers
                int addr = (ConstantsReader.GetAddress("overlayPointersBank") << 16) +
                    (ROM.DATA[ConstantsReader.GetAddress("overlayPointers") + (index * 2) + 1] << 8) +
                    ROM.DATA[ConstantsReader.GetAddress("overlayPointers") + (index * 2)];
                addr = Addresses.snestopc(addr);

                int a = 0;
                int x = 0;
                int sta = 0;
                //16-bit mode : 
                //A9 (LDA #$)
                //A2 (LDX #$)
                //8D (STA $xxxx)
                //9D (STA $xxxx ,x)
                //8F (STA $xxxxxx)
                //1A (INC A)
                //4C (JMP)
                //60 (END)
                byte b = 0;
                while (b != 0x60)
                {
                    b = ROM.DATA[addr];
                    if (b == 0xFF)
                    {
                        break;
                    }
                    else if (b == 0xA9) //LDA #$xxxx (Increase addr+3)
                    {
                        a = (ROM.DATA[addr + 2] << 8) +
                        ROM.DATA[addr + 1];
                        addr += 3;
                        continue;
                    }
                    else if (b == 0xA2) //LDX #$xxxx (Increase addr+3)
                    {
                        x = (ROM.DATA[addr + 2] << 8) +
                        ROM.DATA[addr + 1];
                        addr += 3;
                        continue;
                    }
                    else if (b == 0x8D) //STA $xxxx (Increase addr+3)
                    {
                        sta = (ROM.DATA[addr + 2] << 8) +
                        ROM.DATA[addr + 1];

                        //draw tile at sta position
                        //Console.WriteLine("Draw Tile" + a + " at " + sta.ToString("X4"));
                        //64
                        sta = sta & 0x1FFF;
                        int yp = ((sta / 2) / 0x40);
                        int xp = (sta / 2) - (yp * 0x40);
                        overlaysDatas[index].Add(new OverlayData( (byte)xp, (byte)yp, (ushort)a));
                        addr += 3;
                        continue;
                    }
                    else if (b == 0x9D) //STA $xxxx, x (Increase addr+3)
                    {
                        sta = (ROM.DATA[addr + 2] << 8) +
                        ROM.DATA[addr + 1];
                        //draw tile at sta,X position
                        //Console.WriteLine("Draw Tile" + a + " at " + (sta + x).ToString("X4"));

                        int stax = (sta & 0x1FFF) + x;
                        int yp = ((stax / 2) / 0x40);
                        int xp = (stax / 2) - (yp * 0x40);
                        overlaysDatas[index].Add(new OverlayData((byte)xp, (byte)yp, (ushort)a));

                        addr += 3;
                        continue;
                    }
                    else if (b == 0x8F) //STA $xxxxxx (Increase addr+4)
                    {
                        sta = (ROM.DATA[addr + 2] << 8) +
                        ROM.DATA[addr + 1];
                        //draw tile at sta,X position
                        //Console.WriteLine("Draw Tile" + a + " at " + (sta + x).ToString("X4"));

                        int stax = (sta & 0x1FFF) + x;
                        int yp = ((stax / 2) / 0x40);
                        int xp = (stax / 2) - (yp * 0x40);
                        overlaysDatas[index].Add(new OverlayData((byte)xp, (byte)yp, (ushort)a));

                        addr += 4;
                        continue;
                    }
                    else if (b == 0x1A) //INC A (Increase addr+1)
                    {
                        a += 1;
                        addr += 1;
                        continue;
                    }
                    else if (b == 0x4C) //JMP $xxxx (move addr to the new address)
                    {
                        addr = (ConstantsReader.GetAddress("overlayPointersBank") << 16) +
                        (ROM.DATA[addr + 2] << 8) +
                        ROM.DATA[addr + 1];
                        addr = Addresses.snestopc(addr);
                        continue;
                    }
                    else if (b == 0x60) //RTS
                    {
                        break; //just to be sure
                    }
                }
            }

            File.WriteAllText(path + "Overworld//Overlays.json", JsonConvert.SerializeObject(overlaysDatas));
        }

        byte[] mapParent = new byte[143];
        public void getLargeMaps()
        {
            for (int i = 128; i < 143; i++)
            {
                mapParent[i] = 0;
            }
            bool[] mapChecked = new bool[64];
            for (int i = 0; i < 64; i++)
            {
                mapChecked[i] = false;
            }
            int xx = 0;
            int yy = 0;
            while (true)
            {

                int i = xx + (yy * 8);
                if (mapChecked[i] == false)
                {
                    if (all_maps[i].largeMap == true)
                    {
                        mapChecked[i] = true;
                        mapParent[i] = (byte)i;
                        mapParent[i + 64] = (byte)(i + 64);

                        mapChecked[i + 1] = true;
                        mapParent[i + 1] = (byte)i;
                        mapParent[i + 65] = (byte)(i + 64);

                        mapChecked[i + 8] = true;
                        mapParent[i + 8] = (byte)i;
                        mapParent[i + 72] = (byte)(i + 64);

                        mapChecked[i + 9] = true;
                        mapParent[i + 9] = (byte)i;
                        mapParent[i + 73] = (byte)(i + 64);
                        xx++;
                    }
                    else
                    {
                        mapParent[i] = (byte)i;
                    }
                }


                xx++;
                if (xx >= 8)
                {
                    xx = 0;
                    yy += 1;
                    if (yy >= 8)
                    {
                        break;
                    }
                }
            }
        }

        public void writeOverworldSprites(string path)
        {


            if (!Directory.Exists(path + "Overworld//Sprites"))
                Directory.CreateDirectory(path + "Overworld//Sprites");


            List<Room_Sprite> sprites = new List<Room_Sprite>();

            int spritesAddress = ConstantsReader.GetAddress("overworldSpritesBegining");
            //09 bank ? Need to check if HM change that
            for (int i = 0; i < 64; i++)
            {
                if (all_maps[i].largeMap == true)
                {
                    if (mapParent[i] != (byte)i)
                    {
                        continue;
                    }
                }
                int sprite_address_snes = (09 << 16) +
                (ROM.DATA[spritesAddress + (i * 2) + 1] << 8) +
                ROM.DATA[spritesAddress + (i * 2)];
                int sprite_address = Addresses.snestopc(sprite_address_snes);



                while (true)
                {

                    byte b1 = ROM.DATA[sprite_address];
                    byte b2 = ROM.DATA[sprite_address + 1];
                    byte b3 = ROM.DATA[sprite_address + 2];

                    if (b1 == 0xFF) { break; }

                    sprites.Add(new Room_Sprite(b3, (byte)(b2 & 0x3F), (byte)(b1 & 0x3F), (ushort)i, Sprites_Names.name[b3], 0, 0, 0, 0));
                    sprite_address += 3;
                }
            }
            File.WriteAllText(path + "Overworld//Sprites//Beginning Sprites.json", JsonConvert.SerializeObject(sprites));

            sprites.Clear();
            spritesAddress = ConstantsReader.GetAddress("overworldSpritesZelda");



            for (int i = 0; i < 143; i++)
            {
                if (all_maps[i].largeMap == true)
                {
                    if (mapParent[i] != (byte)i)
                    {
                        continue;
                    }
                }
                int sprite_address_snes = (09 << 16) +
                    (ROM.DATA[spritesAddress + (i * 2) + 1] << 8) +
                    ROM.DATA[spritesAddress + (i * 2)];
                int sprite_address = Addresses.snestopc(sprite_address_snes);
                while (true)
                {

                    byte b1 = ROM.DATA[sprite_address];
                    byte b2 = ROM.DATA[sprite_address + 1];
                    byte b3 = ROM.DATA[sprite_address + 2];

                    if (b1 == 0xFF) { break; }

                    sprites.Add(new Room_Sprite(b3, (byte)(b2 & 0x3F), (byte)(b1 & 0x3F), (ushort)i, Sprites_Names.name[b3], 0, 0, 0, 0));
                    sprite_address += 3;
                }
            }
            File.WriteAllText(path + "Overworld//Sprites//ZeldaRescued Sprites.json", JsonConvert.SerializeObject(sprites));

            sprites.Clear();
            spritesAddress = ConstantsReader.GetAddress("overworldSpritesAgahnim");


            for (int i = 0; i < 143; i++)
            {
                if (all_maps[i].largeMap == true)
                {
                    if (mapParent[i] != (byte)i)
                    {
                        continue;
                    }
                }
                int sprite_address_snes = (09 << 16) +
                    (ROM.DATA[spritesAddress + (i * 2) + 1] << 8) +
                    ROM.DATA[spritesAddress + (i * 2)];
                int sprite_address = Addresses.snestopc(sprite_address_snes);
                while (true)
                {

                    byte b1 = ROM.DATA[sprite_address];
                    byte b2 = ROM.DATA[sprite_address + 1];
                    byte b3 = ROM.DATA[sprite_address + 2];

                    if (b1 == 0xFF) { break; }

                    sprites.Add(new Room_Sprite(b3, (byte)(b2 & 0x3F), (byte)(b1 & 0x3F), (ushort)i, Sprites_Names.name[b3], 0, 0, 0, 0));
                    sprite_address += 3;
                }
            }

            File.WriteAllText(path + "Overworld//Sprites//AgahnimDefeated Sprites.json", JsonConvert.SerializeObject(sprites));
        }

        public void writeOverworldGroups(string path)
        {
            if (!Directory.Exists(path + "Overworld"))
                Directory.CreateDirectory(path + "Overworld");

            const int dim = 80 * 4;
            byte[] owblocksetgroups = new byte[dim];
            for (int i = 0; i < dim; i++)
                owblocksetgroups[i] = ROM.DATA[ConstantsReader.GetAddress("overworldgfxGroups") + i];
            File.WriteAllText(path + "Overworld//BlocksetGroups.json", JsonConvert.SerializeObject(owblocksetgroups));
        }

        public void writeOverworldSpriteset(string path)
        {
            if (!Directory.Exists(path + "Overworld"))
                Directory.CreateDirectory(path + "Overworld");

            const int dim = 143 * 4;
            byte[] owblocksetgroups = new byte[dim];
            for (int i = 0; i < dim; i++)
                owblocksetgroups[i] = ROM.DATA[ConstantsReader.GetAddress("sprite_blockset_pointer") + i];
            File.WriteAllText(path + "Overworld//SpritesetGroups.json", JsonConvert.SerializeObject(owblocksetgroups));
        }



        public void writeOverworldGroups2(string path)
        {
            if (!Directory.Exists(path + "Overworld"))
                Directory.CreateDirectory(path + "Overworld");

            const int dim = 80 * 8;
            byte[] owblocksetgroups = new byte[dim];
            for (int i = 0; i < dim; i++)
                owblocksetgroups[i] = ROM.DATA[ConstantsReader.GetAddress("overworldgfxGroups2") + i];
            File.WriteAllText(path + "Overworld//BlocksetGroups2.json", JsonConvert.SerializeObject(owblocksetgroups));
        }

        public void writeOverworldConfig(string path)
        {
            if (!Directory.Exists(path + "Overworld"))
                Directory.CreateDirectory(path + "Overworld");


            const int dim = 0xA6;
            byte[] owpalettesgroups = new byte[dim];
            int addr = ConstantsReader.GetAddress("overworldMapPaletteGroup");
            for (int i = 0; i < dim; i++)
                owpalettesgroups[i] = ROM.DATA[i + addr];





            File.WriteAllText(path + "Overworld//PalettesGroups.json", JsonConvert.SerializeObject(owpalettesgroups));

            OverworldConfig c = new OverworldConfig();
            Color[] grasscolors = new Color[3];
            grasscolors[0] = c.hardCodedLWGrass;
            grasscolors[1] = c.hardCodedDWGrass;
            grasscolors[2] = c.hardCodedDMGrass;
            File.WriteAllText(path + "Overworld//GrassColors.json", JsonConvert.SerializeObject(grasscolors));
        }

        public void writeOverworldTiles16(string path)
        {
            if (!Directory.Exists(path + "Overworld"))
            {
                Directory.CreateDirectory(path + "Overworld");
            }
            File.WriteAllText(path + "Overworld//Tiles16.json", JsonConvert.SerializeObject(overworld.tiles16));
        }

        public void writeOverworldHoles(string path)
        {
            if (!Directory.Exists(path + "Overworld"))
            {
                Directory.CreateDirectory(path + "Overworld");
            }
            if (!Directory.Exists(path + "Overworld//Entrances"))
            {
                Directory.CreateDirectory(path + "Overworld//Entrances");
            }
            EntranceOW[] holes = new EntranceOW[0x13];
            for (int i = 0; i < 0x13; i++)
            {
                short mapId = (short)((ROM.DATA[ConstantsReader.GetAddress("OWHoleArea") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWHoleArea") + (i * 2)]));
                short mapPos = (short)((ROM.DATA[ConstantsReader.GetAddress("OWHolePos") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWHolePos") + (i * 2)]));
                byte entranceId = (byte)((ROM.DATA[ConstantsReader.GetAddress("OWHoleEntrance") + i]));
                EntranceOW eo = new EntranceOW(mapId, mapPos, entranceId);
                holes[i] = eo;

            }
            File.WriteAllText(path + "Overworld//Entrances//Holes.json", JsonConvert.SerializeObject(holes));
        }



        public void writeOverworldEntrances(string path)
        {
            if (!Directory.Exists(path + "Overworld"))
            {
                Directory.CreateDirectory(path + "Overworld");
            }
            if (!Directory.Exists(path + "Overworld//Entrances"))
            {
                Directory.CreateDirectory(path + "Overworld//Entrances");
            }
            EntranceOW[] entrances = new EntranceOW[129];
            for (int i = 0; i < 129; i++)
            {
                short mapId = (short)((ROM.DATA[ConstantsReader.GetAddress("OWEntranceMap") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWEntranceMap") + (i * 2)]));
                short mapPos = (short)((ROM.DATA[ConstantsReader.GetAddress("OWEntrancePos") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWEntrancePos") + (i * 2)]));
                byte entranceId = (byte)((ROM.DATA[ConstantsReader.GetAddress("OWEntranceEntranceId") + i]));
                EntranceOW eo = new EntranceOW(mapId, mapPos, entranceId);
                entrances[i] = eo;
            }
            File.WriteAllText(path + "Overworld//Entrances//Entrances.json", JsonConvert.SerializeObject(entrances));
        }

        public void writeOverworldExits(string path)
        {
            if (!Directory.Exists(path + "Overworld"))
            {
                Directory.CreateDirectory(path + "Overworld");
            }
            if (!Directory.Exists(path + "Overworld//Entrances"))
            {
                Directory.CreateDirectory(path + "Overworld//Entrances");
            }

            ExitOW[] exits = new ExitOW[0x4F];
            for (int i = 0; i < 0x4F; i++)
            {
                short[] e = new short[13];
                e[0] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitRoomId") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitRoomId") + (i * 2)]));
                e[1] = (byte)((ROM.DATA[ConstantsReader.GetAddress("OWExitMapId") + i]));
                e[2] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitVram") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitVram") + (i * 2)]));
                e[3] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitYScroll") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitYScroll") + (i * 2)]));
                e[4] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitXScroll") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitXScroll") + (i * 2)]));
                e[5] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitYPlayer") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitYPlayer") + (i * 2)]));
                e[6] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitXPlayer") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitXPlayer") + (i * 2)]));
                e[7] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitYCamera") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitYCamera") + (i * 2)]));
                e[8] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitXCamera") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitXCamera") + (i * 2)]));
                e[9] = (byte)((ROM.DATA[ConstantsReader.GetAddress("OWExitUnk1") + i]));
                e[10] = (byte)((ROM.DATA[ConstantsReader.GetAddress("OWExitUnk2") + i]));
                e[11] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitDoorType1") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitDoorType1") + (i * 2)]));
                e[12] = (short)((ROM.DATA[ConstantsReader.GetAddress("OWExitDoorType2") + (i * 2) + 1] << 8) + (ROM.DATA[ConstantsReader.GetAddress("OWExitDoorType2") + (i * 2)]));
                ExitOW eo = (new ExitOW(e[0], (byte)e[1], e[2], e[3], e[4], e[5], e[6], e[7], e[8], (byte)e[9], (byte)e[10], e[11], e[12]));
                exits[i] = eo;
            }

            File.WriteAllText(path + "Overworld//Entrances//Exits.json", JsonConvert.SerializeObject(exits));
        }

        public void writeOverworldMaps(string path)
        {
            if (!Directory.Exists(path + "Overworld"))
            {
                Directory.CreateDirectory(path + "Overworld");
            }
            if (!Directory.Exists(path + "Overworld//Maps"))
            {
                Directory.CreateDirectory(path + "Overworld//Maps");
            }
            for (int i = 0; i < 160; i++)
            {

                File.WriteAllText(path + "Overworld//Maps//Map" + i.ToString("D3") + ".json", JsonConvert.SerializeObject(all_maps[i]));
            }
        }

        public void writeGfx(string path)
        {
            if (!Directory.Exists(path + "Graphics"))
            {
                Directory.CreateDirectory(path + "Graphics");
            }

            for (int i = 0; i < 223; i++)
            {
                GFX.singleGrayscaletobmp(i).Save(path + "Graphics//" + i.ToString("D3") + ".png");
            }

            GFX.linkGrayscaletobmp().Save(path + "Graphics//" + "Link" + ".png");
        }

        public void writeText(string path)
        {
            if (!Directory.Exists(path + "Texts"))
            {
                Directory.CreateDirectory(path + "Texts");
            }

            File.WriteAllText(path + "Texts//AllTexts.json", JsonConvert.SerializeObject(TextData.messages.ToArray()));
        }



        public void writePalettes(string path)
        {
            //save them into yy-chr format

            //:thinking:


            //Separating palettes

            //DD218-DD290 lightworld sprites palettes (15*4)

            writePalette(0xDD218, 15, 4, path, "Sprites Palettes", "Lightworld Sprites");

            //DD291-DD308 darkworld sprites palettes (15*4)

            writePalette(0xDD290, 15, 4, path, "Sprites Palettes", "Darkworld Sprites");

            //DD309-DD39D Armors Palettes (15*5)
            writePalette(0xDD308, 15, 5, path, "Link Palettes", "Mails");

            //DD39E-DD445 Spr Aux Palettes? (7*12)
            writePalette(0xDD39E, 7, 12, path, "Sprites Palettes", "Aux Sprites1");

            //DD446-DD4DF Spr Aux2 Palettes? (7*11)
            writePalette(0xDD446, 7, 11, path, "Sprites Palettes", "Aux Sprites2");

            //DD4E0-DD62F Spr Aux Palettes? (7*24)
            writePalette(0xDD4E0, 7, 24, path, "Sprites Palettes", "Aux Sprites3");

            //DD630-DD647 Sword Palettes (3*4)
            writePalette(0xDD39E, 3, 4, path, "Link Palettes", "Sword Sprites");

            //DD648-DD65F Shield Palettes (4*3)
            writePalette(0xDD648, 4, 3, path, "Link Palettes", "Shield Sprites");

            //DD660-DD69F Hud Palettes (4*8)
            writePalette(0xDD660, 4, 8, path, "Hud Palettes", "Hud1");

            //DD6A0-DD6DF Hud Palettes2 (4*8)
            writePalette(0xDD6A0, 4, 8, path, "Hud Palettes", "Hud2");

            //DD6E0-DD709 Unused Palettes (7*3) ?
            writePalette(0xDD6E0, 7, 3, path, "Unused Palettes", "Unused");

            //DD70A-DD733 Map Sprites Palettes (7*3)
            writePalette(0xDD70A, 7, 3, path, "Dungeon Map Palette", "Map Sprite");

            //DD734 Dungeons Palettes :scream: (15*6) * 19
            for (int i = 0; i < 19; i++)
            {
                writePalette(0xDD734 + (i * 180), 15, 6, path, "Dungeon Palette", "Dungeon " + i.ToString("D2"));
            }
            //DE544-DE603 Map bg palette (15*6)
            writePalette(0xDE544, 15, 6, path, "Dungeon Map Palette", "Map Bg");

            //DE604-DE6C7 overworld Aux Palettes (7*14)
            writePalette(0xDE604, 7, 14, path, "Overworld Palette", "Overworld Animated");

            //DE6C8 Main Overworld Palettes (7*5) * 6
            for (int i = 0; i < 6; i++)
            {
                writePalette(0xDE6C8 + (i * 70), 7, 5, path, "Overworld Palette", "Main Overworld " + i.ToString("D2"));
            }

            //DE86C Overworld Aux Palettes (7*3) * 20
            for (int i = 0; i < 20; i++)
            {
                writePalette(0xDE86C + (i * 42), 7, 3, path, "Overworld Palette", "Overworld Aux2 " + i.ToString("D2"));
            }

            //save them in .png format 8x8 for each colors
        }

        public void writePalette(int palettePos, int w, int h, string path, string dir, string name)
        {

            if (!Directory.Exists(path + "Palettes//" + dir))
                Directory.CreateDirectory(path + "Palettes//" + dir);

            //Bitmap paletteBitmap = new Bitmap(w * 8, h * 8);
            Color[] palettes = new Color[h * w];
            int pos = palettePos;
            int ppos = 0;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {

                    palettes[ppos] = GFX.getColor((short)((ROM.DATA[pos + 1] << 8) + ROM.DATA[pos]));
                    //Graphics g = Graphics.FromImage(paletteBitmap);
                    //g.FillRectangle(new SolidBrush(c), new Rectangle(x * 8, y * 8, 8, 8));
                    pos += 2;
                    ppos++;
                }
            }
            File.WriteAllText(path + "Palettes//" + dir + "//" + name + ".json", JsonConvert.SerializeObject(palettes));
            /*
            //path = ProjectDirectory//
            paletteBitmap.Save(path + "Palettes//" + dir + "//" + name + ".png");

            paletteBitmap.Dispose();*/


        }

        public string writeProjectConfig()
        {
            configSave cs = new configSave();
            return JsonConvert.SerializeObject(cs);
        }

        public void writeEntrances(string path)
        {
            if (!Directory.Exists(path + "Dungeons"))
                Directory.CreateDirectory(path + "Dungeons");
            if (!Directory.Exists(path + "Dungeons//Entrances"))
                Directory.CreateDirectory(path + "Dungeons//Entrances");

            Entrance[] entrances = new Entrance[133];
            for (int i = 0; i < 133; i++)
            {
                Entrance e = new Entrance((byte)i);
                entrances[i] = e;
            }
            File.WriteAllText(path + "Dungeons//Entrances//Entrances.json", JsonConvert.SerializeObject(entrances));
            entrances = new Entrance[7];
            for (int i = 0; i < 7; i++)
            {
                Entrance e = new Entrance((byte)i, true);
                entrances[i] = e;

            }
            File.WriteAllText(path + "Dungeons//Entrances//Starting Entrances.json", JsonConvert.SerializeObject(entrances));

        }

        public void writeRooms(string path)
        {
            if (!Directory.Exists(path + "Dungeons"))
            {
                Directory.CreateDirectory(path + "Dungeons");
            }
            if (!Directory.Exists(path + "Dungeons//Rooms"))
            {
                Directory.CreateDirectory(path + "Dungeons//Rooms");
            }
            for (int i = 0; i < 296; i++)
            {
                RoomSave rs = new RoomSave((short)i);
                File.WriteAllText(path + "Dungeons//Rooms//Room " + i.ToString("D3") + ".json", JsonConvert.SerializeObject(rs, Formatting.None, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));

            }
        }

        public int getLongPointerSnestoPc(int pos)
        {
            int p = (ROM.DATA[pos + 2] << 16) + (ROM.DATA[pos + 1] << 8) + (ROM.DATA[pos]);
            return (Addresses.snestopc(p));
        }
    }

    public class TextSave
    {
        public string[] all_texts;
        public TextSave(string[] all_texts)
        {
            this.all_texts = all_texts;
        }
    }

    //Rooms, Pots, Chests, Sprites, Headers, Done !
    public class roomPotSave
    {
        public byte x, y, id;
        public bool bg2 = false;
        public ushort roomMapId;
        public roomPotSave(byte id, ushort roomMapId, byte x, byte y, bool bg2)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.bg2 = bg2;
            this.roomMapId = roomMapId;
        }
    }



    public class configSave
    {
        public string ProjectName = "";
        public string ProjectVersion = "";
        public string[] allDungeons = new string[17];
        public DataRoom[] allrooms = new DataRoom[296];
        public string[] allMapsNames = new string[160];

        public configSave()
        {
            ProjectName = "Test Name";
            ProjectVersion = "V1.0";

            allDungeons = ROMStructure.dungeonsNames;
            DataRoom[] dr = ROMStructure.dungeonsRoomList
            .Where(x => x != null)
            .OrderBy(x => x.id)
            .Select(x => x) //?
            .ToArray();
            allrooms = dr;
        }


    }

    public struct MapSave
    {
        public ushort[,] tiles; //all map tiles (short values) 0 to 1024 from left to right
        public bool largeMap;
        public byte spriteset;
        public short index;
        public byte palette;
        public byte sprite_palette;
        public byte blockset;
        public short msgid;
        public string name;
        public byte tileTypeSet;
        //public List<Room_Sprite> sprites;
        //public List<roomPotSave> items;
        public MapSave(short id, Overworld overworld)
        {
            tiles = new ushort[32, 32];
            largeMap = false;
            this.index = id;
            this.palette = (byte)(ROM.DATA[ConstantsReader.GetAddress("overworldMapPalette") + index] << 2);
            this.blockset = ROM.DATA[ConstantsReader.GetAddress("mapGfx") + index];
            this.tileTypeSet = ROM.DATA[0x1EF800 + index]; //all 0 by default so that's fine
            this.sprite_palette = (byte)(ROM.DATA[ConstantsReader.GetAddress("overworldSpritePalette") + index]);
            this.msgid = (short)((ROM.DATA[(ConstantsReader.GetAddress("overworldMessages") + index * 2) + 1] << 8) + ROM.DATA[(ConstantsReader.GetAddress("overworldMessages") + index * 2)]);
            if (index != 0x80)
                if (index <= 150)
                    if (ROM.DATA[ConstantsReader.GetAddress("overworldMapSize") + (index & 0x3F)] != 0)
                        largeMap = true;

            this.spriteset = ROM.DATA[ConstantsReader.GetAddress("overworldSpriteset") + index];
            this.name = ROMStructure.mapsNames[index];

            /*sprites = new List<Room_Sprite>();
            int address = 0;
            if (index < 0x40)
            {
                address = ConstantsReader.GetAddress("overworldSpritesLW");
                }
            else
            {
                address = ConstantsReader.GetAddress("overworldSpritesDW");
                }
            //09 bank ? Need to check if HM change that
            int sprite_address_snes = (09 << 16) +
            (ROM.DATA[address + (index * 2) + 1] << 8) +
            ROM.DATA[address + (index * 2)];
            int sprite_address = Addresses.snestopc(sprite_address_snes);

            while (true)
            {
                byte b1 = ROM.DATA[sprite_address];
                byte b2 = ROM.DATA[sprite_address + 1];
                byte b3 = ROM.DATA[sprite_address + 2];

                if (b1 == 0xFF) { break; }

                sprites.Add(new Room_Sprite(b3, (byte)(b2 & 0x3F), (byte)(b1 & 0x3F), Sprites_Names.name[b3], 0, 0, 0, 0));
                sprite_address += 3;
            }


            items = new List<roomPotSave>();

            int addr = (ConstantsReader.GetAddress("overworldItemsBank") << 16) +
                        (ROM.DATA[ConstantsReader.GetAddress("overworldItemsPointers") + (index * 2) + 1] << 8) +
                        (ROM.DATA[ConstantsReader.GetAddress("overworldItemsPointers") + (index * 2)]);

            addr = Addresses.snestopc(addr);

            while (true)
            {
                byte b1 = ROM.DATA[addr];
                byte b2 = ROM.DATA[addr + 1];
                byte b3 = ROM.DATA[addr + 2];
                if (b1 == 0xFF && b2 == 0xFF)
                {
                    break;
                }

                int p = (((b2 & 0x1F) << 8) + b1) >> 1;

                int x = p % 64;
                int y = p >> 6;

                items.Add(new roomPotSave(b3, (byte)x, (byte)y, false));
                addr += 3;
            }
            */

            int t = index * 256;
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    tiles[(x * 2), (y * 2)] = overworld.map16tiles[t].tile0;
                    tiles[(x * 2) + 1, (y * 2)] = overworld.map16tiles[t].tile1;
                    tiles[(x * 2), (y * 2) + 1] = overworld.map16tiles[t].tile2;
                    tiles[(x * 2) + 1, (y * 2) + 1] = overworld.map16tiles[t].tile3;
                    t++;
                }
            }
        }
    }



    public class OverworldConfig
    {
        public Color hardCodedDWGrass;
        public Color hardCodedLWGrass;
        public Color hardCodedDMGrass;

        public OverworldConfig()
        {
            hardCodedDWGrass = GFX.getColor((short)((ROM.DATA[ConstantsReader.GetAddress("hardcodedGrassDW") + 1] << 8) + ROM.DATA[ConstantsReader.GetAddress("hardcodedGrassDW")]));
            hardCodedLWGrass = GFX.getColor((short)((ROM.DATA[ConstantsReader.GetAddress("hardcodedGrassLW") + 1] << 8) + ROM.DATA[ConstantsReader.GetAddress("hardcodedGrassLW")]));
            hardCodedDMGrass = GFX.getColor((short)((ROM.DATA[ConstantsReader.GetAddress("hardcodedGrassSpecial") + 1] << 8) + ROM.DATA[ConstantsReader.GetAddress("hardcodedGrassSpecial")]));
        }
    }

    public class PaletteConfig
    { byte[] owpalgroup1 = new byte[0xA6]; }

}

public struct OverlayData
{
    public byte x;
    public byte y;
    public ushort tileId;
    public OverlayData(byte x, byte y, ushort tileId)
    {
        this.x = x;
        this.y = y;
        this.tileId = tileId;
    }
}