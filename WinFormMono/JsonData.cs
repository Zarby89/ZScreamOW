using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
        public PaletteHandler allPalettes;
        public Bitmap[] tilesetBitmaps = new Bitmap[223];
        public MapSave[] mapdata = new MapSave[128];
        string projectLoaded = "";

        public JsonData(string projectLoaded)
        {
            this.projectLoaded = projectLoaded;
            allPalettes = new PaletteHandler(projectLoaded);

            blocksetGroups = JsonConvert.DeserializeObject<byte[]>(File.ReadAllText(projectLoaded + "//Overworld//BlocksetGroups.json"));
            blocksetGroups2 = JsonConvert.DeserializeObject<byte[]>(File.ReadAllText(projectLoaded + "//Overworld//BlocksetGroups2.json"));
            palettesGroups = JsonConvert.DeserializeObject<byte[]>(File.ReadAllText(projectLoaded + "//Overworld//PalettesGroups.json"));
            spritesetGroups = JsonConvert.DeserializeObject<byte[]>(File.ReadAllText(projectLoaded + "//Overworld//SpritesetGroups.json"));

            for (int i = 0; i < 128; i++)
            {
                mapdata[i] = JsonConvert.DeserializeObject<MapSave>(File.ReadAllText(projectLoaded + "//Overworld//Maps//Map" + i.ToString("D3") + ".json"));
            }

            for (int i = 0; i < 223; i++)
            {
                tilesetBitmaps[i] = new Bitmap(projectLoaded + "//Graphics//blockset" + i.ToString("D3") + ".png");
            }

            for (int i = 0; i < 128; i++)
            {
                entranceOWs[i] = JsonConvert.DeserializeObject<EntranceOW>(File.ReadAllText(projectLoaded + "//Overworld//Entrances//Entrance" + i.ToString("D3") + ".json"));

                byte m = entranceOWs[i].entranceId;
                short s = entranceOWs[i].mapId;
                int p = entranceOWs[i].mapPos >> 1;
                int x = (p % 64);
                int y = (p >> 6);



                entranceOWsEditor[i] = new EntranceOWEditor((x * 16) + ((s - ((s / 8) * 8)) * 512), (y * 16) + ((s / 8) * 512), m, s, entranceOWs[i].mapPos);

            }

            for (int i = 0; i < 19; i++)
            {
                holes[i] = JsonConvert.DeserializeObject<EntranceOW>(File.ReadAllText(projectLoaded + "//Overworld//Holes//hole" + i.ToString("D3") + ".json"));
            }

            for (int i = 0; i < 133; i++)
            {
                entrances[i] = JsonConvert.DeserializeObject<Entrance>(File.ReadAllText(projectLoaded + "//Dungeons//Entrances//Entrance " + i.ToString("D3") + ".json"));
            }

            for (int i = 0; i < 78; i++)
            {
                exitsOWs[i] = JsonConvert.DeserializeObject<ExitOW>(File.ReadAllText(projectLoaded + "//Overworld//Exits//Exit" + i.ToString("D3") + ".json"));
            }

            alltiles16 = JsonConvert.DeserializeObject<Tile16[]>(File.ReadAllText(projectLoaded + "//Overworld//Tiles16.json"));
            gameTexts = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(projectLoaded + "//Texts//AllTexts.json"));

        }

        public void SaveEntrances()
        {
            for (int i = 0; i < 128; i++)
            {
                entranceOWs[i].mapId = entranceOWsEditor[i].mapId;
                entranceOWs[i].mapPos = entranceOWsEditor[i].mapPos;
                entranceOWs[i].entranceId = entranceOWsEditor[i].entranceId;
                File.WriteAllText(projectLoaded + "//Overworld//Entrances//Entrance" + i.ToString("D3") + ".json", JsonConvert.SerializeObject(entranceOWs[i]));
            }
        }

    }
}
