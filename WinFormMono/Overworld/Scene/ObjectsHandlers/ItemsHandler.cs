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
    class ItemsHandler
    {
        public RoomPotSaveEditor selectedItem;
        public RoomPotSaveEditor lastselectedItem;
        JsonData jsonData;
        SceneOverworld scene;
        public ItemsHandler(SceneOverworld scene, JsonData jsonData)
        {
            this.scene = scene;
            this.jsonData = jsonData;
        }

        public void onMouseDown(MouseEventArgs e, int mouse_x, int mouse_y, bool mouse_down)
        {

            foreach (RoomPotSaveEditor item in jsonData.itemsOWEditor)
            {
                if (item.roomMapId >= scene.offset && item.roomMapId < scene.offset + 64)
                {
                    if (mouse_x >= item.x && mouse_x <= item.x + 16 && mouse_y >= item.y && mouse_y <= item.y + 16)
                    {
                        selectedItem = item;
                        lastselectedItem = item;
                        scene.refresh = true;
                    }
                }
            }

    



        }

        public void onMouseUp(MouseEventArgs e, int mouse_x,int mouse_y,bool mouse_down, short mouseOverMap, Map16 map)
        {

            if (e.Button == MouseButtons.Left)
            {
                if (selectedItem != null)
                {
                    byte mid = map.parentMapId;
                    if (map.parentMapId == 255)
                    {
                        mid = (byte)(mouseOverMap + scene.offset);
                    }
                    selectedItem.updateMapStuff(mid);
                    lastselectedItem = selectedItem;
                    selectedItem = null;
                }
                else
                {
                    lastselectedItem = null;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                    ContextMenuStrip menu = new ContextMenuStrip();
                    menu.Items.Add("Add Item");
                    menu.Items.Add("Item Properties");
                    menu.Items.Add("Delete Item");
                    if (lastselectedItem == null)
                    {
                        menu.Items[1].Enabled = false;
                        menu.Items[2].Enabled = false;
                    }
 
                    menu.Items[0].Click += addItem_Click;
                    menu.Items[1].Click += ItemProperties_Click;
                    menu.Items[2].Click += deleteItem_Click;
                    menu.Show(Cursor.Position);
            }
        }

        public void onMouseTileChanged(MouseEventArgs e, int mouse_tile_x,int mouse_tile_y, bool mouse_down)
        {
            if (selectedItem != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (mouse_down)
                    {
                        selectedItem.x = mouse_tile_x * 16;
                        selectedItem.y = mouse_tile_y * 16;
                    }
                }
            }
        }

        public void Draw(int transparency, Graphics g)
        {
            Brush bgrBrush = new SolidBrush(Color.FromArgb((int)transparency, 200, 0, 0));
            Pen contourPen = new Pen(Color.FromArgb((int)transparency, 0, 0, 0));

            foreach (RoomPotSaveEditor item in jsonData.itemsOWEditor)
            {
                if (item.roomMapId >= scene.offset && item.roomMapId < scene.offset + 64)
                {
                    g.CompositingMode = CompositingMode.SourceOver;
                    if (selectedItem == item)
                    {
                        bgrBrush = new SolidBrush(Color.FromArgb((int)transparency, 00, 200, 200));
                        contourPen = new Pen(Color.FromArgb((int)transparency, 0, 0, 0));
                    }
                    else
                    {
                        bgrBrush = new SolidBrush(Color.FromArgb((int)transparency, 200, 0, 0));
                        contourPen = new Pen(Color.FromArgb((int)transparency, 0, 0, 0));
                    }
                    g.FillRectangle(bgrBrush, new Rectangle((item.x), (item.y), 16, 16));
                    g.DrawRectangle(contourPen, new Rectangle((item.x), (item.y), 16, 16));
                    byte nid = item.id;
                    if ((item.id & 0x80) == 0x80)
                    {
                        nid = (byte)(((item.id - 0x80) / 2) + 0x17);
                    }
                    scene.DrawText(g, item.id.ToString("X2") + " - " + ItemsNames.name[nid], new Point((item.x) - 1, (item.y) + 1));

                }
            }
            g.CompositingMode = CompositingMode.SourceCopy;
        }


        private void addItem_Click(object sender, EventArgs e)
        {
            AddItemForm additemF = new AddItemForm();
            if (additemF.ShowDialog() == DialogResult.OK)
            {
                RoomPotSaveEditor s = new RoomPotSaveEditor(additemF.pickedItem, (ushort)scene.selectedMap.index, (scene.mouse_x / 16) * 16, (scene.mouse_y / 16) * 16, false);

                short mid = (short)scene.selectedMap.index;
                if (scene.selectedMap.largeMap)
                {
                    if (scene.selectedMap.parentMapId != 255)
                    {
                        mid = scene.selectedMap.parentMapId;
                    }
                }

                s.updateMapStuff(mid);
                jsonData.itemsOWEditor.Add(s);

            }
        }
        private void deleteItem_Click(object sender, EventArgs e)
        {
            jsonData.itemsOWEditor.Remove(lastselectedItem);
            lastselectedItem = null;
            selectedItem = null;
        }
        private void ItemProperties_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

    }
}
