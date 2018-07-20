using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormMono
{
    public partial class OverworldForm : Form
    {
        public OverworldForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            statecomboBox1.SelectedIndex = 0;
            toolStripRedoButton.Enabled = false;

            

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        ushort[,] copiedTiles;
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            KeyEventArgs e = new KeyEventArgs(keyData);
            if (e.Control && e.KeyCode == Keys.Z)
            {
                overworldDisplay.Undo();
                return true; // handled
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                copiedTiles = (ushort[,])overworldDisplay.scene.selectedTiles.Clone();
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                overworldDisplay.scene.selectedTiles = copiedTiles;
                overworldDisplay.scene.setOverlaytiles(allgfx16Ptr);

            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            tilesetPicturebox.Image = new Bitmap(128, 128);
            overworldDisplay.DrawPalettes(Graphics.FromImage(tilesetPicturebox.Image));
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            //overworldDisplay.setMode(radioButton1.Checked, radioButton2.Checked, radioButton3.Checked,checkBox1.Checked,radioButton4.Checked);
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
        JsonData jsonData;
        public void setData(JsonData jsonData)
        {
            overworldDisplay.setData(jsonData);
            this.jsonData = jsonData;
            overworldDisplay.CreateProject();
        }
        IntPtr allgfx16Ptr = Marshal.AllocHGlobal(128 * 7520);
        bool updateScroll = false;
        private void tilesetPicturebox_Paint(object sender, PaintEventArgs e)
        {
            Map16 map = overworldDisplay.selectedMap;




            if (map != null)
            {
                //musicUpDown.Value = jsonData.mapdata[map.index].m
                Bitmap tilesgfx = new Bitmap(128, 8192, 128, PixelFormat.Format8bppIndexed, allgfx16Ptr);
                tilesgfx.Palette = map.GetPalette();
                map.UpdateGfx(jsonData.allgfx8array, allgfx16Ptr, jsonData.tilesetBitmaps);
                e.Graphics.DrawImage(tilesgfx, new Rectangle(0, 0, 128, 8192), 0, 0, 128, 8192, GraphicsUnit.Pixel);
                //e.Graphics.DrawImage(tilesgfx, new Rectangle(128, 0, 128, 3760), 0, 3760, 128, 3760, GraphicsUnit.Pixel);


            }

            int y = (overworldDisplay.selectedTile / 8);
            int x = overworldDisplay.selectedTile - (y * 8);

            e.Graphics.DrawRectangle(Pens.LightGreen, new Rectangle(x * 16, y * 16, 16, 16));







            //overworldDisplay.
        }
        private void overworldDisplay_MouseClick(object sender, MouseEventArgs e)
        {
            Map16 map = overworldDisplay.selectedMap;
            Map16 lastmap = overworldDisplay.lastSelectedMap;
            if (map != null)
            {
                if (lastmap != map)
                {
                    nameTextbox.Text = jsonData.mapdata[map.index].name;
                    blocksetUpDown.Value = jsonData.mapdata[map.index].blockset;
                    paletteUpDown.Value = jsonData.mapdata[map.index].palette;
                    spritepalUpDown.Value = jsonData.mapdata[map.index].sprite_palette;
                    spritegfxUpDown.Value = jsonData.mapdata[map.index].spriteset;
                    tiletypesetUpDown.Value = jsonData.mapdata[map.index].tileTypeSet;
                    textUpDown.Value = jsonData.mapdata[map.index].msgid;
                    lastmap = map;
                }
                //Marshal.FreeHGlobal(allgfx16Ptr);
            }

        }
        private void tilesetPicturebox_MouseDown(object sender, MouseEventArgs e)
        {
            
            int y = (e.Y / 16);
            int x = (e.X / 16);
            ushort tid = (ushort)(x + (y * 8));
            if (e.Button == MouseButtons.Left)
            {
                selectedtileUpDown.Value = tid;
            }
            else if (e.Button == MouseButtons.Right)
            {
                selectedtileUpDown.Value = tid;
                Tile16EditorForm tile16Editor = new Tile16EditorForm();
                tile16Editor.setGfxData(jsonData.tilesetBitmaps, jsonData, overworldDisplay.selectedMap, tid);
                
                if (tile16Editor.ShowDialog() == DialogResult.OK)
                {
                    
                    jsonData.alltiles16[tid] = tile16Editor.editingTile;
                    overworldDisplay.selectedMap.UpdateMap(allgfx16Ptr);
                }
            }

            tilesetPicturebox.Refresh();
        }

        private void toolStripPenButton_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void toolStripPenButton_Click(object sender, EventArgs e)
        {
            toolStripPenButton.Checked = false;
            toolStripEntrancesButton.Checked = false;
            toolStripExitsButton.Checked = false;
            toolStripWarpButton.Checked = false;
            toolStripHoleButton.Checked = false;
            toolStripItemsButton.Checked = false;
            toolStripSpriteButton.Checked = false;
            overlaytoolStripButton.Checked = false;
            (sender as ToolStripButton).Checked = true;
            overworldDisplay.setMode(toolStripEntrancesButton.Checked, toolStripExitsButton.Checked, toolStripSpriteButton.Checked, toolStripPenButton.Checked,toolStripHoleButton.Checked,toolStripWarpButton.Checked,toolStripItemsButton.Checked,overlaytoolStripButton.Checked);


        }

        private void splitContainer1_Panel1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            overworldDisplay.lightworld = !overworldDisplay.lightworld;
            overworldDisplay.Refresh();
        }

        private void selectedtileUpDown_ValueChanged(object sender, EventArgs e)
        {
            overworldDisplay.selectedTile = (ushort)selectedtileUpDown.Value;
            overworldDisplay.scene.selectedTiles = new ushort[1, 1] { { overworldDisplay.selectedTile } };
            overworldDisplay.scene.setOverlaytiles(allgfx16Ptr);
            int y = (overworldDisplay.selectedTile / 8);
            int x = overworldDisplay.selectedTile - (y * 8);

            if (y * 16 > (splitContainer1.Panel1.VerticalScroll.Value + splitContainer1.Panel1.Height))
            {
                splitContainer1.Panel1.VerticalScroll.Value = (y * 16);
            }
            else if (y * 16 < (splitContainer1.Panel1.VerticalScroll.Value))
            {
                splitContainer1.Panel1.VerticalScroll.Value = (y * 16);
            }
            splitContainer1.Panel1.Refresh();
        }

        private void toolStripUndoButton_Click(object sender, EventArgs e)
        {
            overworldDisplay.Undo();
        }

        private void OverworldForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        int transparencyState = 0;

        private void transparencyButton_Click(object sender, EventArgs e)
        {
            transparencyState++;
            if (transparencyState == 4)
            {
                transparencyState = 0;
            }
            if (transparencyState == 0) //100%
            {
                transparencyButton.Text = "100%";
                overworldDisplay.scene.transparency = 255f;

            }
            else if (transparencyState == 1)//50%
            {
                transparencyButton.Text = "50%";
                overworldDisplay.scene.transparency = 127f;
            }
            else if (transparencyState == 2)//25%
            {
                transparencyButton.Text = "25%";
                overworldDisplay.scene.transparency = 75f;
            }
            else if (transparencyState == 3)//0%
            {
                transparencyButton.Text = "0%";
                overworldDisplay.scene.transparency = 0f;
            }
            overworldDisplay.Refresh();
        }
        public bool needUpdate = false;
        private void blocksetUpDown_ValueChanged(object sender, EventArgs e)
        {
            overworldDisplay.selectedMap.mapdata.blockset = (byte)blocksetUpDown.Value;
            needUpdate = true;
        }

        private void overworldDisplay_Click(object sender, EventArgs e)
        {
            overworldDisplay.selectedTile = overworldDisplay.scene.selectedTiles[0, 0];
            
            //tilesetPicturebox.Refresh();
            selectedtileUpDown.Value = overworldDisplay.selectedTile;
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            if (needUpdate == false)
            {
                return;
            }

                int pid = -1;
                if (overworldDisplay.selectedMap.largeMap == true)
                {
                    pid = overworldDisplay.selectedMap.parentMapId;
                }
                if (pid == -1)
                {
                    overworldDisplay.scene.UpdateMap(overworldDisplay.selectedMap);
                }
                else
                {
                    overworldDisplay.scene.UpdateMap(overworldDisplay.allmaps[pid]);
                    overworldDisplay.scene.UpdateMap(overworldDisplay.allmaps[pid + 1]);
                    overworldDisplay.scene.UpdateMap(overworldDisplay.allmaps[pid + 8]);
                    overworldDisplay.scene.UpdateMap(overworldDisplay.allmaps[pid + 9]);
                }
                overworldDisplay.Refresh();
                needUpdate = false;
        }

        private void paletteUpDown_ValueChanged(object sender, EventArgs e)
        {
            overworldDisplay.selectedMap.mapdata.palette = (byte)paletteUpDown.Value;
            needUpdate = true;
        }

        private void spritegfxUpDown_ValueChanged(object sender, EventArgs e)
        {
            overworldDisplay.selectedMap.mapdata.spriteset = (byte)spritegfxUpDown.Value;
            needUpdate = true;
        }

        private void spritepalUpDown_ValueChanged(object sender, EventArgs e)
        {
            overworldDisplay.selectedMap.mapdata.sprite_palette = (byte)spritepalUpDown.Value;
            needUpdate = true;
        }

        private void textUpDown_ValueChanged(object sender, EventArgs e)
        {
            overworldDisplay.selectedMap.mapdata.msgid = (byte)textUpDown.Value;
        }

        private void gridtoolStripButton_CheckedChanged(object sender, EventArgs e)
        {
            overworldDisplay.showGrid = gridtoolStripButton.Checked;
        }

        private void gridsnaptoolStripButton_CheckedChanged(object sender, EventArgs e)
        {
            overworldDisplay.scene.snapToGrid = gridsnaptoolStripButton.Checked;
            overworldDisplay.scene.debugMode = debugtoolStripButton.Checked;
        }

        private void statecomboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            overworldDisplay.scene.sceneState = (byte)statecomboBox1.SelectedIndex;
            overworldDisplay.Refresh();
        }

        private void savetoolStripButton_Click(object sender, EventArgs e)
        {
            /*for (int i = 0; i < 128; i++)
            {
                File.WriteAllText(File.ReadAllText("Exported//Overworld//Maps//Map" + i.ToString("D3") + ".json"), JsonConvert.SerializeObject(jsonData.mapdata[i]));
            }*/

            jsonData.SaveAll(overworldDisplay.allmaps);
        }

        public void saveOverworld()
        {
            jsonData.SaveAll(overworldDisplay.allmaps);
        }

        private void tiletypesetUpDown_ValueChanged(object sender, EventArgs e)
        {
            overworldDisplay.selectedMap.mapdata.tileTypeSet = (byte)tiletypesetUpDown.Value;
        }
    }


}
