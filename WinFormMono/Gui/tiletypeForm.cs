using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    public partial class tiletypeForm : Form
    {
        public tiletypeForm()
        {
            InitializeComponent();
        }

        public byte[][] allTypesSet = new byte[16][];
        IntPtr allgfx8array = Marshal.AllocHGlobal((128 * 512) / 2);
        IntPtr allgfx8array8bpp = Marshal.AllocHGlobal((128 * 512));
        IntPtr tile16gfx8bpp = Marshal.AllocHGlobal(16 * 16);
        Bitmap[] allbitmaps;
        JsonData jsonData;
        Bitmap blocksetsBitmap;
        Bitmap tileBitmap;

        Dictionary<string, byte> typeString = new Dictionary<string, byte>();

        public void setGfxData(Bitmap[] allbitmaps, JsonData jsonData, Map16 map)
        {
            typeString.Clear();
            this.allbitmaps = allbitmaps;
            this.jsonData = jsonData;
            //blocksetsBitmap = new Bitmap(128, 512,64,PixelFormat.Format4bppIndexed, allgfx8array);
            blocksetsBitmap = new Bitmap(128, 512, 128, PixelFormat.Format8bppIndexed, allgfx8array8bpp);
            tileBitmap = new Bitmap(16, 16, 16, PixelFormat.Format8bppIndexed, tile16gfx8bpp);
            blocksetsBitmap.Palette = map.GetPalette();
            tileBitmap.Palette = map.GetPalette();
            //blocksetPicturebox.Image = blocksetsBitmap;
            blocksetpictureBox.Refresh();
            for (int i = 0; i < 16; i++)
            {
                allTypesSet[i] = new byte[512];
                for (int j = 0; j < 512; j++)
                {
                    allTypesSet[i][j] = jsonData.tileTypeSet[i][j];
                }
            }
            typeString.Add("Normal Passable", 0);
            typeString.Add("Normal Blocked", 1);
            typeString.Add("Deep Water", 8);
            typeString.Add("Shallow Water", 9);
            //typeString.Add("Moving Floor - Dungeon Only?", 12);
            typeString.Add("Spike Floor", 13);
            typeString.Add("Ice1 Floor", 14);
            typeString.Add("Ice2 Floor", 15);
            typeString.Add("Hole", 32);
            typeString.Add("Stair Tile (slow down)", 34);
            typeString.Add("Ledge Up", 40);
            typeString.Add("Ledge Down", 41);
            typeString.Add("Ledge Left", 42);
            typeString.Add("Ledge Right", 43);
            typeString.Add("Ledge Up Left", 44);
            typeString.Add("Ledge Down Left", 45);
            typeString.Add("Ledge Up Right", 46);
            typeString.Add("Ledge Down Right", 47);
            typeString.Add("High Grass", 64);
            typeString.Add("Spike Block", 68);
            typeString.Add("Plain Grass (Diggable)", 72);
            typeString.Add("Warp", 75);
            typeString.Add("Bush", 80);
            typeString.Add("Off color Bush", 81);
            typeString.Add("Small Light Rock", 82);
            typeString.Add("Small Heavy Rock", 83);
            typeString.Add("Sign", 84);
            typeString.Add("Large Light Rock", 85);
            typeString.Add("Large Heavy Rock", 86);
            typeString.Add("Chest - not quite working on OW", 88);
            foreach(KeyValuePair<string,byte> s in typeString)
            {
                listBox1.Items.Add(s.Value + ": " + s.Key);
            }

        }

        private void tileUpDown_ValueChanged(object sender, EventArgs e)
        {
            //allTypesSet[(int)tiletypesetUpDown.Value][(int)tileUpDown.Value] = (byte)(selectedtypeUpDown.Value);
            selectedtypeUpDown.Value = allTypesSet[(int)tiletypesetUpDown.Value][(int)tileUpDown.Value];
            blocksetpictureBox.Refresh();
            
        }

        private void tiletypesetUpDown_ValueChanged(object sender, EventArgs e)
        {
            selectedtypeUpDown.Value = allTypesSet[(int)tiletypesetUpDown.Value][(int)tileUpDown.Value];
            blocksetpictureBox.Refresh();
        }

        private void selectedtypeUpDown_ValueChanged(object sender, EventArgs e)
        {
            allTypesSet[(int)tiletypesetUpDown.Value][(int)tileUpDown.Value] = (byte)selectedtypeUpDown.Value;
        }

        private void blocksetpictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            int x = (e.X / 16);
            int y = (e.Y / 16);

            tileUpDown.Value = x + (y * 16);
        }

        private void blocksetpictureBox_Paint(object sender, PaintEventArgs e)
        {
            buildTileset();
            updateTileset();

            e.Graphics.CompositingMode = CompositingMode.SourceCopy;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            e.Graphics.DrawImage(blocksetsBitmap, new Rectangle(0, 0, 256, 1024), 0, 0, 128, 512, GraphicsUnit.Pixel);
            int ty = ((int)tileUpDown.Value / 16);
            int tx = (int)tileUpDown.Value - (ty * 16);
            e.Graphics.DrawRectangle(Pens.LightGreen, new Rectangle(tx * 16, ty * 16, 16, 16));
        }

        private void updateTileset()
        {
            unsafe
            {
                byte* source = (byte*)allgfx8array.ToPointer();
                byte* dest = (byte*)allgfx8array8bpp.ToPointer();
                int xx = 0;
                int yy = 0;
                for (int i = 0; i < 1024; i++)
                {

                    for (var y = 0; y < 8; y++)
                    {
                        for (var x = 0; x < 4; x++)
                        {
                            CopyTile(x, y, xx, yy, 128, (ushort)i, dest, source);
                        }
                    }
                    xx += 8;
                    if (xx >= 128)
                    {
                        yy += 1024;
                        xx = 0;
                    }

                }
            }

        }

        private unsafe void CopyTile(int x, int y, int xx, int yy, int offset, int stride, TileInfo tile, byte* dest, byte* source)
        {
            int mx = x;
            int my = y;
            byte r = 0;

            if (tile.h)
            {
                mx = 3 - x;
                r = 1;
            }
            if (tile.v)
            {
                my = 7 - y;
            }

            int tx = ((tile.id / 16) * 512) + ((tile.id - ((tile.id / 16) * 16)) * 4);
            var index = xx + yy + offset + (mx * 2) + (my * stride);
            var pixel = source[tx + (y * 64) + x];

            dest[index + r ^ 1] = (byte)((pixel & 0x0F) + tile.palette * 16);
            dest[index + r] = (byte)(((pixel >> 4) & 0x0F) + tile.palette * 16);
        }

        private unsafe void CopyTile(int x, int y, int xx, int yy, int stride, int tile, byte* dest, byte* source)
        {
            int mx = x;
            int my = y;
            byte r = 0;

            int tx = ((tile / 16) * 512) + ((tile - ((tile / 16) * 16)) * 4);
            var index = xx + yy + (mx * 2) + (my * stride);
            var pixel = source[tx + (y * 64) + x];

            dest[index + r ^ 1] = (byte)((pixel & 0x0F) + paletteUpDown.Value * 16);
            dest[index + r] = (byte)(((pixel >> 4) & 0x0F) + paletteUpDown.Value * 16);
        }

        private void buildTileset()
        {
            byte[] staticgfx = new byte[16];
            unsafe
            {
                staticgfx[8] = 115 + 0;
                staticgfx[9] = 115 + 10;
                staticgfx[10] = 115 + 6;
                staticgfx[11] = 115 + 7;

                int index = 0x21;

                for (int i = 0; i < 8; i++)
                {
                    staticgfx[i] = jsonData.blocksetGroups2[(index * 8) + i];
                }

                if (jsonData.blocksetGroups[((int)gfxUpDown.Value * 4)] != 0)
                {
                    staticgfx[3] = jsonData.blocksetGroups[((int)gfxUpDown.Value * 4)];
                }
                if (jsonData.blocksetGroups[((int)gfxUpDown.Value * 4) + 1] != 0)
                {
                    staticgfx[4] = jsonData.blocksetGroups[((int)gfxUpDown.Value * 4) + 1];
                }
                if (jsonData.blocksetGroups[((int)gfxUpDown.Value * 4) + 2] != 0)
                {
                    staticgfx[5] = jsonData.blocksetGroups[((int)gfxUpDown.Value * 4) + 2];
                }
                if (jsonData.blocksetGroups[((int)gfxUpDown.Value * 4) + 3] != 0)
                {
                    staticgfx[6] = jsonData.blocksetGroups[((int)gfxUpDown.Value * 4) + 3];
                }


                staticgfx[7] = 91;


                byte* allgfx8Data = (byte*)allgfx8array.ToPointer();
                for (int i = 0; i < 16; i++)
                {
                    Bitmap mapBitmap = allbitmaps[staticgfx[i]];
                    BitmapData mapBitmapData = mapBitmap.LockBits(new Rectangle(0, 0, 128, 32), ImageLockMode.ReadOnly, PixelFormat.Format4bppIndexed);
                    byte* mapData = (byte*)mapBitmapData.Scan0.ToPointer();

                    for (int j = 0; j < 2048; j++)
                    {
                        byte mapByte = mapData[j];

                        switch (i)
                        {
                            case 0:
                            case 3:
                            case 4:
                            case 5:
                                mapByte += 0x88;
                                break;
                        }

                        allgfx8Data[(i * 2048) + j] = mapByte;
                    }
                    mapBitmap.UnlockBits(mapBitmapData);
                }
            }
        }

        private void tiletypeForm_Load(object sender, EventArgs e)
        {

        }

        private void tiletypeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Marshal.FreeHGlobal(allgfx8array);
            Marshal.FreeHGlobal(allgfx8array8bpp);
            Marshal.FreeHGlobal(tile16gfx8bpp);
        }

        private void paletteUpDown_ValueChanged(object sender, EventArgs e)
        {
            blocksetpictureBox.Refresh();
        }

        private void gfxUpDown_ValueChanged(object sender, EventArgs e)
        {
            blocksetpictureBox.Refresh();
        }
        
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            selectedtypeUpDown.Value = typeString.ElementAt(listBox1.SelectedIndex).Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }
    }
}
