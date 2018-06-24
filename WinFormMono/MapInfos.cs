using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormMono
{
    public class MapInfos
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
        
        public MapInfos(string projectLoaded)
        {
            blocksetGroups = JsonConvert.DeserializeObject<byte[]>(File.ReadAllText(projectLoaded + "//Overworld//BlocksetGroups.json"));
            blocksetGroups2 = JsonConvert.DeserializeObject<byte[]>(File.ReadAllText(projectLoaded + "//Overworld//BlocksetGroups2.json"));
            palettesGroups = JsonConvert.DeserializeObject<byte[]>(File.ReadAllText(projectLoaded + "//Overworld//PalettesGroups.json"));
            spritesetGroups = JsonConvert.DeserializeObject<byte[]>(File.ReadAllText(projectLoaded + "//Overworld//SpritesetGroups.json"));

            for (int i = 0; i < 128; i++)
            {
                entranceOWs[i] = JsonConvert.DeserializeObject<EntranceOW>(File.ReadAllText(projectLoaded + "//Overworld//Entrances//Entrance" + i.ToString("D3") + ".json"));
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

    }
}
