using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormMono
{
    class Scene
    {
        int mouse_x;
        int mouse_y;
        int last_mouse_tile_x;
        int last_mouse_tile_y;
        int mouse_tile_x;
        int mouse_tile_y;
        int mouse_tile_x_down;
        int mouse_tile_y_down;
        bool mouse_down = false;
        public int mouseOverMap;
        public int last_mouseOverMap;
        public bool refresh = false;
        public int screenChanged = -1;
        public IntPtr allgfx16Ptr = Marshal.AllocHGlobal(128 * 7520);
        IntPtr selectedTilesGfxPtr = Marshal.AllocHGlobal(512 * 512);
        Bitmap selectedTilesGfx;
        Bitmap[] allbitmaps;
        ushort[,] selectedTiles;
        bool allowCopy = false;

        Rectangle selectionSize = new Rectangle(0, 0, 16, 16);
        public Scene(Bitmap[] allbitmaps)
        {
            this.allbitmaps = allbitmaps;
            selectedTilesGfx = new Bitmap(512, 512, 512, PixelFormat.Format8bppIndexed, selectedTilesGfxPtr);
            selectedTiles = new ushort[2, 2];
            selectedTiles[0, 0] = 200; selectedTiles[1, 0] = 301;
            selectedTiles[0, 1] = 503; selectedTiles[1, 1] = 404;
        }

        public void mouseMove(MouseEventArgs e, Map16 map, IntPtr allgfx8array)
        {
            
            if (e.X >= 0 && e.Y >= 0 && e.X < 4096 && e.Y < 4096)
            {
                mouse_x = e.X;
                mouse_y = e.Y;
                mouse_tile_x = e.X / 16;
                mouse_tile_y = e.Y / 16;
                if (mouse_x != last_mouse_tile_x || mouse_y != last_mouse_tile_y)
                {
                    Scene_MouseTileChanged(e,map, allgfx8array); //the mouse is not hovering the same tile
                }
                last_mouse_tile_x = mouse_x;
                last_mouse_tile_y = mouse_y;
                
            }
        }
        public void UpdateGfx(IntPtr allgfx8array, Map16 map)
        {
            map.UpdateGfx(allgfx8array, allgfx16Ptr, allbitmaps);
        }

        public void Scene_MouseTileChanged(MouseEventArgs e, Map16 map, IntPtr allgfx8array)
        {
            int mx = (mouse_tile_x / 32);
            int my = (mouse_tile_y / 32);
            mouseOverMap = mx + (my * 8);
            selectionSize.X = mouse_tile_x*16;
            selectionSize.Y = mouse_tile_y*16;
            selectedTilesGfx.Palette = map.GetPalette();
            if (mouseOverMap != last_mouseOverMap)
            {
                setOverlaytiles(allgfx16Ptr);
                screenChanged = last_mouseOverMap;
                last_mouseOverMap = mouseOverMap;
            }
            else if (mouse_down)
            {
                if (e.Button == MouseButtons.Left)
                {
                    map.setTiles(allgfx16Ptr, mouse_tile_x - (mx * 32), mouse_tile_y - (my * 32), selectedTiles);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (screenChanged!=-1)
                    {
                        allowCopy = false;
                        mouse_down = false;
                    }
                    else
                    {
                        allowCopy = true;
                    }
                    int startX = mouse_tile_x_down;
                    int startY = mouse_tile_y_down;
                    int sizeX = 0;
                    int sizeY = 0;
                    int mouse_x_map = mouse_tile_x - (mx * 32);
                    int mouse_y_map = mouse_tile_y - (my * 32);

                    if (mouse_tile_x_down < mouse_x_map)
                    {
                        sizeX = (mouse_x_map - mouse_tile_x_down) + 1;
                    }
                    else
                    {
                        startX = mouse_x_map;
                        sizeX = Math.Abs(mouse_x_map - mouse_tile_x_down) + 1;
                    }
                    
                    if (mouse_tile_y_down < mouse_y_map)
                    {
                        sizeY = (mouse_y_map - mouse_tile_y_down) + 1;
                    }
                    else
                    {
                        startY = mouse_y_map;
                        sizeY = Math.Abs(mouse_y_map - mouse_tile_y_down) + 1;
                    }
                    selectionSize = new Rectangle(((startX+(mx * 32)) * 16) , ((startY + (my * 32)) * 16), (sizeX * 16), (sizeY * 16));
                    //overlayGraphics.DrawRectangle(new Pen(Brushes.Yellow), new Rectangle(startX * 16, startY * 16, (sizeX * 16), (sizeY * 16)));
                }
            }

            refresh = true;
        }


        public void mouseDown(MouseEventArgs e, Map16 map,IntPtr allgfx8array)
        {
            mouse_down = true;
            int yT = (mouseOverMap / 8);
            int xT = mouseOverMap - (yT * 8);
            int mx = (mouse_tile_x / 32);
            int my = (mouse_tile_y / 32);
            if (e.Button == MouseButtons.Right)
            {
                mouse_tile_x_down = mouse_tile_x-(mx*32);
                mouse_tile_y_down = mouse_tile_y-(my*32);
                allowCopy = true;
                selectionSize = new Rectangle(mouse_tile_x_down * 16, mouse_tile_y_down * 16, 16, 16);
            }
            else if (e.Button == MouseButtons.Left)
            {
                map.setTiles(allgfx16Ptr, mouse_tile_x - (mx * 32), mouse_tile_y - (my * 32), selectedTiles);
            }
            map.UpdateMap(allgfx16Ptr);
            //setOverlaytiles(allgfx16Ptr);
            //map.setTile(allgfx16Ptr,mouse_tile_x, mouse_tile_y, 500);

        }

        public void mouseUp(MouseEventArgs e, Map16 map)
        {
            mouse_down = false;
            int mx = (mouse_tile_x / 32);
            int my = (mouse_tile_y / 32);
            int mouse_x_map = mouse_tile_x - (mx * 32);
            int mouse_y_map = mouse_tile_y - (my * 32);
            if (e.Button == MouseButtons.Right)
            {
                if (allowCopy)
                {
                    int sizeX = 0;
                    int sizeY = 0;
                    if (mouse_tile_x_down < mouse_x_map)
                    {
                        sizeX = (mouse_x_map - mouse_tile_x_down) + 1;
                    }
                    else
                    {
                        int oldmdownx = mouse_tile_x_down;
                        mouse_tile_x_down = mouse_x_map;
                        mouse_x_map = oldmdownx;
                        sizeX = (mouse_x_map - mouse_tile_x_down) + 1;
                    }
                    if (mouse_tile_y_down < mouse_y_map)
                    {
                        sizeY = (mouse_y_map - mouse_tile_y_down) + 1;
                    }
                    else
                    {
                        int oldmdowny = mouse_tile_y_down;
                        mouse_tile_y_down = mouse_y_map;
                        mouse_y_map = oldmdowny;
                        sizeY = (mouse_y_map - mouse_tile_y_down) + 1;
                    }

                    //selectedTiles = new ushort[sizeX, sizeY];
                    /*for (int x = 0; x < sizeX; x++)
                    {
                        for (int y = 0; y < sizeY; y++)
                        {

                        }
                    }*/
                    selectionSize = new Rectangle(mouse_tile_x_down*16, mouse_tile_y_down*16, (sizeX * 16), (sizeY * 16));
                    selectedTiles = map.getTiles(mouse_tile_x_down, mouse_tile_y_down, sizeX, sizeY);
                    setOverlaytiles(allgfx16Ptr);
                }
            }
        }

        public unsafe void setOverlaytiles(IntPtr allgfx16Ptr)
        {

            byte* gfxData = (byte*)selectedTilesGfxPtr.ToPointer();
            byte* gfx16Data = (byte*)allgfx16Ptr.ToPointer();

            for (int xx = 0; xx < selectedTiles.GetLength(0); xx++)
            {
                for (int yy = 0; yy < selectedTiles.GetLength(1); yy++)
                {
                    int mapPos = GetTilePos(xx, yy);
                    if (mapPos != -1)
                    {
                       // tile[xx, yy];
                        for (int i = 0; i < 16; i++)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                gfxData[((xx) * 16) + ((yy) * 8192) + j + (i * 512)] = gfx16Data[mapPos + j + (i * 128)];
                            }
                        }
                    }
                }
            }
        }

        public int GetTilePos(int x, int y)
        {
            if (x > 31){return -1;}
            if (y > 31){return -1;}
            return ((selectedTiles[x, y] / 8) * 2048) + ((selectedTiles[x, y] - ((selectedTiles[x, y] / 8) * 8)) * 16);
        }

        public void Draw(Graphics g)
        {

            int yT = (mouseOverMap / 8);
            int xT = mouseOverMap - (yT * 8);

            
           
            g.CompositingMode = CompositingMode.SourceOver;
            if (!mouse_down)
            {
                ColorMatrix cm = new ColorMatrix();
                cm.Matrix33 = 0.50f;
                cm.Matrix22 = 2f;
                ImageAttributes ia = new ImageAttributes();
                ia.SetColorMatrix(cm);
                g.DrawImage(selectedTilesGfx, new Rectangle((mouse_tile_x * 16), (mouse_tile_y * 16), (selectedTiles.GetLength(0) * 16), (selectedTiles.GetLength(1) * 16)), 0, 0, (selectedTiles.GetLength(0) * 16), (selectedTiles.GetLength(1) * 16), GraphicsUnit.Pixel, ia);
            }
            g.CompositingMode = CompositingMode.SourceCopy;
            g.DrawRectangle(Pens.LightGreen, selectionSize);
            g.DrawRectangle(Pens.DarkOrange, new Rectangle(xT * 512, yT * 512, 511, 511));

            DrawText(g, selectionSize.ToString(), new Point(xT * 512, yT * 512));
        }

        public void DrawText(Graphics g, string text, Point position)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            GraphicsPath gpath = new GraphicsPath();
            gpath.AddString(text, new FontFamily("Courier New"), 1, 12, position, StringFormat.GenericDefault);
            Pen pen = new Pen(Color.FromArgb(30, 30, 30), 2);
            g.DrawPath(pen, gpath);
            SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 255));
            g.FillPath(brush, gpath);
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
        }

    }
}
