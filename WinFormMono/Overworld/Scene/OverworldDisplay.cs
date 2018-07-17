using System;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace WinFormMono
{
    class OverworldDisplay : PictureBox
    {

        public ushort selectedTile = 0;
        Bitmap allgfx8;
        JsonData jsonData;
        public Map16[] allmaps = new Map16[128];
        byte[] mapParent = new byte[128];
        public SceneOverworld scene;
        int worldOffset = 0;
        public bool lightworld = true;
        public Map16 lastSelectedMap = null;
        public Map16 selectedMap = null;
        public bool showGrid = false;

        public OverworldDisplay()
        {
            MouseMove += DrawTest_MouseMove;
            MouseDown += DrawTest_MouseDown;
            MouseUp += DrawTest_MouseUp;
        }

        public void setData(JsonData jsonData)
        {
            this.jsonData = jsonData;
        }
        bool mouseDown = false;
        private void DrawTest_MouseUp(object sender, MouseEventArgs e)
        {
            if (scene != null)
            {
                scene.mouseUp(e, allmaps[scene.mouseOverMap + worldOffset]);
                
                int yT = (scene.mouseOverMap / 8);
                int xT = scene.mouseOverMap - (yT * 8);
                Invalidate(new Rectangle(xT * 512, yT * 512, 512, 512));
            }
            mouseDown = false;
        }

        private void DrawTest_MouseDown(object sender, MouseEventArgs e)
        {
            if (scene != null)
            {
                
                scene.mouseDown(e, allmaps[scene.mouseOverMap + worldOffset], jsonData.allgfx8array);
                
                int yT = (scene.mouseOverMap / 8);
                int xT = scene.mouseOverMap - (yT * 8);
                selectedMap = allmaps[scene.mouseOverMap + worldOffset];
                Invalidate(new Rectangle(xT * 512, yT * 512, 512, 512));
                mouseDown = true;

                
            }
        }


        private void DrawTest_MouseMove(object sender, MouseEventArgs e)
        {
            if (scene != null)
            {
                scene.mouseMove(e, allmaps[(scene.mouseOverMap + worldOffset)], jsonData.allgfx8array);
                if (scene.refresh)
                {
                    
                    // Stopwatch sw = new Stopwatch();
                    //sw.Start();
                    //Refresh();
                    int yT = ((scene.mouseOverMap) / 8);
                    int xT = (scene.mouseOverMap) - (yT * 8);
                    
                    if (allmaps[(scene.mouseOverMap + worldOffset)].largeMap)
                    {
                        yT = (allmaps[(scene.mouseOverMap)].parentMapId / 8);
                        xT = allmaps[(scene.mouseOverMap)].parentMapId - (yT * 8);
                        Invalidate(new Rectangle(xT * 512, yT * 512, 1024, 1024));
                    }
                    else
                    {
                        Invalidate(new Rectangle(xT * 512, yT * 512, 512, 512));
                    }
                    if (scene.screenChanged != -1)
                    {
                        if (mouseDown)
                        {
                            selectedMap = allmaps[(scene.mouseOverMap + worldOffset)];
                        }
                        if (allmaps[(scene.mouseOverMap + worldOffset)].largeMap)
                        {
                            
                            scene.UpdateGfx(jsonData.allgfx8array, allmaps[(scene.mouseOverMap + worldOffset)]);
                        }
                        else
                        {
                            scene.UpdateGfx(jsonData.allgfx8array, allmaps[(scene.mouseOverMap + worldOffset)]);
                        }
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




        public void CreateProject()
        {
            this.Image = new Bitmap(4096, 4096);
            allgfx8 = new Bitmap(128, 512, 64, PixelFormat.Format4bppIndexed, jsonData.allgfx8array); //temporary variable used for all rooms
            
            

           

            getLargeMaps();
            for (int i = 0; i < 128; i++)
            {
                if (allmaps[i] != null)
                {
                    allmaps[i].Dispose();
                    allmaps[i] = null;
                }
                if (jsonData.mapdata[i].largeMap)
                {
                    jsonData.mapdata[i].palette = jsonData.mapdata[mapParent[i]].palette;
                    jsonData.mapdata[i].blockset = jsonData.mapdata[mapParent[i]].blockset;
                }
                allmaps[i] = new Map16(jsonData.allgfx8array, jsonData, jsonData.mapdata[i], jsonData.tilesetBitmaps);
                allmaps[i].index = i;
            }
                setLargeMaps();
            scene = new SceneOverworld(jsonData.tilesetBitmaps, allmaps[0], jsonData.allgfx8array, jsonData);
            scene.SetAllMaps(allmaps);
        }

        public void Undo()
        {
            scene.Undo();
            this.Refresh();
        }

        public void getLargeMaps()
        {
            bool[] mapChecked = new bool[64];
            for(int i = 0;i<64;i++)
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
                    if (jsonData.mapdata[i].largeMap == true)
                    {
                        mapChecked[i] = true;
                        mapParent[i] = (byte)i;
                        mapParent[i+64] = (byte)(i+64);

                        mapChecked[i + 1] = true;
                        mapParent[i + 1] = (byte)i;
                        mapParent[i+65] = (byte)(i+64);

                        mapChecked[i + 8] = true;
                        mapParent[i + 8] = (byte)i;
                        mapParent[i+72] = (byte)(i+64);

                        mapChecked[i + 9] = true;
                        mapParent[i + 9] = (byte)i;
                        mapParent[i+73] = (byte)(i+64);
                        xx++;
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

        public void setLargeMaps()
        {
            //List<byte> largemaps = new List<byte>();
            int xx = 0;
            int yy = 0;
            while(true)
            {

                int i = xx + (yy * 8);
                if (allmaps[i].parentMapId == 255)
                {
                    if (allmaps[i].largeMap == true)
                    {
                        allmaps[i].parentMapId = (byte)i;
                        allmaps[i].parentMap = allmaps[i];
                        allmaps[i + 1].parentMapId = (byte)i;
                        allmaps[i + 1].parentMap = allmaps[i];
                        allmaps[i + 8].parentMapId = (byte)i;
                        allmaps[i + 8].parentMap = allmaps[i];
                        allmaps[i + 9].parentMapId = (byte)i;
                        allmaps[i + 9].parentMap = allmaps[i];

                        allmaps[i+64].parentMapId = (byte)(i+64);
                        allmaps[i+64].parentMap = allmaps[i+64];
                        allmaps[i + 65].parentMapId = (byte)(i+64);
                        allmaps[i + 65].parentMap = allmaps[i+64];
                        allmaps[i + 72].parentMapId = (byte)(i+64);
                        allmaps[i + 72].parentMap = allmaps[i+64];
                        allmaps[i + 73].parentMapId = (byte)(i+64);
                        allmaps[i + 73].parentMap = allmaps[i+64];

                        xx++;
                    }
                }


                xx++;
                if (xx>=8)
                {
                    xx = 0;
                    yy+=1;
                    if (yy >=8)
                    {
                        break;
                    }
                }
            }

            //return largemaps.ToArray();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            if (scene != null)
            {
                DrawMaps(pe.Graphics);
                if (showGrid)
                {
                    DrawGrid(pe.Graphics);
                }

                scene.Draw(pe.Graphics,selectedMap);
                scene.DrawObjects(pe.Graphics);
                


            }
                //sw.Stop();
                //Console.WriteLine(sw.ElapsedMilliseconds);

        }
        
        public void DrawGrid(Graphics g)
        {
            g.CompositingMode = CompositingMode.SourceOver;
            for(int i = 0;i<256;i++)
            {
                g.DrawLine(new Pen(Color.FromArgb(120,255,255,255),1), 0, i * 16, 4096, i * 16);
                g.DrawLine(new Pen(Color.FromArgb(120, 255, 255, 255), 1), i * 16, 0, i * 16, 4096);
            }
            g.CompositingMode = CompositingMode.SourceCopy;
        }

        public void setMode(bool entrances,bool exits,bool sprites,bool tiles,bool holes,bool warp,bool items)
        {
            if (entrances)
            {
                scene.sceneMode = SceneMode.entrances;
            }
            else if (exits)
            {
                scene.sceneMode = SceneMode.exits;
            }
            else if (sprites)
            {
                scene.sceneMode = SceneMode.sprites;
            }
            else if (tiles)
            {
                scene.sceneMode = SceneMode.tiles;
            }
            else if (holes)
            {
                scene.sceneMode = SceneMode.holes;
            }
            else if (warp)
            {
                scene.sceneMode = SceneMode.warps;
            }
            else if (items)
            {
                scene.sceneMode = SceneMode.items;
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
            scene.updateDisplayedMaps(worldOffset);
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
