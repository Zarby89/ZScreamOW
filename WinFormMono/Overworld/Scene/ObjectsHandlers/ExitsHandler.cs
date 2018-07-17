using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormMono
{
    class ExitsHandler
    {
        public ExitOW selectedExit = null;
        public ExitOW lastselectedExit = null;
        JsonData jsonData;
        SceneOverworld scene;
        public ExitsHandler(SceneOverworld scene, JsonData jsonData)
        {
            this.scene = scene;
            this.jsonData = jsonData;
        }

        public void onMouseDown(MouseEventArgs e, int mouse_x, int mouse_y, bool rightClick)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (rightClick)
                {
                    rightClick = false;
                    return;
                }

                for (int i = 0; i < 78; i++)
                {
                    ExitOW en = jsonData.exitsOWs[i];
                    if (en.mapId >= scene.offset && en.mapId < 64 + scene.offset)
                    {
                        if (mouse_x >= en.playerX && mouse_x < en.playerX + 16 && mouse_y >= en.playerY && mouse_y < en.playerY + 16)
                        {
                            if (scene.mouse_down == false)
                            {

                                //Cursor.Hide();
                                selectedExit = en;
                                lastselectedExit = en;
                                scene.refresh = true;

                            }
                        }

                    }
                }
            }
        }



        public void onMouseMove(MouseEventArgs e,int mouse_x,int mouse_y, Map16 map, int mouseOverMap)
        {
            if (scene.mouse_down)
            {
                if (selectedExit != null)
                {
                    selectedExit.playerX = (short)mouse_x;
                    selectedExit.playerY = (short)mouse_y;
                    if (scene.snapToGrid)
                    {
                        selectedExit.playerX = (short)((mouse_x / 8) * 8);
                        selectedExit.playerY = (short)((mouse_y / 8) * 8);
                    }
                    byte mid = map.parentMapId;
                    if (map.parentMapId == 255)
                    {
                        mid = (byte)(mouseOverMap + scene.offset);
                    }
                    selectedExit.updateMapStuff(mid, jsonData, scene.allmaps);
                    scene.refresh = true;
                }
            }
        }

        public void onMouseUp(MouseEventArgs e, int mouse_x,int mouse_y, short mouseOverMap, Map16 map)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (selectedExit != null)
                {
                    lastselectedExit = selectedExit;
                    selectedExit = null;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < 78; i++)
                {

                    ExitOW en = jsonData.exitsOWs[i];
                    if (en.mapId >= scene.offset && en.mapId < 64 + scene.offset)
                    {
                        if (mouse_x >= en.playerX && mouse_x < en.playerX + 16 && mouse_y >= en.playerY && mouse_y < en.playerY + 16)
                        {
                            ContextMenuStrip menu = new ContextMenuStrip();
                            menu.Items.Add("Exit Properties");
                            lastselectedExit = en;
                            selectedExit = null;
                            scene.mouse_down = false;
                            scene.rightClick = true;
                            if (lastselectedExit == null)
                            {
                                menu.Items[0].Enabled = false;
                            }

                            menu.Items[0].Click += exitProperty_Click;
                            menu.Show(Cursor.Position);
                        }
                    }
                }
            }
        }


        public void Draw(int transparency, Graphics g)
        {

            for (int i = 0; i < 78; i++)
            {
                g.CompositingMode = CompositingMode.SourceOver;
                ExitOW e = jsonData.exitsOWs[i];

                if (e.mapId < scene.offset + 64 && e.mapId >= scene.offset)
                {

                    Brush bgrBrush = new SolidBrush(Color.FromArgb((int)transparency, 222, 222, 222));
                    Pen contourPen = new Pen(Color.FromArgb((int)transparency, 0, 0, 0));
                    Brush fontBrush = Brushes.Black;

                    if (selectedExit == null)
                    {
                        if (lastselectedExit == e)
                        {
                            g.CompositingMode = CompositingMode.SourceOver;
                            bgrBrush = new SolidBrush(Color.FromArgb((int)transparency, 160, 160, 160));
                            g.FillRectangle(bgrBrush, new Rectangle(e.playerX, e.playerY, 16, 16));
                            g.DrawRectangle(contourPen, new Rectangle(e.playerX, e.playerY, 16, 16));
                            scene.DrawText(g, i.ToString("X2"), new Point(e.playerX - 1, e.playerY + 1));

                            g.DrawRectangle(Pens.LightPink, new Rectangle(e.xScroll, e.yScroll, 256, 224));
                            g.DrawLine(Pens.Blue, e.cameraX - 8, e.cameraY, e.cameraX + 8, e.cameraY);
                            g.DrawLine(Pens.Blue, e.cameraX, e.cameraY - 8, e.cameraX, e.cameraY + 8);
                            g.CompositingMode = CompositingMode.SourceCopy;
                            continue;
                        }
                        g.FillRectangle(bgrBrush, new Rectangle(e.playerX, e.playerY, 16, 16));
                        g.DrawRectangle(contourPen, new Rectangle(e.playerX, e.playerY, 16, 16));
                        scene.DrawText(g, i.ToString("X2"), new Point(e.playerX - 1, e.playerY + 1));
                    }
                    else
                    {
                        if (selectedExit == e)
                        {
                            g.CompositingMode = CompositingMode.SourceOver;
                            g.DrawImage(jsonData.linkGfx, e.playerX, e.playerY, new Rectangle(16, 0, 16, 16), GraphicsUnit.Pixel);
                            g.DrawImage(jsonData.linkGfx, e.playerX, e.playerY + 8, new Rectangle(48, 16, 16, 16), GraphicsUnit.Pixel);
                            g.CompositingMode = CompositingMode.SourceCopy;

                            g.DrawRectangle(Pens.LightPink, new Rectangle(e.xScroll, e.yScroll, 256, 224));
                            g.DrawLine(Pens.Blue, e.cameraX - 8, e.cameraY, e.cameraX + 8, e.cameraY);
                            g.DrawLine(Pens.Blue, e.cameraX, e.cameraY - 8, e.cameraX, e.cameraY + 8);
                        }
                        else
                        {
                            g.FillRectangle(bgrBrush, new Rectangle(e.playerX, e.playerY, 16, 16));
                            g.DrawRectangle(contourPen, new Rectangle(e.playerX, e.playerY, 16, 16));
                            scene.DrawText(g, i.ToString("X2"), new Point(e.playerX - 1, e.playerY + 1));
                        }
                    }
                }
            }
            g.CompositingMode = CompositingMode.SourceCopy;
        }

        ExitEditorForm exitPropForm = new ExitEditorForm();
        public void exitProperty_Click(object sender, EventArgs e)
        {

            exitPropForm.SetExit(lastselectedExit);
            DialogResult dr = exitPropForm.ShowDialog();
            if (dr == DialogResult.OK)
            {
                int index = Array.IndexOf(jsonData.exitsOWs, lastselectedExit);
                jsonData.exitsOWs[index] = exitPropForm.editingExit;
                lastselectedExit = jsonData.exitsOWs[index];
                scene.sceneMode = SceneMode.exits;
            }
            else if (dr == DialogResult.Yes)
            {
                scene.sceneMode = SceneMode.door;
                if (lastselectedExit.doorType1 != 0) //wooden door
                {
                    scene.selectedTiles = new ushort[2, 1];
                    scene.selectedTiles[0, 0] = 1865;
                    scene.selectedTiles[1, 0] = 1866;
                    scene.setOverlaytiles(scene.allgfx16Ptr);
                }
                else if ((lastselectedExit.doorType2 & 0x8000) != 0) //castle door
                {
                    scene.selectedTiles = new ushort[2, 2];
                    scene.selectedTiles[0, 0] = 3510;
                    scene.selectedTiles[1, 0] = 3511;
                    scene.selectedTiles[0, 1] = 3512;
                    scene.selectedTiles[1, 1] = 3513;
                    scene.setOverlaytiles(scene.allgfx16Ptr);
                }
                else if ((lastselectedExit.doorType2 & 0x7FFF) != 0) //sanc door
                {
                    scene.selectedTiles = new ushort[2, 1];
                    scene.selectedTiles[0, 0] = 3502;
                    scene.selectedTiles[1, 0] = 3503;
                    scene.setOverlaytiles(scene.allgfx16Ptr);
                }
            }
            else
            {
                scene.sceneMode = SceneMode.exits;
            }
            selectedExit = null;
            scene.mouse_down = false;
            scene.rightClick = false;
        }

    }
}
