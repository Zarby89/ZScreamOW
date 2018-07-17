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
    public partial class Tile16EditorForm : Form
    {
        public Tile16EditorForm()
        {
            InitializeComponent();
        }

        private void Tile16EditorForm_Load(object sender, EventArgs e)
        {

        }
        IntPtr allgfx8array = Marshal.AllocHGlobal((128 * 512) / 2);
        IntPtr allgfx8array8bpp = Marshal.AllocHGlobal((128 * 512));
        IntPtr tile16gfx8bpp = Marshal.AllocHGlobal(16*16);
        Bitmap[] allbitmaps;
        JsonData jsonData;
        public Tile16 editingTile;
        Map16 editingMap;
        Bitmap blocksetsBitmap;
        Bitmap tileBitmap;
        TileInfo selectedTile = new TileInfo(0,0,false,false,false);
        public void setGfxData(Bitmap[] allbitmaps,JsonData jsonData, Map16 map,ushort tile)
        {
            this.Text = "Tile 16 editor - Editing tile : " + tile.ToString();
            this.allbitmaps = allbitmaps;
            this.jsonData = jsonData;
            editingTile = new Tile16(jsonData.alltiles16[tile].Info[0], jsonData.alltiles16[tile].Info[1],
                jsonData.alltiles16[tile].Info[2], jsonData.alltiles16[tile].Info[3]);

            editingMap = map;
            tilePicturebox.Refresh();
            //blocksetsBitmap = new Bitmap(128, 512,64,PixelFormat.Format4bppIndexed, allgfx8array);
            blocksetsBitmap = new Bitmap(128, 512, 128, PixelFormat.Format8bppIndexed, allgfx8array8bpp);
            tileBitmap = new Bitmap(16, 16, 16, PixelFormat.Format8bppIndexed, tile16gfx8bpp);
            blocksetsBitmap.Palette = map.GetPalette();
            tileBitmap.Palette = map.GetPalette();
            //blocksetPicturebox.Image = blocksetsBitmap;
            blocksetPicturebox.Refresh();
            tilePicturebox.Refresh();
            
        }

        private void tilePicturebox_Paint(object sender, PaintEventArgs e)
        {
            BuildTiles16Gfx();
            e.Graphics.CompositingMode = CompositingMode.SourceCopy;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            e.Graphics.DrawImage(tileBitmap, new Rectangle(0, 0, 64, 64), 0, 0, 16, 16, GraphicsUnit.Pixel);
            e.Graphics.DrawLine(Pens.WhiteSmoke, 32, 0, 32, 64);
            e.Graphics.DrawLine(Pens.WhiteSmoke, 0, 32, 64, 32);
        }

        private void blocksetPicturebox_Paint(object sender, PaintEventArgs e)
        {
            buildTileset();
            updateTileset();

             e.Graphics.CompositingMode = CompositingMode.SourceCopy;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            e.Graphics.DrawImage(blocksetsBitmap, new Rectangle(0,0,256,1024),0,0,128,512,GraphicsUnit.Pixel);
            int ty = (selectedTile.id / 16);
            int tx = selectedTile.id - (ty * 16);
            e.Graphics.DrawRectangle(Pens.LightGreen, new Rectangle(tx*16, ty*16, 16, 16));

        }

        private unsafe void drawTile16()
        {
            

            //Draw tile0


        }

        private unsafe void BuildTiles16Gfx()
        {
            byte* source = (byte*)allgfx8array.ToPointer();
            byte* dest = (byte*)tile16gfx8bpp.ToPointer();
            int[] offsets = { 0, 8, 128, 136};
            var yy = 0;
            var xx = 0;

            //8x8 tile draw
            //gfx8 = 4bpp so everyting is /2
            var tiles = editingTile;

            for (var tile = 0; tile < 4; tile++)
            {
                TileInfo info = tiles.Info[tile];
                int offset = offsets[tile];

                for (var y = 0; y < 8; y++)
                {
                    for (var x = 0; x < 4; x++)
                    {
                        CopyTile(x, y, xx, yy, offset,16, info, dest, source);
                    }
                }
            }

        }

        private unsafe void CopyTile(int x, int y, int xx, int yy, int offset,int stride, TileInfo tile, byte* dest, byte* source)
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

        private unsafe void CopyTile(int x, int y, int xx, int yy,int stride,int tile, byte* dest, byte* source)
        {
            int mx = x;
            int my = y;
            byte r = 0;

            if (mirrorxCheckbox.Checked)
            {
                mx = 3 - x;
                r = 1;
            }
            if (mirroryCheckbox.Checked)
            {
                my = 7 - y;
            }

            int tx = ((tile / 16) * 512) + ((tile - ((tile / 16) * 16)) * 4);
            var index = xx + yy + (mx * 2) + (my * stride);
            var pixel = source[tx + (y * 64) + x];

            dest[index + r ^ 1] = (byte)((pixel & 0x0F) + paletteUpDown.Value * 16);
            dest[index + r] = (byte)(((pixel >> 4) & 0x0F) + paletteUpDown.Value * 16);
        }

        private void updateTileset()
        {
            unsafe
            {
                /* byte* allgfx8Data = (byte*)allgfx8array.ToPointer();
                 byte* allgfx8Data8bpp = (byte*)allgfx8array8bpp.ToPointer();
                 int pal = 0;
                 for (int i = 0; i < 32768; i++)
                 {
                     if (i >= 16384)
                     {
                         pal = 8;
                     }

                     allgfx8Data8bpp[(i * 2) + 1] = (byte)((allgfx8Data[i] & 0x0F) + (paletteUpDown.Value+pal) * 16 );
                     allgfx8Data8bpp[(i * 2)] = (byte)(((allgfx8Data[i] >> 4) & 0x0F) + (paletteUpDown.Value+pal) * 16);
                 }
                 */
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





        private void buildTileset()
        {
            unsafe
            {
                byte* allgfx8Data = (byte*)allgfx8array.ToPointer();
                for (int i = 0; i < 16; i++)
                {
                    Bitmap mapBitmap = allbitmaps[editingMap.staticgfx[i]];
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





        private void Tile16EditorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Marshal.FreeHGlobal(allgfx8array);
            Marshal.FreeHGlobal(allgfx8array8bpp);
            Marshal.FreeHGlobal(tile16gfx8bpp);
        }

        private void paletteUpDown_ValueChanged(object sender, EventArgs e)
        {
            blocksetPicturebox.Refresh();
        }

        private void mirrorxCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            blocksetPicturebox.Refresh();
        }

        private void tilePicturebox_MouseDown(object sender, MouseEventArgs e)
        {
            int tileId = 0;
            if (e.Y >= 32 && e.Y < 64)
            {
                tileId += 2;
            }
            if (e.X >= 32 && e.X < 64)
            {
                tileId += 1;
            }
            if (e.Button == MouseButtons.Left)
            {
                editingTile.Info[tileId] = new TileInfo((short)tileidUpDown.Value, (byte)paletteUpDown.Value, mirroryCheckbox.Checked, mirrorxCheckbox.Checked, ontopCheckbox.Checked);
                tilePicturebox.Refresh();
            }
            else if (e.Button == MouseButtons.Right)
            {
                selectedTile = editingTile.Info[tileId];
                mirrorxCheckbox.Checked = selectedTile.h;
                mirroryCheckbox.Checked = selectedTile.v;
                paletteUpDown.Value = selectedTile.palette;
                tileidUpDown.Value = selectedTile.id;
                blocksetPicturebox.Refresh();
            }
        }

        private void tileidUpDown_ValueChanged(object sender, EventArgs e)
        {
            selectedTile.id = (short)tileidUpDown.Value;
            blocksetPicturebox.Refresh();
        }

        private void blocksetPicturebox_MouseDown(object sender, MouseEventArgs e)
        {
            int x = (e.X / 16);
            int y = (e.Y / 16);
            
            tileidUpDown.Value = x + (y * 16);
        }

        private void tiletypeButton_Click(object sender, EventArgs e)
        {
            tiletypeForm ttf = new tiletypeForm();
            ttf.setGfxData(allbitmaps, jsonData,editingMap);
            if (ttf.ShowDialog() == DialogResult.OK)
            {
                //Set Tile Types tiles
                for(int i = 0;i<16;i++)
                {
                    for (int j = 0; j < 512; j++)
                    {
                        jsonData.tileTypeSet[i][j] = ttf.allTypesSet[i][j];
                    }
                }
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
