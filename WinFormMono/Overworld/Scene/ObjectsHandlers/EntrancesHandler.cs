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
    class EntrancesHandler
    {
        public EntranceOWEditor selectedEntrance = null;
        public EntranceOWEditor lastselectedEntrance = null;
        JsonData jsonData;
        SceneOverworld scene;
        public EntrancesHandler(SceneOverworld scene, JsonData jsonData)
        {
            this.scene = scene;
            this.jsonData = jsonData;
        }

        public void onMouseDown(MouseEventArgs e, int mouse_x, int mouse_y, bool mouse_down)
        {
            for (int i = 0; i < 128; i++)
            {
                EntranceOWEditor en = jsonData.entranceOWsEditor[i];
                if (en.mapId >= scene.offset && en.mapId < 64 + scene.offset)
                {
                    if (mouse_x >= en.x && mouse_x < en.x + 16 && mouse_y >= en.y && mouse_y < en.y + 16)
                    {
                        if (mouse_down == false)
                        {
                            if (e.Button == MouseButtons.Left)
                            {
                                selectedEntrance = en;
                                lastselectedEntrance = en;
                                scene.refresh = true;
                            }
                            else if (e.Button == MouseButtons.Right)
                            {
                                lastselectedEntrance = en;
                                scene.refresh = true;
                            }
                        }
                    }
                }
            }
        }

        public void onMouseUp(MouseEventArgs e, int mouse_x,int mouse_y,bool mouse_down, short mouseOverMap, Map16 map)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (selectedEntrance != null)
                {
                    short mid = map.parentMapId;
                    if (map.parentMapId == 255)
                    {
                        mid = (short)mouseOverMap;
                    }
                    selectedEntrance.updateMapStuff(mid);
                    selectedEntrance = null;

                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (selectedEntrance != null)
                    {
                        lastselectedEntrance = selectedEntrance;
                        selectedEntrance = null;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    for (int i = 0; i < jsonData.entranceOWsEditor.Length; i++)
                    {

                        EntranceOWEditor en = jsonData.entranceOWsEditor[i];
                        if (en.mapId >= scene.offset && en.mapId < 64 + scene.offset)
                        {
                            if (mouse_x >= en.x && mouse_x < en.x + 16 && mouse_y >= en.y && mouse_y < en.y + 16)
                            {
                                ContextMenuStrip menu = new ContextMenuStrip();
                                menu.Items.Add("Entrance Properties");
                                lastselectedEntrance = en;
                                selectedEntrance = null;
                                mouse_down = false;
                                if (lastselectedEntrance == null)
                                {
                                    menu.Items[0].Enabled = false;
                                }

                                menu.Items[0].Click += entranceProperty_Click;
                                menu.Show(Cursor.Position);
                            }
                        }
                    }
                }
            }
        }

        public void onMouseTileChanged(MouseEventArgs e, int mouse_tile_x,int mouse_tile_y, bool mouse_down)
        {
            if (selectedEntrance != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (mouse_down)
                    {
                        selectedEntrance.x = mouse_tile_x * 16;
                        selectedEntrance.y = mouse_tile_y * 16;
                    }
                }
            }
        }

        public void Draw(int transparency, Graphics g)
        {
            Brush bgrBrush = new SolidBrush(Color.FromArgb((int)transparency, 255, 200, 16));
            Pen contourPen = new Pen(Color.FromArgb((int)transparency, 0, 0, 0));
            for (int i = 0; i < jsonData.entranceOWsEditor.Length; i++)
            {
                g.CompositingMode = CompositingMode.SourceOver;
                EntranceOWEditor e = jsonData.entranceOWsEditor[i];

                if (e.mapId < scene.offset + 64 && e.mapId >= scene.offset)
                {
                    if (selectedEntrance != null)
                    {
                        if (e == selectedEntrance)
                        {
                            bgrBrush = new SolidBrush(Color.FromArgb((int)transparency, 0, 55, 240));
                        }
                        else
                        {
                            bgrBrush = new SolidBrush(Color.FromArgb((int)transparency, 255, 200, 16));
                        }
                    }

                    g.FillRectangle(bgrBrush, new Rectangle(e.x, e.y, 16, 16));
                    g.DrawRectangle(contourPen, new Rectangle(e.x, e.y, 16, 16));
                    scene.DrawText(g, e.entranceId.ToString("X2") + "- " + scene.roomsNames[jsonData.entrances[e.entranceId].room], new Point(e.x - 1, e.y + 1));
                }

            }
            g.CompositingMode = CompositingMode.SourceCopy;
        }

        private void entranceProperty_Click(object sender, EventArgs e)
        {
            EntranceEditorForm ef = new EntranceEditorForm();
            if (ef.ShowDialog() == DialogResult.OK)
            {
                //lastselectedEntrance. ef.choice
            }
        }

    }
}
