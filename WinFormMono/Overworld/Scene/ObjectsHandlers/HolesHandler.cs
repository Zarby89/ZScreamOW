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
    class HolesHandler
    {
        public EntranceOWEditor selectedHole = null;
        JsonData jsonData;
        SceneOverworld scene;
        public HolesHandler(SceneOverworld scene, JsonData jsonData)
        {
            this.scene = scene;
            this.jsonData = jsonData;
        }

        public void onMouseDown(MouseEventArgs e, int mouse_x, int mouse_y, bool mouse_down)
        {
            for (int i = 0; i < 0x13; i++)
            {
                EntranceOWEditor en = jsonData.holesOWsEditor[i];
                if (en.mapId >= scene.offset && en.mapId <= 64 + scene.offset)
                {
                    if (mouse_x >= en.x && mouse_x < en.x + 16 && mouse_y >= en.y && mouse_y < en.y + 16)
                    {
                        if (mouse_down == false)
                        {
                            if (e.Button == MouseButtons.Left)
                            {
                                selectedHole = en;
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
                if (selectedHole != null)
                {
                    //Console.WriteLine("OldMPos" + selectedEntrance.mapPos);
                    short mid = map.parentMapId;
                    if (map.parentMapId == 255)
                    {
                        mid = (short)mouseOverMap;
                    }
                    selectedHole.updateMapStuff(mid);
                    selectedHole = null;

                }
            }
        }

        public void onMouseTileChanged(MouseEventArgs e, int mouse_tile_x,int mouse_tile_y, bool mouse_down)
        {
            if (selectedHole != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (mouse_down)
                    {
                        selectedHole.x = mouse_tile_x * 16;
                        selectedHole.y = mouse_tile_y * 16;
                    }
                }
            }
        }

        public void Draw(int transparency, Graphics g)
        {
            for (int i = 0; i < 0x13; i++)
            {
                g.CompositingMode = CompositingMode.SourceOver;
                EntranceOWEditor e = jsonData.holesOWsEditor[i];
                Brush bgrBrush = new SolidBrush(Color.FromArgb((int)transparency, 20, 20, 20));
                Pen contourPen = new Pen(Color.FromArgb((int)transparency, 0, 0, 0));
                if (e.mapId < scene.offset + 64 && e.mapId >= scene.offset)
                {


                    if (selectedHole != null)
                    {
                        if (e == selectedHole)
                        {
                            bgrBrush = new SolidBrush(Color.FromArgb((int)transparency, 0, 55, 240));
                        }
                        else
                        {
                            bgrBrush = new SolidBrush(Color.FromArgb((int)transparency, 255, 200, 16));
                        }
                    }

                    g.FillRectangle(bgrBrush, new Rectangle((e.x), (e.y), 16, 16));
                    g.DrawRectangle(contourPen, new Rectangle((e.x), (e.y), 16, 16));
                    scene.DrawText(g, e.entranceId.ToString("X2") + "- " + scene.roomsNames[jsonData.entrances[e.entranceId].room], new Point((e.x), (e.y)));
                    g.CompositingMode = CompositingMode.SourceCopy;
                }

            }

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
