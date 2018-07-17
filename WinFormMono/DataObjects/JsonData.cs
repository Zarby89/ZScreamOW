using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinFormMono
{
    public class JsonData
    {
        public byte[] blocksetGroups;
        public byte[] blocksetGroups2;
        public byte[] palettesGroups;
        public byte[] spritesetGroups;
        public EntranceOW[] entranceOWs = new EntranceOW[128];
        public Entrance[] entrances = new Entrance[133];
        public EntranceOW[] holes = new EntranceOW[19];
        public ExitOW[] exitsOWs = new ExitOW[78];
        public string[] gameTexts;
        public Tile16[] alltiles16;
        public EntranceOWEditor[] entranceOWsEditor = new EntranceOWEditor[128];
        public EntranceOWEditor[] holesOWsEditor = new EntranceOWEditor[19];
        public PaletteHandler allPalettes;
        public Bitmap[] tilesetBitmaps = new Bitmap[223];
        public MapSave[] mapdata = new MapSave[128];
        public RoomPotSave[] itemsOW;
        public List<RoomPotSaveEditor> itemsOWEditor = new List<RoomPotSaveEditor>();
        public IntPtr allgfx8array = Marshal.AllocHGlobal(32768);
        string projectLoaded = "";
        public Bitmap linkGfx;
        public List<Room_Sprite>[] spritesOW = new List<Room_Sprite>[3];
        public List<Room_SpriteOWEditor>[] spritesOWEditor = new List<Room_SpriteOWEditor>[3];
        public byte[][] tileTypeSet = new byte[16][];
        public JsonData(string projectLoaded)
        {
            this.projectLoaded = projectLoaded;
            allPalettes = new PaletteHandler(projectLoaded);

            blocksetGroups = JsonConvert.DeserializeObject<byte[]>(File.ReadAllText(projectLoaded + "//Overworld//BlocksetGroups.json"));
            blocksetGroups2 = JsonConvert.DeserializeObject<byte[]>(File.ReadAllText(projectLoaded + "//Overworld//BlocksetGroups2.json"));
            palettesGroups = JsonConvert.DeserializeObject<byte[]>(File.ReadAllText(projectLoaded + "//Overworld//PalettesGroups.json"));
            spritesetGroups = JsonConvert.DeserializeObject<byte[]>(File.ReadAllText(projectLoaded + "//Overworld//SpritesetGroups.json"));

            itemsOW = JsonConvert.DeserializeObject<RoomPotSave[]>(File.ReadAllText(projectLoaded + "//Overworld//Items//Items.json"));
            createItems();

            spritesOW[0] = JsonConvert.DeserializeObject<List<Room_Sprite>>(File.ReadAllText(projectLoaded + "//Overworld//Sprites//Beginning Sprites.json"));
            spritesOW[1] = JsonConvert.DeserializeObject<List<Room_Sprite>>(File.ReadAllText(projectLoaded + "//Overworld//Sprites//ZeldaRescued Sprites.json"));
            spritesOW[2] = JsonConvert.DeserializeObject<List<Room_Sprite>>(File.ReadAllText(projectLoaded + "//Overworld//Sprites//AgahnimDefeated Sprites.json"));
            createSprites(0);
            createSprites(1);
            createSprites(2);


            for (int i = 0; i < 128; i++)
            {
                mapdata[i] = JsonConvert.DeserializeObject<MapSave>(File.ReadAllText(projectLoaded + "//Overworld//Maps//Map" + i.ToString("D3") + ".json"));
            }

            for (int i = 0; i < 223; i++)
            {
                tilesetBitmaps[i] = new Bitmap(projectLoaded + "//Graphics//" + i.ToString("D3") + ".png");
            }
            entranceOWs = JsonConvert.DeserializeObject<EntranceOW[]>(File.ReadAllText(projectLoaded + "//Overworld//Entrances//Entrances.json"));
            for (int i = 0; i < 128; i++)
            {
                byte m = entranceOWs[i].entranceId;
                short s = (short)(entranceOWs[i].mapId);
                int p = entranceOWs[i].mapPos >> 1;
                int x = (p % 64);
                int y = (p >> 6);
                entranceOWsEditor[i] = new EntranceOWEditor((x * 16) + (((s % 64) - (((s % 64) / 8) * 8)) * 512), (y * 16) + (((s % 64) / 8) * 512), m, s, entranceOWs[i].mapPos);
            }

            holes = JsonConvert.DeserializeObject<EntranceOW[]>(File.ReadAllText(projectLoaded + "//Overworld//Entrances//Holes.json"));
            for (int i = 0; i < 19; i++)
            {
                byte m = holes[i].entranceId;
                short s = holes[i].mapId;
                int p = (holes[i].mapPos + 0x400) >> 1;
                int x = (p % 64);
                int y = (p >> 6);
                holesOWsEditor[i] = new EntranceOWEditor((x * 16) + ((s - ((s / 8) * 8)) * 512), (y * 16) + ((s / 8) * 512), m, s, (short)(holes[i].mapPos + 0x400));
                //Handle hole to editorhole

            }
            entrances = JsonConvert.DeserializeObject<Entrance[]>(File.ReadAllText(projectLoaded + "//Dungeons//Entrances//Entrances.json"));

            exitsOWs = JsonConvert.DeserializeObject<ExitOW[]>(File.ReadAllText(projectLoaded + "//Overworld//Entrances//Exits.json"));

            tileTypeSet = JsonConvert.DeserializeObject<byte[][]>(File.ReadAllText(projectLoaded + "//Overworld//TilesTypes.json"));

            alltiles16 = JsonConvert.DeserializeObject<Tile16[]>(File.ReadAllText(projectLoaded + "//Overworld//Tiles16.json"));

            File.WriteAllText(projectLoaded + "//Overworld//Tiles16Backup.json", JsonConvert.SerializeObject(alltiles16));

            gameTexts = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(projectLoaded + "//Texts//AllTexts.json"));
            linkGfx = new Bitmap(128, 448);
            using (Graphics g = Graphics.FromImage(linkGfx))
            {
                Bitmap templinkGfx = new Bitmap(projectLoaded + "//Graphics//Link.png");
                g.DrawImage(templinkGfx, 0, 0);
            }
            linkGfx.MakeTransparent(Color.Black);

        }

        public void SaveAll(Map16[] allmaps)
        {
            SaveEntrances();
            saveExits(allmaps);
            saveMaps(allmaps);
            saveSprites();
            saveItems();
            saveTilesTypes();
            saveTiles16();
            SaveHoles();
        }

        public void saveTiles16()
        {
            File.WriteAllText(projectLoaded + "//Overworld//Tiles16.json", JsonConvert.SerializeObject(alltiles16));
        }

        public void saveTilesTypes()
        {
            File.WriteAllText(projectLoaded + "//Overworld//TilesTypes.json", JsonConvert.SerializeObject(tileTypeSet));
        }

        public void SaveEntrances()
        {
            for (int i = 0; i < 128; i++)
            {
                entranceOWs[i].mapId = entranceOWsEditor[i].mapId;
                entranceOWs[i].mapPos = entranceOWsEditor[i].mapPos;
                entranceOWs[i].entranceId = entranceOWsEditor[i].entranceId;

            }
            File.WriteAllText(projectLoaded + "//Overworld//Entrances//Entrances.json", JsonConvert.SerializeObject(entranceOWs));
        }

        public void SaveHoles()
        {
            for (int i = 0; i < holes.Length; i++)
            {
                holes[i].mapId = holesOWsEditor[i].mapId;
                holes[i].mapPos = (short)(holesOWsEditor[i].mapPos - 0x400);
                holes[i].entranceId = holesOWsEditor[i].entranceId;

            }
            File.WriteAllText(projectLoaded + "//Overworld//Entrances//Holes.json", JsonConvert.SerializeObject(holes));
        }

        public void createSprites(int ind)
        {
            spritesOWEditor[ind] = new List<Room_SpriteOWEditor>();
            for (int i = 0; i < spritesOW[ind].Count; i++)
            {
                
                int s = spritesOW[ind][i].roomMapId % 64;

                byte id = spritesOW[ind][i].id;
                byte o = spritesOW[ind][i].overlord;
                byte sub = spritesOW[ind][i].subtype;
                int x = (spritesOW[ind][i].x * 16) + ((s - ((s / 8) * 8)) * 512);
                int y = (spritesOW[ind][i].y * 16) + ((s / 8) * 512);

                spritesOWEditor[ind].Add(new Room_SpriteOWEditor(id, x, y, spritesOW[ind][i].roomMapId, o, sub));
                spritesOWEditor[ind][i].gameX = spritesOW[ind][i].x;
                spritesOWEditor[ind][i].gameY = spritesOW[ind][i].y;
            }

        }

        public void createItems()
        {
            for (int i = 0; i < itemsOW.Length; i++)
            {
                int s = itemsOW[i].roomMapId % 64;
                int x = (itemsOW[i].x * 16) + ((s - ((s / 8) * 8)) * 512);
                int y = (itemsOW[i].y * 16) + ((s / 8) * 512);

                itemsOWEditor.Add(new RoomPotSaveEditor((byte)itemsOW[i].id, (ushort)itemsOW[i].roomMapId, x, y, itemsOW[i].bg2));
                itemsOWEditor[i].gameX = itemsOW[i].x;
                itemsOWEditor[i].gameY = itemsOW[i].y;
            }


        }

        public void saveMaps(Map16[] allmaps)
        {

            for (int i = 0; i < 128; i++)
            {
                File.WriteAllText(projectLoaded + "//Overworld//Maps//Map" + i.ToString("D3") + ".json", JsonConvert.SerializeObject(allmaps[i].mapdata));
            }

        }

        public void saveSprites()
        {
            spritesOW[0].Clear();
            spritesOW[1].Clear();
            spritesOW[2].Clear();
            for (int i = 0; i < spritesOWEditor[0].Count; i++)
            {
                spritesOW[0].Add(new Room_Sprite(spritesOWEditor[0][i].id, spritesOWEditor[0][i].gameX, spritesOWEditor[0][i].gameY, spritesOWEditor[0][i].roomMapId, Sprites_Names.name[spritesOWEditor[0][i].id], 0, 0, 0, 0));
            }
            for (int i = 0; i < spritesOWEditor[1].Count; i++)
            {
                spritesOW[1].Add(new Room_Sprite(spritesOWEditor[1][i].id, spritesOWEditor[1][i].gameX, spritesOWEditor[1][i].gameY, spritesOWEditor[1][i].roomMapId, Sprites_Names.name[spritesOWEditor[1][i].id], 0, 0, 0, 0));
            }
            for (int i = 0; i < spritesOWEditor[2].Count; i++)
            {
                spritesOW[2].Add(new Room_Sprite(spritesOWEditor[2][i].id, spritesOWEditor[2][i].gameX, spritesOWEditor[2][i].gameY, spritesOWEditor[2][i].roomMapId, Sprites_Names.name[spritesOWEditor[2][i].id], 0, 0, 0, 0));
            }

            File.WriteAllText(projectLoaded + "//Overworld//Sprites//Beginning Sprites.json", JsonConvert.SerializeObject(spritesOW[0]));
            File.WriteAllText(projectLoaded + "//Overworld//Sprites//ZeldaRescued Sprites.json",JsonConvert.SerializeObject(spritesOW[1]));
            File.WriteAllText(projectLoaded + "//Overworld//Sprites//AgahnimDefeated Sprites.json",JsonConvert.SerializeObject(spritesOW[2]));

        }

        public void saveItems()
        {
            itemsOW = new RoomPotSave[itemsOWEditor.Count];
            for(int i = 0;i<itemsOWEditor.Count;i++)
            {
                itemsOW[i] = new RoomPotSave(itemsOWEditor[i].id, itemsOWEditor[i].roomMapId, itemsOWEditor[i].gameX, itemsOWEditor[i].gameY, itemsOWEditor[i].bg2);
            }

            File.WriteAllText(projectLoaded + "//Overworld//Items//Items.json", JsonConvert.SerializeObject(itemsOW));


        }

        public void saveExits(Map16[] allmaps)
        {
            for (int i = 0; i < 75; i++)
            {

                
                File.WriteAllText(projectLoaded + "//Overworld//Entrances//Exits.json", JsonConvert.SerializeObject(exitsOWs));
                
        }
        }

    }
}
