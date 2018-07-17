using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WinFormMono
{
    public class Map16 : IDisposable
    {
        public Bitmap mapGfx;
        public int index = 0;
        IntPtr mapGfxPtr = Marshal.AllocHGlobal(512 * 512);
        public MapSave mapdata;
        Color[] currentPalette = new Color[256];
        JsonData jsonData;
        public bool largeMap = false;
        public Map16 parentMap; //for large map
        public byte parentMapId = 255;
        public byte[] staticgfx = new byte[] { 58, 59, 60, 61, 0, 0, 89, 91, 0, 0, 0, 0, 0, 0, 0, 0 };
        public Map16(IntPtr allgfx8Ptr, JsonData jsonData, MapSave mapdata, Bitmap[] allBitmaps)
        {
            this.mapdata = mapdata;
            this.jsonData = jsonData;
            this.largeMap = mapdata.largeMap;
            IntPtr allgfx16Ptr = Marshal.AllocHGlobal(128 * 7520);
            //Bitmap allgfx16Bitmap = new Bitmap(128, 7520, 128, PixelFormat.Format8bppIndexed, allgfx16Ptr);
            mapGfx = new Bitmap(512, 512, 512, PixelFormat.Format8bppIndexed, mapGfxPtr);

            LoadPalette(jsonData.allPalettes);
            Buildtileset(allgfx8Ptr, allBitmaps);
            BuildTiles16Gfx(allgfx8Ptr, allgfx16Ptr);
            BuildMap(allgfx16Ptr);

            Marshal.FreeHGlobal(allgfx16Ptr);
        }

        public unsafe void setTiles(IntPtr allgfx16Ptr, int x, int y, ushort[,] tile)
        {
            byte* gfxData = (byte*)mapGfxPtr.ToPointer();
            byte* gfx16Data = (byte*)allgfx16Ptr.ToPointer();
            for (int xx = 0; xx < tile.GetLength(0); xx++)
            {
                for (int yy = 0; yy < tile.GetLength(1); yy++)
                {
                    int mapPos = GetTilePos(x+xx, y+yy);
                    if (mapPos != -1)
                    {
                        mapdata.tiles[x + xx, y + yy] = tile[xx, yy];
                        for (int i = 0; i < 16; i++)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                gfxData[((x + xx) * 16) + ((y + yy) * 8192) + j + (i * 512)] = gfx16Data[mapPos + j + (i * 128)];
                            }
                        }
                    }
                }
            }
        }

        public ushort[,] getTiles(int x, int y, int w, int h)
        {
            ushort[,] tiles = new ushort[w, h];
            for(int xx = 0; xx < w; xx++)
            {
                for (int yy = 0; yy < h; yy++)
                {
                    tiles[xx, yy] = mapdata.tiles[xx+x,yy+y];
                }
            }

            return tiles;
        }

        public unsafe void UpdateGfx(IntPtr allgfx8Ptr, IntPtr allgfx16Ptr,Bitmap[] allBitmaps)
        {
            if (largeMap)
            {
                if (index != parentMapId)
                {
                    jsonData.mapdata[index].blockset = jsonData.mapdata[parentMapId].blockset;
                    jsonData.mapdata[index].sprite_palette = jsonData.mapdata[parentMapId].sprite_palette;
                    jsonData.mapdata[index].spriteset = jsonData.mapdata[parentMapId].spriteset;
                    jsonData.mapdata[index].palette = jsonData.mapdata[parentMapId].palette;
                    jsonData.mapdata[index].msgid = jsonData.mapdata[parentMapId].msgid;
                }
            }
            Buildtileset(allgfx8Ptr, allBitmaps);
            BuildTiles16Gfx(allgfx8Ptr, allgfx16Ptr);
        }

        public unsafe void UpdateMap(IntPtr allgfx16Ptr)
        {
            BuildMap(allgfx16Ptr);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(mapGfxPtr);
        }

        public unsafe void BuildMap(IntPtr allgfx16Ptr)
        {
            byte* gfxData = (byte*)mapGfxPtr.ToPointer();
            byte* gfx16Data = (byte*)allgfx16Ptr.ToPointer();

            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    int mapPos = GetTilePos(x, y);
                    if (mapPos != -1)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                gfxData[(x * 16) + (y * 8192) + j + (i * 512)] = gfx16Data[mapPos + j + (i * 128)];
                            }
                        }
                    }
                }
            }
        }

        public int GetTilePos(int x, int y)
        {
            if (x > 31)
            {
                return -1;
            }
            if (y > 31)
            {
                return -1;
            }
            ushort tile = mapdata.tiles[x, y];
            return ((tile / 8) * 2048) + ((tile - ((tile / 8) * 8)) * 16);
        }

        public ColorPalette GetPalette()
        {
            return mapGfx.Palette;
        }

        /*public Room_Sprite[] GetSprites()
        {
            return mapdata.sprites;
        }*/

        private unsafe void BuildTiles16Gfx(IntPtr allgfx8Ptr, IntPtr allgfx16Ptr)
        {
            var gfx16Data = (byte*)allgfx16Ptr.ToPointer();
            var gfx8Data = (byte*)allgfx8Ptr.ToPointer();
            int[] offsets = { 0, 8, 1024, 1032 };
            var yy = 0;
            var xx = 0;

            for (var i = 0; i < 3748; i++) //number of tiles16
            {
                //8x8 tile draw
                //gfx8 = 4bpp so everyting is /2
                var tiles = jsonData.alltiles16[i];

                for (var tile = 0; tile < 4; tile++)
                {
                    TileInfo info = tiles.Info[tile];
                    int offset = offsets[tile];

                    for (var y = 0; y < 8; y++)
                    {
                        for (var x = 0; x < 4; x++)
                        {
                            CopyTile(x, y, xx, yy, offset, info, gfx16Data, gfx8Data);
                        }
                    }
                }

                xx += 16;
                if (xx >= 128)
                {
                    yy += 2048;
                    xx = 0;
                }
            }
        }

        private unsafe void CopyTile(int x, int y, int xx, int yy, int offset, TileInfo tile, byte* gfx16Pointer, byte* gfx8Pointer)
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
            var index = xx + yy + offset + (mx * 2) + (my * 128);
            var pixel = gfx8Pointer[tx + (y * 64) + x];

            gfx16Pointer[index + r ^ 1] = (byte)((pixel & 0x0F) + tile.palette * 16);
            gfx16Pointer[index + r] = (byte)(((pixel >> 4) & 0x0F) + tile.palette * 16);
        }

        public void LoadPalette(PaletteHandler allpalettes)
        {
            int paletteid = mapdata.palette;
            if (paletteid >= 0xA3)
            {
                paletteid = 0xA3;
            }

            byte pal0 = 0;
            byte pal1 = allpalettes.palettesGroups[paletteid]; //aux1
            byte pal2 = allpalettes.palettesGroups[paletteid + 1]; //aux2
            byte pal3 = allpalettes.palettesGroups[paletteid + 2]; //animated

            Color[] aux1, aux2, main, animated, hud;
            Color bgr = allpalettes.bgrcolor[0];
            if (pal1 != 255)
            {
                aux1 = allpalettes.auxPalettes[pal1];
            }
            else
            {
                aux1 = allpalettes.auxPalettes[0];
            }
            if (pal2 != 255)
            {
                aux2 = allpalettes.auxPalettes[pal2];
            }
            else
            {
                aux2 = allpalettes.auxPalettes[0];
            }
            if (pal3 == 255)
            {
                pal3 = 0;
            }
            if (pal3 >= (allpalettes.animatedPalettes.Length/7))
            {
                pal3 = 0;
            }



            if (mapdata.index < 0x40)
            {
                //default LW Palette
                pal0 = 0;
                bgr = allpalettes.bgrcolor[0];
                //hardcoded LW DM palettes if we are on one of those maps (might change it to read game code)
                if ((mapdata.index >= 0x03 && mapdata.index <= 0x07))
                {
                    pal0 = 2;
                }
                else if (mapdata.index >= 0x0B && mapdata.index <= 0x0E)
                {
                    pal0 = 2;
                }
            }
            else if (mapdata.index >= 0x40 && mapdata.index < 0x80)
            {
                bgr = allpalettes.bgrcolor[1];
                //default DW Palette
                pal0 = 1;
                //hardcoded DW DM palettes if we are on one of those maps (might change it to read game code)
                if (mapdata.index >= 0x43 && mapdata.index <= 0x47)
                {
                    pal0 = 3;
                }
                else if (mapdata.index >= 0x4B && mapdata.index <= 0x4E)
                {
                    pal0 = 3;
                }
            }
            else if (mapdata.index >= 132) //special area like Zora's domain, etc...
            {
                bgr = allpalettes.bgrcolor[2];
                pal0 = 4;
            }


            if (pal0 != 255)
            {
                main = allpalettes.mainPalettes[pal0];
            }
            else
            {
                main = allpalettes.mainPalettes[0];
            }

            animated = new Color[7];

            for (int i = 0; i < 7; i++)
            {
                animated[i] = allpalettes.animatedPalettes[(pal3 * 7) + i];
            }

            hud = allpalettes.hudPalettes[0];

            SetColorsPalette(main, animated, aux1, aux2, hud, bgr);
        }

        private void SetColorsPalette(Color[] main, Color[] animated, Color[] aux1, Color[] aux2, Color[] hud, Color bgrcolor)
        {
            //Palettes infos, color 0 of a palette is always transparent (the arrays contains 7 colors width wide)
            //there is 16 color per line so 16*Y

            //Left side of the palette - Main, Animated

            //Main Palette, Location 0,2 : 35 colors [7x5]
            int k = 0;
            for (int y = 2; y < 7; y++)
            {
                for (int x = 1; x < 8; x++)
                {
                    currentPalette[x + (16 * y)] = main[k];
                    k++;
                }
            }

            //Animated Palette, Location 0,7 : 7colors
            for (int x = 1; x < 8; x++)
            {
                currentPalette[(16 * 7) + (x)] = animated[(x - 1)];
            }


            //Right side of the palette - Aux1, Aux2 

            //Aux1 Palette, Location 8,2 : 21 colors [7x3]
            k = 0;
            for (int y = 2; y < 5; y++)
            {
                for (int x = 9; x < 16; x++)
                {
                    currentPalette[x + (16 * y)] = aux1[k];
                    k++;
                }
            }

            //Aux2 Palette, Location 8,5 : 21 colors [7x3]
            k = 0;
            for (int y = 5; y < 8; y++)
            {
                for (int x = 9; x < 16; x++)
                {
                    currentPalette[x + (16 * y)] = aux2[k];
                    k++;
                }
            }

            //Hud Palette, Location 0,0 : 32 colors [16x2]
            k = 0;
            for (int i = 0; i < 32; i++)
            {
                currentPalette[i] = hud[i];
            }

            //Hardcoded grass color (that might change to become invisible instead)
            for (int i = 0; i < 8; i++)
            {
                currentPalette[(i * 16)] = bgrcolor;
                currentPalette[(i * 16) + 8] = bgrcolor;
            }

            
            //Sprite Palettes
            k = (1 * 7);
            for (int y = 8; y < 9; y++)
            {
                for (int x = 1; x < 8; x++)
                {
                    currentPalette[x + (16 * y)] = jsonData.allPalettes.aux2spritePalettes[k];
                    k++;
                }
            }

            //Sprite Palettes
            k = (7*7);
            for (int y = 8; y < 9; y++)
            {
                for (int x = 9; x < 16; x++)
                {
                    currentPalette[x + (16 * y)] = jsonData.allPalettes.aux3spritePalettes[k];
                    k++;
                }
            }


            //Sprite Palettes
            k = 0;
            for (int y = 9; y < 13; y++)
            {
                for (int x = 1; x < 16; x++)
                {
                    currentPalette[x + (16 * y)] = jsonData.allPalettes.spritePalettes[0][k];
                    k++;
                }
            }

            ColorPalette pal = mapGfx.Palette;
            for (int i = 0; i < 256; i++)
            {
                pal.Entries[i] = currentPalette[i];
            }
            mapGfx.Palette = pal;
        }
        
        private void Buildtileset(IntPtr allgfx8array, Bitmap[] allBitmaps)
        {
            staticgfx[8] = 115 + 0;
            staticgfx[9] = 115 + 10;
            staticgfx[10] = 115 + 6;
            staticgfx[11] = 115 + 7;

            int index = 0x21;
            if (mapdata.index < 0x40)
            {
                index = 0x20;
            }
            else if (mapdata.index >= 0x40)//&& mapdata.index < 0x80)
            {
                index = 0x21;
            }
            /*else
            {
                index = 0x24;
            }*/

            for (int i = 0; i < 8; i++)
            {
                staticgfx[i] = jsonData.blocksetGroups2[(index * 8) + i];
            }

            for (int i = 0; i < 4; i++)
            {
                staticgfx[12 + i] = (byte)(jsonData.spritesetGroups[((mapdata.spriteset) * 4) + i] + 115);
            }

            if (jsonData.blocksetGroups[(mapdata.blockset * 4)] != 0)
            {
                staticgfx[3] = jsonData.blocksetGroups[(mapdata.blockset * 4)];
            }
            if (jsonData.blocksetGroups[(mapdata.blockset * 4) + 1] != 0)
            {
                staticgfx[4] = jsonData.blocksetGroups[(mapdata.blockset * 4) + 1];
            }
            if (jsonData.blocksetGroups[(mapdata.blockset * 4) + 2] != 0)
            {
                staticgfx[5] = jsonData.blocksetGroups[(mapdata.blockset * 4) + 2];
            }
            if (jsonData.blocksetGroups[(mapdata.blockset * 4) + 3] != 0)
            {
                staticgfx[6] = jsonData.blocksetGroups[(mapdata.blockset * 4) + 3];
            }

            if ((mapdata.index >= 0x03 && mapdata.index <= 0x07) || (mapdata.index >= 0x0B && mapdata.index <= 0x0E))
            {
                staticgfx[7] = 89;
            }
            else if ((mapdata.index >= 0x43 && mapdata.index <= 0x47) || (mapdata.index >= 0x4B && mapdata.index <= 0x4E))
            {
                staticgfx[7] = 89;
            }
            else
            {
                staticgfx[7] = 91;
            }

            if (mapdata.index >= 128 & mapdata.index < 148)
            {
                staticgfx[4] = 71;
                staticgfx[5] = 72;
            }

            unsafe
            {
                byte* allgfx8Data = (byte*)allgfx8array.ToPointer();
                for (int i = 0; i < 16; i++)
                {
                    Bitmap mapBitmap = allBitmaps[staticgfx[i]];
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
    }
}
