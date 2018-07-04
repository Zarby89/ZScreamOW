using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WinFormMono
{
    public class PaletteHandler
    {
        public Color[][] mainPalettes = new Color[6][];
        public Color[][] hudPalettes = new Color[2][];
        public Color[][] auxPalettes = new Color[20][];
        public Color[] animatedPalettes = new Color[32];
        public Color[] bgrcolor;
        public Color[][] spritePalettes = new Color[2][];
        public Color[] aux3spritePalettes = new Color[17 * 7];
        public Color[] aux2spritePalettes = new Color[15 * 7];
        public byte[] palettesGroups;
        public PaletteHandler(string projectLoaded)
        {
            //Main Palettes
            for(int i = 0;i<6;i++)
            {
                mainPalettes[i] = JsonConvert.DeserializeObject<Color[]>(File.ReadAllText(projectLoaded + "//Palettes//Overworld Palette//Main Overworld " + i.ToString("D2") + ".json"));
            }
            //Hud Palettes
            for (int i = 0; i < 2; i++)
            {
                hudPalettes[i] = JsonConvert.DeserializeObject<Color[]>(File.ReadAllText(projectLoaded + "//Palettes//Hud Palettes//Hud" + (i+1).ToString("D1") + ".json"));
            }
            //Aux Palettes
            for (int i = 0; i < 20; i++)
            {
                auxPalettes[i] = JsonConvert.DeserializeObject<Color[]>(File.ReadAllText(projectLoaded + "//Palettes//Overworld Palette//Overworld Aux2 "+i.ToString("D2")+".json"));
            }


            aux3spritePalettes = JsonConvert.DeserializeObject<Color[]>(File.ReadAllText(projectLoaded + "//Palettes//Sprites Palettes//Aux Sprites3" + ".json"));
            aux2spritePalettes = JsonConvert.DeserializeObject<Color[]>(File.ReadAllText(projectLoaded + "//Palettes//Sprites Palettes//Aux Sprites2" + ".json"));

            spritePalettes[0] = JsonConvert.DeserializeObject<Color[]>(File.ReadAllText(projectLoaded + "//Palettes//Sprites Palettes//Lightworld Sprites.json"));
            spritePalettes[1] = JsonConvert.DeserializeObject<Color[]>(File.ReadAllText(projectLoaded + "//Palettes//Sprites Palettes//Darkworld Sprites.json"));
            //Animated Palettes
            animatedPalettes = JsonConvert.DeserializeObject<Color[]>(File.ReadAllText(projectLoaded + "//Palettes//Overworld Palette//Overworld Animated.json"));

            bgrcolor = JsonConvert.DeserializeObject<Color[]>(File.ReadAllText(projectLoaded + "//Overworld//GrassColors.json"));

            palettesGroups = JsonConvert.DeserializeObject<byte[]>(File.ReadAllText(projectLoaded + "//Overworld//PalettesGroups.json"));

        }

    }
}
