
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinFormMono
{
    class Map16
    {
        
        IntPtr allgfx16array = Marshal.AllocHGlobal(962560);
        public ColorPalette pal;
        IntPtr mapGfxArray = Marshal.AllocHGlobal(262144);
        public Bitmap mapGfx;
        Bitmap allgfx16;
        MapSave mapdata;
        Color[] currentPalette = new Color[256];
        MapInfos mapinfos;
        public Map16(IntPtr allgfx8array, PaletteHandler allpalettes, MapInfos mapinfos, MapSave mapdata,Bitmap[] allBitmaps)
        {
            this.mapdata = mapdata;
            this.mapinfos = mapinfos;
            allgfx16 = new Bitmap(128, 7520, 128, PixelFormat.Format8bppIndexed, allgfx16array);
            mapGfx = new Bitmap(512, 512, 512, PixelFormat.Format8bppIndexed, mapGfxArray);

            loadPalette(allpalettes);
            buildtileset(allgfx8array, allBitmaps);
            buildTiles16Gfx(allgfx8array);
            buildMap();
        }

        public void buildMap()
        {
            unsafe
            {
                byte* bytes = (byte*)mapGfxArray.ToPointer();
                byte* bytes2 = (byte*)allgfx16array.ToPointer();

                for (int x = 0; x < 32; x++)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        int mapPos = getTilePos(x, y);
                        for (int i = 0; i < 16; i++)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                bytes[(x * 16) + (y * 8192) + j + (i * 512)] = bytes2[mapPos + j + (i * 128)];
                            }
                        }
                    }
                }
            }
        }

        public int getTilePos(int x, int y)
        {
            ushort tile = mapdata.tiles[x, y];
            return ((tile / 8) * 2048) + ((tile - ((tile / 8) * 8)) * 16);

        }

        public void buildTiles16Gfx(IntPtr allgfx8array)
        {
            unsafe
            {
                byte* bytePointer = (byte*)allgfx16array.ToPointer();
                byte* bytePointer2 = (byte*)allgfx8array.ToPointer();
                uint yy = 0;
                int xx = 0;
                for (int i = 0; i < 3748; i++) //number of tiles16
                {

                    //8x8 tile draw
                    //gfx8 = 4bpp so everyting is /2
                    int mx;
                    int my;
                    byte r = 0;
                    TileInfo tile;


                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            mx = x;
                            my = y;
                            r = 0;
                            tile = mapinfos.alltiles16[i].tile0;
                            if (tile.h)
                            {
                                mx = 3 - x;
                                r = 1;
                            }
                            if (tile.v)
                            {
                                my = (7 - y);
                            }
                            int tx = ((tile.id / 16) * 512) + ((tile.id - ((tile.id / 16) * 16)) * 4);
                            bytePointer[xx + yy + (mx * 2) + (my * 128) + r ^ 1] = (byte)((bytePointer2[tx + x + (y * 64)] & 0x0F) + (tile.palette) * 16);
                            bytePointer[xx + yy + (mx * 2) + (my * 128) + r] = (byte)(((bytePointer2[tx + x + (y * 64)] >> 4) & 0x0F) + (tile.palette) * 16);

                            mx = x;
                            my = y;
                            tile = mapinfos.alltiles16[i].tile1;
                            r = 0;
                            if (tile.h)
                            {
                                mx = 3 - x;
                                r = 1;
                            }
                            if (tile.v)
                            {
                                my = (7 - y);
                            }
                            tx = ((tile.id / 16) * 512) + ((tile.id - ((tile.id / 16) * 16)) * 4);
                            bytePointer[xx + yy + 8 + (mx * 2) + (my * 128) + r ^ 1] = (byte)((bytePointer2[tx + x + (y * 64)] & 0x0F) + (tile.palette) * 16);
                            bytePointer[xx + yy + 8 + (mx * 2) + (my * 128) + r] = (byte)(((bytePointer2[tx + x + (y * 64)] >> 4) & 0x0F) + (tile.palette) * 16);

                            mx = x;
                            my = y;
                            tile = mapinfos.alltiles16[i].tile2;
                            r = 0;
                            if (tile.h)
                            {
                                mx = 3 - x;
                                r = 1;
                            }
                            if (tile.v)
                            {
                                my = (7 - y);
                            }
                            tx = ((tile.id / 16) * 512) + ((tile.id - ((tile.id / 16) * 16)) * 4);
                            bytePointer[xx + yy + 1024 + (mx * 2) + (my * 128) + r ^ 1] = (byte)((bytePointer2[tx + x + (y * 64)] & 0x0F) + (tile.palette) * 16);
                            bytePointer[xx + yy + 1024 + (mx * 2) + (my * 128) + r] = (byte)(((bytePointer2[tx + x + (y * 64)] >> 4) & 0x0F) + (tile.palette) * 16);

                            mx = x;
                            my = y;
                            tile = mapinfos.alltiles16[i].tile3;
                            r = 0;
                            if (tile.h)
                            {
                                mx = 3 - x;
                                r = 1;
                            }
                            if (tile.v)
                            {
                                my = (7 - y);
                            }
                            tx = ((tile.id / 16) * 512) + ((tile.id - ((tile.id / 16) * 16)) * 4);
                            bytePointer[xx + yy + 1032 + (mx * 2) + (my * 128) + r ^ 1] = (byte)((bytePointer2[tx + x + (y * 64)] & 0x0F) + (tile.palette) * 16);
                            bytePointer[xx + yy + 1032 + (mx * 2) + (my * 128) + r] = (byte)(((bytePointer2[tx + x + (y * 64)] >> 4) & 0x0F) + (tile.palette) * 16);

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
        }

        public void loadPalette(PaletteHandler allpalettes)
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
             
            for(int i = 0;i<7;i++)
            {
                animated[i] = allpalettes.animatedPalettes[(pal3*7)+i];
            }

            hud = allpalettes.hudPalettes[0];



            setColorsPalette(main,animated,aux1,aux2,hud,bgr);

        }



        public void setColorsPalette(Color[] main, Color[] animated, Color[] aux1, Color[] aux2, Color[] hud, Color bgrcolor)
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


            pal = (allgfx16).Palette;
            for (int i = 0; i < 256; i++)
            {
                pal.Entries[i] = currentPalette[i];
            }
            allgfx16.Palette = pal;

            mapGfx.Palette = pal;

        }

        byte[] staticgfx;
        public void buildtileset(IntPtr allgfx8array, Bitmap[] allBitmaps)
        {

            staticgfx = new byte[] { 58, 59, 60, 61, 0, 0, 89, 91, 0, 0, 0, 0, 0, 0, 0, 0 };
            staticgfx[8] = 115 + 0; staticgfx[9] = 115 + 10; staticgfx[10] = 115 + 6; staticgfx[11] = 115 + 7;


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
                staticgfx[i] = mapinfos.blocksetGroups2[(index * 8) + i];
            }


            for (int i = 0; i < 4; i++)
            {
                staticgfx[12 + i] = (byte)(mapinfos.spritesetGroups[+((mapdata.spriteset) * 4) + i] + 115);
            }


            if (mapinfos.blocksetGroups[(mapdata.blockset * 4)] != 0)
            {
                staticgfx[3] = mapinfos.blocksetGroups[(mapdata.blockset * 4)];
            }
            if (mapinfos.blocksetGroups[(mapdata.blockset * 4) + 1] != 0)
            {
                staticgfx[4] = mapinfos.blocksetGroups[(mapdata.blockset * 4) + 1];
            }
            if (mapinfos.blocksetGroups[(mapdata.blockset * 4) + 2] != 0)
            {
                staticgfx[5] = mapinfos.blocksetGroups[(mapdata.blockset * 4) + 2];
            }
            if (mapinfos.blocksetGroups[(mapdata.blockset * 4) + 3] != 0)
            {
                staticgfx[6] = mapinfos.blocksetGroups[(mapdata.blockset * 4) + 3];
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


            for (int i = 0; i < 16; i++)
            {

                BitmapData currentbmpData = allBitmaps[staticgfx[i]].LockBits(new Rectangle(0, 0, 128, 32), ImageLockMode.ReadOnly, PixelFormat.Format4bppIndexed);
                unsafe
                {
                    byte* bytePointer = (byte*)currentbmpData.Scan0.ToPointer();

                    (allBitmaps[staticgfx[i]] as Bitmap).UnlockBits(currentbmpData);
                    byte* bytePointerNew = (byte*)allgfx8array.ToPointer();

                    for (int j = 0; j < 2048; j++)
                    {
                        switch (i)
                        {
                            case 0:
                                bytePointerNew[(i * 2048) + j] = (byte)(bytePointer[j] + 0x88);
                                break;
                            case 1:
                                bytePointerNew[(i * 2048) + j] = bytePointer[j];
                                break;
                            case 2:
                                bytePointerNew[(i * 2048) + j] = bytePointer[j];
                                break;
                            case 3:
                                bytePointerNew[(i * 2048) + j] = (byte)(bytePointer[j] + 0x88);
                                break;
                            case 4:
                                bytePointerNew[(i * 2048) + j] = (byte)(bytePointer[j] + 0x88);
                                break;
                            case 5:
                                bytePointerNew[(i * 2048) + j] = (byte)(bytePointer[j] + 0x88);
                                break;
                            case 6:
                                bytePointerNew[(i * 2048) + j] = bytePointer[j];
                                break;
                            case 7:
                                bytePointerNew[(i * 2048) + j] = bytePointer[j];

                                break;
                            default:
                                bytePointerNew[(i * 2048) + j] = bytePointer[j];
                                break;

                        }

                    }
                }
            }

        }


    }

}
