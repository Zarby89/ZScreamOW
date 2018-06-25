using System;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace WinFormMono
{
    class DrawTest : PictureBox
    {
        string projectLoaded = "";
        Bitmap[] tilesetBitmaps = new Bitmap[222];
        IntPtr allgfx8array = Marshal.AllocHGlobal(32768);
        Bitmap allgfx8;
        PaletteHandler allPalettes;
        Map16[] allmaps = new Map16[128];
        MapInfos mapInfos;
        public bool lightworld = true;

        public void CreateProject(string projectLoaded)
        {
            this.Image = new Bitmap(4096, 4096);
            this.projectLoaded = projectLoaded;
            allgfx8 = new Bitmap(128, 512, 64, PixelFormat.Format4bppIndexed, allgfx8array); //temporary variable used for all rooms
            allPalettes = new PaletteHandler(projectLoaded);
            mapInfos = new MapInfos(projectLoaded);
            for (int i = 0; i < 222; i++)
            {
                tilesetBitmaps[i] = new Bitmap(projectLoaded + "//Graphics//Tilesets 3bpp//blockset" + i.ToString("D3") + ".png");
            }

            for (int i = 0; i < 128; i++)
            {
                // Unload
                if (allmaps[i] != null)
                {
                    allmaps[i].Dispose();
                    allmaps[i] = null;
                }

                MapSave map = JsonConvert.DeserializeObject<MapSave>(File.ReadAllText(projectLoaded + "//Overworld//Maps//Map" + i.ToString("D3") + ".json"));
                allmaps[i] = new Map16(allgfx8array, allPalettes, mapInfos, map, tilesetBitmaps);
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            if (projectLoaded != "")
            {
                int ind = 0;
                if (lightworld)
                {
                    ind = 0;
                }
                else
                {
                    ind = 64;
                }
                pe.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                pe.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                pe.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                int xx = 0;
                int yy = 0;
                for (int i = 0; i < 64; i++)
                {
                    pe.Graphics.DrawImage(allmaps[i + ind].mapGfx, new Point(xx * 512, yy * 512));

                    xx++;
                    if (xx >= 8)
                    {
                        yy++;
                        xx = 0;
                    }
                }


            }

        }

        public void DrawPalettes(Graphics g)
        {
            int x = 0;
            int y = 0;
            int p = 0;
            for (int i = 0; i < 256; i++)
            {
                g.FillRectangle(new SolidBrush(allmaps[64].pal.Entries[p]), new Rectangle(x * 8, y * 8, 8, 8));
                x++;
                if (x >= 16)
                {
                    y++;
                    x = 0;
                }
                p++;
            }
        }

    }
}
