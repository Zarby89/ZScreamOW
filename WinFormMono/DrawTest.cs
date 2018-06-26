using System;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;

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
        Scene scene;
        int worldOffset = 0;
        public bool lightworld = true;
        public DrawTest()
        {
            MouseMove += DrawTest_MouseMove;
            MouseDown += DrawTest_MouseDown;
            MouseUp += DrawTest_MouseUp;
        }

        private void DrawTest_MouseUp(object sender, MouseEventArgs e)
        {
            if (scene != null)
            {
                scene.mouseUp(e, allmaps[scene.mouseOverMap + worldOffset]);
                
                int yT = (scene.mouseOverMap / 8);
                int xT = scene.mouseOverMap - (yT * 8);
                Invalidate(new Rectangle(xT * 512, yT * 512, 512, 512));
            }
        }

        private void DrawTest_MouseDown(object sender, MouseEventArgs e)
        {
            if (scene != null)
            {
                scene.mouseDown(e, allmaps[scene.mouseOverMap + worldOffset], allgfx8array);
                
                int yT = (scene.mouseOverMap / 8);
                int xT = scene.mouseOverMap - (yT * 8);
                Invalidate(new Rectangle(xT * 512, yT * 512, 512, 512));
            }
        }

        private void DrawTest_MouseMove(object sender, MouseEventArgs e)
        {
            if (scene != null)
            {
                scene.mouseMove(e, allmaps[scene.mouseOverMap], allgfx8array);
                if (scene.refresh)
                {
                    
                    // Stopwatch sw = new Stopwatch();
                    //sw.Start();
                    //Refresh();
                    int yT = (scene.mouseOverMap / 8);
                    int xT = scene.mouseOverMap - (yT * 8);
                    Invalidate(new Rectangle(xT * 512, yT * 512, 512, 512));
                    if (scene.screenChanged != -1)
                    {
                        scene.UpdateGfx(allgfx8array, allmaps[scene.mouseOverMap + worldOffset]);
                        scene.setOverlaytiles(scene.allgfx16Ptr);
                        yT = (scene.screenChanged / 8);
                        xT = scene.screenChanged - (yT * 8);
                        //invalidate last screen as well to prevent artifact
                        Invalidate(new Rectangle(xT * 512, yT * 512, 512, 512));
                        
                        scene.screenChanged = -1;
                    }

                    scene.refresh = false;

                    //sw.Stop();
                    //Console.WriteLine(sw.ElapsedMilliseconds);
                }
            }

        }

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
            scene = new Scene(tilesetBitmaps);
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
                Stopwatch sw = new Stopwatch();
                sw.Start();
                DrawMaps(pe.Graphics);

                scene.Draw(pe.Graphics);
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);
            }

        }



        public void DrawMaps(Graphics g)
        {
            if (lightworld)
            {
                worldOffset = 0;
            }
            else
            {
                worldOffset = 64;
            }
            g.CompositingMode = CompositingMode.SourceCopy;
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            int xx = 0;
            int yy = 0;
            for (int i = 0; i < 64; i++)
            {
                g.DrawImage(allmaps[i + worldOffset].mapGfx, new Point(xx * 512, yy * 512));
                xx++;
                if (xx >= 8)
                {
                    yy++;
                    xx = 0;
                }
            }

        }

        public void DrawPalettes(Graphics g)
        {
            /*int x = 0;
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
            }*/
        }

    }
}
