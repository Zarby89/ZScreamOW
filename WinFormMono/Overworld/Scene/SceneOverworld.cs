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
    class SceneOverworld
    {
        public int mouse_x;
        public int mouse_y;
        int last_mouse_tile_x;
        int last_mouse_tile_y;
        int mouse_tile_x;
        int mouse_tile_y;
        int mouse_tile_x_down;
        int mouse_tile_y_down;
        public bool mouse_down = false;
        public int mouseOverMap;
        public int last_mouseOverMap;
        public bool refresh = false;
        public int screenChanged = -1;
        public IntPtr allgfx16Ptr = Marshal.AllocHGlobal(128 * 7520);
        IntPtr selectedTilesGfxPtr = Marshal.AllocHGlobal(512 * 512);
        //Bitmap allTexts = new Bitmap();
        Bitmap selectedTilesGfx;
        Bitmap[] allbitmaps;
        public Bitmap[] fontBitmap = new Bitmap[3];
        public ushort[,] selectedTiles;
        bool allowCopy = false;
        JsonData jsonData;
        public Rectangle selectionSize = new Rectangle(0, 0, 16, 16);
        public int offset = 0;
        public Map16 selectedMap;
        public bool snapToGrid = false;
        public SceneMode sceneMode = SceneMode.tiles;
        List<OverworldUndo> undoData = new List<OverworldUndo>();
        int undoPos = 0;
        public Map16[] allmaps;
        public float transparency = 255f;
        public byte sceneState = 0;
        public bool rightClick = false;
        public bool debugMode = true;
        EntrancesHandler entrancesHandler;
        HolesHandler holesHandler;
        ExitsHandler exitsHandler;
        SpritesHandler spritesHandler;
        ItemsHandler itemsHandler;
        public SceneOverworld(Bitmap[] allbitmaps, Map16 map, IntPtr allgfx8array, JsonData jsonData)
        {
            this.allbitmaps = allbitmaps;
            selectedTilesGfx = new Bitmap(512, 512, 512, PixelFormat.Format8bppIndexed, selectedTilesGfxPtr);
            selectedTiles = new ushort[1, 1];
            selectedTiles[0, 0] = 0;
            UpdateGfx(allgfx8array, map);
            this.jsonData = jsonData;
            fontBitmap[0] = new Bitmap("Resources//font.png");
            fontBitmap[1] = new Bitmap("Resources//fonta50.png");
            fontBitmap[2] = new Bitmap("Resources//fonta25.png");
            entrancesHandler = new EntrancesHandler(this, jsonData);
            holesHandler = new HolesHandler(this, jsonData);
            exitsHandler = new ExitsHandler(this, jsonData);
            spritesHandler = new SpritesHandler(this, jsonData);
            itemsHandler = new ItemsHandler(this, jsonData);
        }

        public void mouseMove(MouseEventArgs e, Map16 map, IntPtr allgfx8array)
        {
            if (e.X >= 0 && e.Y >= 0 && e.X < 4096 && e.Y < 4096)
            {
                mouse_x = e.X;
                mouse_y = e.Y;
                mouse_tile_x = e.X / 16;
                mouse_tile_y = e.Y / 16;

                if (mouse_tile_x != last_mouse_tile_x || mouse_tile_y != last_mouse_tile_y)
                {
                    //used for tiles, sprites, items (they are lock on 16x16 pixels)
                    //can be used for entrances and exits if snap to grid is on
                    Scene_MouseTileChanged(e, map, allgfx8array); //the mouse is not hovering the same tile
                }

                if (sceneMode == SceneMode.exits)
                {
                    exitsHandler.onMouseMove(e, mouse_x, mouse_y, map, mouseOverMap);
                }


                last_mouse_tile_x = mouse_tile_x;
                last_mouse_tile_y = mouse_tile_y;
                selectedMap = map;
            }
        }
        public void UpdateGfx(IntPtr allgfx8array, Map16 map)
        {
            map.UpdateGfx(allgfx8array, allgfx16Ptr, allbitmaps);
        }

        public void AddUndo(OverworldUndo overundo)
        {
            if (undoPos != undoData.Count)
            {
                undoData.RemoveRange(undoPos, (undoData.Count - undoPos));
                //undoPos = undoData.Count;
            }
            undoData.Add(overundo);
            undoPos += 1;

        }

        public void SetAllMaps(Map16[] allmaps)
        {
            this.allmaps = allmaps;
        }

        public void Undo()
        {
            undoPos -= 1;
            if (undoPos < 0)
            {
                undoPos = 0;
            }
            int index = undoData[undoPos].index;
            allmaps[index].mapdata.tiles = undoData[undoPos].tiles;
            UpdateGfx(jsonData.allgfx8array, allmaps[index]);
            allmaps[index].BuildMap(allgfx16Ptr);


        }
        public void UpdateMap(Map16 map)
        {

            UpdateGfx(jsonData.allgfx8array, map);
            map.LoadPalette(jsonData.allPalettes);
            map.BuildMap(allgfx16Ptr);
        }

        public void Scene_MouseTileChanged(MouseEventArgs e, Map16 map, IntPtr allgfx8array)
        {
            int mx = (mouse_tile_x / 32);
            int my = (mouse_tile_y / 32);
            mouseOverMap = mx + (my * 8);
            selectionSize.X = mouse_tile_x * 16;
            selectionSize.Y = mouse_tile_y * 16;
            selectedTilesGfx.Palette = map.GetPalette();

            if (mouseOverMap != last_mouseOverMap)
            {

                if (mouse_down)
                {
                    AddUndo(new OverworldUndo(mouseOverMap + offset, (ushort[,])allmaps[mouseOverMap].mapdata.tiles.Clone()));
                    map = allmaps[mouseOverMap + offset];
                }

                setOverlaytiles(allgfx16Ptr);
                screenChanged = last_mouseOverMap;
                last_mouseOverMap = mouseOverMap;


            }
            if (mouse_down)
            {
                //Tiles Editing Mode
                if (sceneMode == SceneMode.tiles)
                {

                    if (e.Button == MouseButtons.Left)
                    {
                        map.setTiles(allgfx16Ptr, mouse_tile_x - (mx * 32), mouse_tile_y - (my * 32), selectedTiles);
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        if (screenChanged != -1)
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
                        selectionSize = new Rectangle(((startX + (mx * 32)) * 16), ((startY + (my * 32)) * 16), (sizeX * 16), (sizeY * 16));
                        //overlayGraphics.DrawRectangle(new Pen(Brushes.Yellow), new Rectangle(startX * 16, startY * 16, (sizeX * 16), (sizeY * 16)));
                    }

                }
                else if (sceneMode == SceneMode.entrances)
                {
                    entrancesHandler.onMouseTileChanged(e, mouse_tile_x, mouse_tile_y, mouse_down);
                }
                else if (sceneMode == SceneMode.holes)
                {
                    holesHandler.onMouseTileChanged(e, mouse_tile_x, mouse_tile_y, mouse_down);
                }
                else if (sceneMode == SceneMode.sprites)
                {
                    spritesHandler.onMouseTileChanged(e, mouse_tile_x, mouse_tile_y, mouse_down);
                }
                else if (sceneMode == SceneMode.items)
                {
                    itemsHandler.onMouseTileChanged(e, mouse_tile_x, mouse_tile_y, mouse_down);
                }
            }
            map.UpdateMap(allgfx16Ptr);
            refresh = true;

        }

        public void updateDisplayedMaps(int offset)
        {
            this.offset = offset;
        }


        public void mouseDown(MouseEventArgs e, Map16 map, IntPtr allgfx8array)
        {

            if (sceneMode == SceneMode.door)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (map.parentMapId != 255)
                    {
                        int mapy = (exitsHandler.lastselectedExit.mapId / 8);
                        int mapx = exitsHandler.lastselectedExit.mapId - (mapy * 8);
                        mouse_tile_x_down = mouse_tile_x - (mapx * 32);
                        mouse_tile_y_down = mouse_tile_y - (mapy * 32);
                    }
                    else
                    {
                        int mx = (mouse_tile_x / 32);
                        int my = (mouse_tile_y / 32);
                        mouse_tile_x_down = mouse_tile_x - (mx * 32);
                        mouse_tile_y_down = mouse_tile_y - (my * 32);
                    }

                    exitsHandler.lastselectedExit.doorXEditor = (byte)mouse_tile_x_down;
                    exitsHandler.lastselectedExit.doorYEditor = (byte)mouse_tile_y_down;
                    exitsHandler.exitProperty_Click(null, null);
                }
            }
            else if (sceneMode == SceneMode.entrances)
            {
                entrancesHandler.onMouseDown(e, mouse_x, mouse_y, mouse_down);
            }
            else if (sceneMode == SceneMode.holes)
            {
                holesHandler.onMouseDown(e, mouse_x, mouse_y, mouse_down);
            }
            else if (sceneMode == SceneMode.exits)
            {
                exitsHandler.onMouseDown(e, mouse_x, mouse_y, rightClick);
            }
            else if (sceneMode == SceneMode.sprites)
            {
                spritesHandler.onMouseDown(e, mouse_x, mouse_y, mouse_down);
            }
            else if (sceneMode == SceneMode.items)
            {
                itemsHandler.onMouseDown(e, mouse_x, mouse_y, mouse_down);
            }
            else if (sceneMode == SceneMode.tiles)
            {
                int yT = (mouseOverMap / 8);
                int xT = mouseOverMap - (yT * 8);
                int mx = (mouse_tile_x / 32);
                int my = (mouse_tile_y / 32);
                mouse_tile_x_down = mouse_tile_x - (mx * 32);
                mouse_tile_y_down = mouse_tile_y - (my * 32);
                if (e.Button == MouseButtons.Right)
                {

                    allowCopy = true;
                    selectionSize = new Rectangle(mouse_tile_x_down * 16, mouse_tile_y_down * 16, 16, 16);
                    selectedTiles = map.getTiles(mouse_tile_x_down, mouse_tile_y_down, 1, 1);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    AddUndo(new OverworldUndo(mouseOverMap + offset, (ushort[,])map.mapdata.tiles.Clone()));
                    map.setTiles(allgfx16Ptr, mouse_tile_x - (mx * 32), mouse_tile_y - (my * 32), selectedTiles);
                }

                map.UpdateMap(allgfx16Ptr);
                //setOverlaytiles(allgfx16Ptr);
                //map.setTile(allgfx16Ptr,mouse_tile_x, mouse_tile_y, 500);
            }
            mouse_down = true;
        }



        public void mouseUp(MouseEventArgs e, Map16 map)
        {
            if (sceneMode == SceneMode.tiles)
            {


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
                        selectionSize = new Rectangle(mouse_tile_x_down * 16, mouse_tile_y_down * 16, (sizeX * 16), (sizeY * 16));
                        selectedTiles = map.getTiles(mouse_tile_x_down, mouse_tile_y_down, sizeX, sizeY);
                        setOverlaytiles(allgfx16Ptr);
                    }
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    selectionSize = new Rectangle(mouse_tile_x_down * 16, mouse_tile_y_down * 16, (1 * 16), (1 * 16));
                    selectedTiles = map.getTiles(mouse_tile_x_down, mouse_tile_y_down, 1, 1);
                    Tile16EditorForm tile16Editor = new Tile16EditorForm();
                    tile16Editor.setGfxData(allbitmaps, jsonData, map, selectedTiles[0, 0]);
                    if (tile16Editor.ShowDialog() == DialogResult.OK)
                    {
                        jsonData.alltiles16[selectedTiles[0, 0]] = tile16Editor.editingTile;
                        map.UpdateMap(allgfx16Ptr);
                        refresh = true;
                    }
                }
            }
            else if (sceneMode == SceneMode.entrances)
            {
                entrancesHandler.onMouseUp(e, mouse_x, mouse_y, mouse_down, (short)mouseOverMap, map);
            }
            else if (sceneMode == SceneMode.holes)
            {
                holesHandler.onMouseUp(e, mouse_x, mouse_y, mouse_down, (short)mouseOverMap, map);
            }
            else if (sceneMode == SceneMode.exits)
            {
                exitsHandler.onMouseUp(e, mouse_x, mouse_y, (short)mouseOverMap, map);
            }
            else if (sceneMode == SceneMode.sprites)
            {
                spritesHandler.onMouseUp(e, mouse_x, mouse_y, mouse_down, (short)mouseOverMap, map);
            }
            else if (sceneMode == SceneMode.items)
            {
                itemsHandler.onMouseUp(e, mouse_x, mouse_y, mouse_down, (short)mouseOverMap, map);
            }
            refresh = true;
            //refresh = true;
            mouse_down = false;
        }

        public unsafe void setOverlaytiles(IntPtr allgfx16Ptr)
        {

            byte* gfxData = (byte*)selectedTilesGfxPtr.ToPointer();
            byte* gfx16Data = (byte*)allgfx16Ptr.ToPointer();
            int sizeX = selectedTiles.GetLength(0);
            int sizeY = selectedTiles.GetLength(1);
            for (int xx = 0; xx < sizeX; xx++)
            {
                for (int yy = 0; yy < sizeY; yy++)
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

            selectionSize.Width = sizeX * 16;
            selectionSize.Height = sizeY * 16;

        }

        public int GetTilePos(int x, int y)
        {
            if (x > 31) { return -1; }
            if (y > 31) { return -1; }
            return ((selectedTiles[x, y] / 8) * 2048) + ((selectedTiles[x, y] - ((selectedTiles[x, y] / 8) * 8)) * 16);
        }
        ColorMatrix cm = new ColorMatrix();
        ImageAttributes ia = new ImageAttributes();
        public void Draw(Graphics g, Map16 focusedMap)
        {

            int yT = (mouseOverMap / 8);
            int xT = mouseOverMap - (yT * 8);
            if (sceneMode == SceneMode.tiles)
            {
                g.CompositingMode = CompositingMode.SourceOver;
                if (!mouse_down)
                {

                    cm.Matrix33 = 0.50f;
                    cm.Matrix22 = 2f;

                    ia.SetColorMatrix(cm);
                    g.DrawImage(selectedTilesGfx, new Rectangle((mouse_tile_x * 16), (mouse_tile_y * 16), (selectedTiles.GetLength(0) * 16), (selectedTiles.GetLength(1) * 16)), 0, 0, (selectedTiles.GetLength(0) * 16), (selectedTiles.GetLength(1) * 16), GraphicsUnit.Pixel, ia);
                }


                g.CompositingMode = CompositingMode.SourceCopy;
                g.DrawRectangle(Pens.LightGreen, selectionSize);
            }
            else if (sceneMode == SceneMode.door)
            {
                g.DrawImage(selectedTilesGfx, new Rectangle((mouse_tile_x * 16), (mouse_tile_y * 16), (selectedTiles.GetLength(0) * 16), (selectedTiles.GetLength(1) * 16)), 0, 0, (selectedTiles.GetLength(0) * 16), (selectedTiles.GetLength(1) * 16), GraphicsUnit.Pixel);
                g.DrawRectangle(Pens.LightGreen, selectionSize);
            }

            g.DrawRectangle(Pens.DarkOrange, new Rectangle(xT * 512, yT * 512, 511, 511));
            if (selectedMap != null)
            {
                if (debugMode)
                {
                    DrawText(g, selectionSize.ToString(), new Point(xT * 512, yT * 512));
                    DrawText(g, selectedMap.largeMap.ToString() + "  ParentID : " + selectedMap.parentMapId.ToString(), new Point(xT * 512, (yT * 512) + 32));
                }
            }

            if (focusedMap != null)
            {

                int yT2 = (focusedMap.index / 8);
                int xT2 = focusedMap.index - (yT * 8);
                g.DrawRectangle(Pens.DarkRed, new Rectangle(xT2 * 512, yT2 * 512, 511, 511));



                /*for (int y = 0; y < 32; y++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        DrawText(g, selectedMap.mapdata.tiles[x, y].ToString("X4"), new Point((x * 16) + (xT*512), (y * 16) + (yT*512)));
                    }
                }*/

            }
        }

        public void DrawObjects(Graphics g)
        {
            if (sceneMode == SceneMode.exits)
            {
                entrancesHandler.Draw((int)transparency, g);
                holesHandler.Draw((int)transparency, g);
                exitsHandler.Draw((int)transparency, g);
                spritesHandler.Draw((int)transparency, g);
                itemsHandler.Draw((int)transparency, g);
            }
            else
            {
                exitsHandler.Draw((int)transparency, g);
                entrancesHandler.Draw((int)transparency, g);
                holesHandler.Draw((int)transparency, g);
                spritesHandler.Draw((int)transparency, g);
                itemsHandler.Draw((int)transparency, g);

            }


        }

        public void DrawText(Graphics g, string text, Point position)
        {
            //ColorMatrix cm = new ColorMatrix();
            //ImageAttributes ia = new ImageAttributes();
            //cm.Matrix33 = (transparency / 255);
            //ia.SetColorMatrix(cm);
            g.CompositingMode = CompositingMode.SourceOver;
            for (int i = 0; i < text.Length; i++)
            {
                byte l = (byte)text[i];
                if (l <= 32)
                {
                    continue; //Space
                }
                else
                {
                    l -= 33;
                }
                int y = ((l / 32));
                int x = l - (y * 32);
                if (transparency == 255f)
                {
                    g.DrawImage(fontBitmap[0], new Rectangle((position.X + (i * 7)) + 3, position.Y + 1, 8, 10), x * 8, y * 10, 8, 10, GraphicsUnit.Pixel);
                }
                else if (transparency == 127f)
                {
                    g.DrawImage(fontBitmap[1], new Rectangle((position.X + (i * 7)) + 3, position.Y + 1, 8, 10), x * 8, y * 10, 8, 10, GraphicsUnit.Pixel);
                }
                else if (transparency == 75f)
                {
                    g.DrawImage(fontBitmap[2], new Rectangle((position.X + (i * 7)) + 3, position.Y + 1, 8, 10), x * 8, y * 10, 8, 10, GraphicsUnit.Pixel);
                }
            }
            g.CompositingMode = CompositingMode.SourceCopy;

        }



        public void DrawAllStrings()
        {

        }




        public string[] roomsNames = new string[]{
      "Ganon","Hyrule Castle (North Corridor)","Behind Sanctuary (Switch)",
      "Houlihan","Turtle Rock (Crysta-Roller)",
      "Empty","Swamp Palace (Arrghus[Boss])",
      "Tower of Hera (Moldorm[Boss])","Cave (Healing Fairy)","Palace of Darkness",
      "Palace of Darkness (Stalfos Trap)","Palace of Darkness (Turtle)","Ganon's Tower (Entrance)",
      "Ganon's Tower (Agahnim2[Boss])","Ice Palace (Entrance )","Empty Clone ",
      "Ganon Evacuation Route","Hyrule Castle (Bombable Stock )",
      "Sanctuary","Turtle Rock (Hokku-Bokku Key 2)","Turtle Rock (Big Key )",
      "Turtle Rock","Swamp Palace (Swimming Treadmill)","Tower of Hera (Moldorm Fall )",
      "Cave","Palace of Darkness (Dark Maze)","Palace of Darkness (Big Chest )",
      "Palace of Darkness (Mimics / Moving Wall )","Ganon's Tower (Ice Armos)","Ganon's Tower (Final Hallway)",
      "Ice Palace (Bomb Floor / Bari )","Ice Palace (Pengator / Big Key )","Agahnim's Tower (Agahnim[Boss])",
      "Hyrule Castle (Key-rat )","Hyrule Castle (Sewer Text Trigger )","Turtle Rock (West Exit to Balcony)",
      "Turtle Rock (Double Hokku-Bokku / Big chest )","Empty Clone ","Swamp Palace (Statue )",
      "Tower of Hera (Big Chest)","Swamp Palace (Entrance )","Skull Woods (Mothula[Boss])",
      "Palace of Darkness (Big Hub )","Palace of Darkness (Map Chest / Fairy )","Cave",
      "Empty Clone ","Ice Palace (Compass )","Cave (Kakariko Well HP)",
      "Agahnim's Tower (Maiden Sacrifice Chamber)","Tower of Hera (Hardhat Beetles )","Hyrule Castle (Sewer Key Chest )",
      "Desert Palace (Lanmolas[Boss])","Swamp Palace (Push Block Puzzle / Pre-Big Key )","Swamp Palace (Big Key / BS )",
      "Swamp Palace (Big Chest )","Swamp Palace (Map Chest / Water Fill )","Swamp Palace (Key Pot )",
      "Skull Woods (Gibdo Key / Mothula Hole )","Palace of Darkness (Bombable Floor )",
      "Palace of Darkness (Spike Block / Conveyor )","Cave","Ganon's Tower (Torch 2)",
      "Ice Palace (Stalfos Knights / Conveyor Hellway)","Ice Palace (Map Chest )","Agahnim's Tower (Final Bridge )",
      "Hyrule Castle (First Dark )","Hyrule Castle (6 Ropes )","Desert Palace (Torch Puzzle / Moving Wall )",
      "Thieves Town (Big Chest )","Thieves Town (Jail Cells )","Swamp Palace (Compass Chest )",
      "Empty Clone ","Empty Clone ","Skull Woods (Gibdo Torch Puzzle )","Palace of Darkness (Entrance )",
      "Palace of Darkness (Warps / South Mimics )","Ganon's Tower (Mini-Helmasaur Conveyor )","Ganon's Tower (Moldorm )",
      "Ice Palace (Bomb-Jump )","Ice Palace Clone (Fairy )","Hyrule Castle (West Corridor)",
      "Hyrule Castle (Throne )","Hyrule Castle (East Corridor)","Desert Palace (Popos 2 / Beamos Hellway )",
      "Swamp Palace (Upstairs Pits )","Castle Secret Entrance / Uncle Death ","Skull Woods (Key Pot / Trap )",
      "Skull Woods (Big Key )","Skull Woods (Big Chest )","Skull Woods (Final Section Entrance )",
      "Palace of Darkness (Helmasaur King[Boss])","Ganon's Tower (Spike Pit )","Ganon's Tower (Ganon-Ball Z)",
      "Ganon's Tower (Gauntlet 1/2/3)","Ice Palace (Lonely Firebar)","Ice Palace (Hidden Chest / Spike Floor )",
      "Hyrule Castle (West Entrance )","Hyrule Castle (Main Entrance )","Hyrule Castle (East Entrance )",
      "Desert Palace (Final Section Entrance )","Thieves Town (West Attic )","Thieves Town (East Attic )",
      "Swamp Palace (Hidden Chest / Hidden Door )","Skull Woods (Compass Chest )","Skull Woods (Key Chest / Trap )",
      "Empty Clone ","Palace of Darkness (Rupee )","Ganon's Tower (Mimics s)","Ganon's Tower (Lanmolas )",
      "Ganon's Tower (Gauntlet 4/5)","Ice Palace (Pengators )","Empty Clone ","Hyrule Castle (Small Corridor to Jail Cells)",
      "Hyrule Castle (Boomerang Chest )","Hyrule Castle (Map Chest )","Desert Palace (Big Chest )",
      "Desert Palace (Map Chest )","Desert Palace (Big Key Chest )","Swamp Palace (Water Drain )",
      "Tower of Hera (Entrance )","Empty Clone ","Empty Clone ","Empty Clone ",
      "Ganon's Tower","Ganon's Tower (East Side Collapsing Bridge / Exploding Wall )","Ganon's Tower (Winder / Warp Maze )",
      "Ice Palace (Hidden Chest / Bombable Floor )","Ice Palace ( Big Spike Traps )","Hyrule Castle (Jail Cell )",
      "Hyrule Castle","Hyrule Castle (Basement Chasm )","Desert Palace (West Entrance )","Desert Palace (Main Entrance )",
      "Desert Palace (East Entrance )","Empty Clone ","Tower of Hera (Tile )","Empty Clone ",
      "Eastern Palace (Fairy )","Empty Clone ","Ganon's Tower (Block Puzzle / Spike Skip / Map Chest )",
      "Ganon's Tower (East and West Downstairs / Big Chest )","Ganon's Tower (Tile / Torch Puzzle )","Ice Palace",
      "Empty Clone ","Misery Mire (Vitreous[Boss])","Misery Mire (Final Switch )","Misery Mire (Dark Bomb Wall / Switches )",
      "Misery Mire (Dark Cane Floor Switch Puzzle )","Empty Clone ","Ganon's Tower (Final Collapsing Bridge )",
      "Ganon's Tower (Torches 1 )","Misery Mire (Torch Puzzle / Moving Wall )","Misery Mire (Entrance )",
      "Eastern Palace (Eyegore Key )","Empty Clone ","Ganon's Tower (Many Spikes / Warp Maze )",
      "Ganon's Tower (Invisible Floor Maze )","Ganon's Tower (Compass Chest / Invisible Floor )",
      "Ice Palace (Big Chest )","Ice Palace","Misery Mire (Pre-Vitreous )","Misery Mire (Fish )",
      "Misery Mire (Bridge Key Chest )","Misery Mire","Turtle Rock (Trinexx[Boss])","Ganon's Tower (Wizzrobes s)",
      "Ganon's Tower (Moldorm Fall )","Tower of Hera (Fairy )","Eastern Palace (Stalfos Spawn )",
      "Eastern Palace (Big Chest )","Eastern Palace (Map Chest )","Thieves Town (Moving Spikes / Key Pot )",
      "Thieves Town (Blind The Thief[Boss])","Empty Clone ","Ice Palace","Ice Palace (Ice Bridge )",
      "Agahnim's Tower (Circle of Pots)","Misery Mire (Hourglass )","Misery Mire (Slug )",
      "Misery Mire (Spike Key Chest )","Turtle Rock (Pre-Trinexx )","Turtle Rock (Dark Maze)",
      "Turtle Rock (Chain Chomps )","Turtle Rock (Map Chest / Key Chest / Roller )","Eastern Palace (Big Key )",
      "Eastern Palace (Lobby Cannonballs )","Eastern Palace (Dark Antifairy / Key Pot )","Thieves Town (Hellway)","Thieves Town (Conveyor Toilet)",
      "Empty Clone ","Ice Palace (Block Puzzle )","Ice Palace Clone (Switch )",
      "Agahnim's Tower (Dark Bridge )","Misery Mire (Compass Chest / Tile )","Misery Mire (Big Hub )",
      "Misery Mire (Big Chest )","Turtle Rock (Final Crystal Switch Puzzle )","Turtle Rock (Laser Bridge)",
      "Turtle Rock","Turtle Rock (Torch Puzzle)","Eastern Palace (Armos Knights[Boss])","Eastern Palace (Entrance )",
      "??","Thieves Town (North West Entrance )","Thieves Town (North East Entrance )",
      "Empty Clone ","Ice Palace (Hole to Kholdstare )","Empty Clone ","Agahnim's Tower (Dark Maze)",
      "Misery Mire (Conveyor Slug / Big Key )","Misery Mire (Mire02 / Wizzrobes )","Empty Clone ","Empty Clone ",
      "Turtle Rock (Laser Key )","Turtle Rock (Entrance )","Empty Clone ","Eastern Palace (Zeldagamer / Pre-Armos Knights )",
      "Eastern Palace (Canonball ","Eastern Palace","Thieves Town (Main (South West) Entrance )",
      "Thieves Town (South East Entrance )","Empty Clone ","Ice Palace (Kholdstare[Boss])",
      "Cave","Agahnim's Tower (Entrance )","Cave (Lost Woods HP)","Cave (Lumberjack's Tree HP)",
      "Cave (1/2 Magic)","Cave (Lost Old Man Final Cave)","Cave (Lost Old Man Final Cave)",
      "Cave","Cave","Cave","Empty Clone ","Cave (Spectacle Rock HP)",
      "Cave","Empty Clone ","Cave","Cave (Spiral Cave)","Cave (Crystal Switch / 5 Chests )",
      "Cave (Lost Old Man Starting Cave)","Cave (Lost Old Man Starting Cave)","House","House (Old Woman (Sahasrahla's Wife?))",
      "House (Angry Brothers)","House (Angry Brothers)","Empty Clone ","Empty Clone ",
      "Cave","Cave","Cave","Cave","Empty Clone ","Cave",
      "Cave","Cave",

      "Chest Minigame","Houses","Sick Boy house","Tavern","Link's House","Sarashrala Hut","Chest Minigame","Library",
      "Chicken House","Witch Shop","A Aginah's Cave","Dam","Mimic Cave","Mire Shed","Cave","Shop","Shop",
      "Archery Minigame","DW Church/Shop","Grave Cave","Fairy Fountain","Fairy Upgrade","Pyramid Fairy","Spike Cave",
      "Chest Minigame","Blind Hut","Bonzai Cave","Circle of bush Cave","Big Bomb Shop, C-House","Blind Hut 2","Hype Cave",
      "Shop","Ice Cave","Smith","Fortune Teller","MiniMoldorm Cave","Under Rock Caves","Smith","Cave","Mazeblock Cave",
      "Smith Peg Cave"
       };
    }





    public enum SceneMode
    {
        tiles, entrances, exits, sprites, holes, warps,items,door
    };
}
