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
    class TransportsHandler
    {
        public Room_SpriteOWEditor selectedSprite;
        public Room_SpriteOWEditor lastselectedSprite;
        JsonData jsonData;
        SceneOverworld scene;
        public TransportsHandler(SceneOverworld scene, JsonData jsonData)
        {
            this.scene = scene;
            this.jsonData = jsonData;
        }

        public void onMouseDown(MouseEventArgs e, int mouse_x, int mouse_y, bool mouse_down)
        {

            byte spriteState = scene.sceneState;
            if (spriteState == 2) { spriteState = 1; }
            else if (spriteState == 3) { spriteState = 2; }
            foreach (Room_SpriteOWEditor spr in jsonData.spritesOWEditor[spriteState])
            {
                if (spr.roomMapId >= scene.offset && spr.roomMapId < scene.offset + 64)
                {
                    if (mouse_x >= spr.x && mouse_x <= spr.x + 16 && mouse_y >= spr.y && mouse_y <= spr.y + 16)
                    {
                        selectedSprite = spr;
                        scene.refresh = true;
                    }
                }
            }


        }

        public void onMouseUp(MouseEventArgs e, int mouse_x,int mouse_y,bool mouse_down, short mouseOverMap, Map16 map)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (selectedSprite != null)
                {
                    byte mid = map.parentMapId;
                    if (map.parentMapId == 255)
                    {
                        mid = (byte)(mouseOverMap + scene.offset);
                    }
                    selectedSprite.updateMapStuff(mid);
                    lastselectedSprite = selectedSprite;
                    selectedSprite = null;
                }
                else
                {
                    lastselectedSprite = null;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip menu = new ContextMenuStrip();
                menu.Items.Add("Add Sprite");
                menu.Items.Add("Sprite Properties");
                menu.Items.Add("Delete Sprite");
                if (lastselectedSprite == null)
                {
                    menu.Items[1].Enabled = false;
                    menu.Items[2].Enabled = false;
                }
                menu.Items[0].Click += addSprite_Click;
                menu.Items[1].Click += spriteProperties_Click;
                menu.Items[2].Click += deleteSprite_Click;
                menu.Show(Cursor.Position);
                
            }
        }

        public void onMouseTileChanged(MouseEventArgs e, int mouse_tile_x,int mouse_tile_y, bool mouse_down)
        {
            if (selectedSprite != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (mouse_down)
                    {
                        selectedSprite.x = mouse_tile_x * 16;
                        selectedSprite.y = mouse_tile_y * 16;
                    }
                }
            }
        }

        public void Draw(int transparency, Graphics g)
        {


            Brush bgrBrush = new SolidBrush(Color.FromArgb((int)transparency, 255, 0, 255));
            Pen contourPen = new Pen(Color.FromArgb((int)transparency, 0, 0, 0));

            byte spriteState = scene.sceneState;
            if (spriteState == 2)
            {
                spriteState = 1;
            }
            else if (spriteState == 3)
            {
                spriteState = 2;
            }
            foreach (Room_SpriteOWEditor spr in jsonData.spritesOWEditor[spriteState])
            {
                if (spr.roomMapId >= scene.offset && spr.roomMapId < scene.offset + 64)
                {
                    g.CompositingMode = CompositingMode.SourceOver;
                    if (selectedSprite == spr)
                    {
                        bgrBrush = new SolidBrush(Color.FromArgb((int)transparency, 00, 255, 0));
                        contourPen = new Pen(Color.FromArgb((int)transparency, 0, 0, 0));
                    }
                    else if (lastselectedSprite == spr)
                    {
                        bgrBrush = new SolidBrush(Color.FromArgb((int)transparency, 0, 180, 0));
                        contourPen = new Pen(Color.FromArgb((int)transparency, 0, 0, 0));
                    }
                    else
                    {
                        bgrBrush = new SolidBrush(Color.FromArgb((int)transparency, 255, 0, 255));
                        contourPen = new Pen(Color.FromArgb((int)transparency, 0, 0, 0));
                    }
                    g.FillRectangle(bgrBrush, new Rectangle((spr.x), (spr.y), 16, 16));
                    g.DrawRectangle(contourPen, new Rectangle((spr.x), (spr.y), 16, 16));
                    scene.DrawText(g, spr.name, new Point((spr.x) - 1, (spr.y) + 1));

                }
            }
            g.CompositingMode = CompositingMode.SourceCopy;
        }

        private void addSprite_Click(object sender, EventArgs e)
        {
            selectedSprite = null;
            lastselectedSprite = null;
            byte spriteState = scene.sceneState;
            if (spriteState == 2) { spriteState = 1; }
            else if (spriteState == 3) { spriteState = 2; }
            AddSpriteForm addspriteF = new AddSpriteForm();
            if (addspriteF.ShowDialog() == DialogResult.OK)
            {
                jsonData.spritesOWEditor[spriteState].Add(new Room_SpriteOWEditor(addspriteF.pickedSprite, (scene.mouse_x / 16) * 16, (scene.mouse_y / 16) * 16, (ushort)scene.selectedMap.index, 0, 0));
            }
        }
        private void deleteSprite_Click(object sender, EventArgs e)
        {
            byte spriteState = scene.sceneState;
            if (spriteState == 2) { spriteState = 1; }
            else if (spriteState == 3) { spriteState = 2; }
            jsonData.spritesOWEditor[spriteState].Remove(lastselectedSprite);
            selectedSprite = null;
            lastselectedSprite = null;
        }
        private void spriteProperties_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

    }
}
