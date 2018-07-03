using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormMono
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            if (of.ShowDialog() == DialogResult.OK)
            {
                drawTest1.CreateProject(Path.GetDirectoryName(of.FileName));
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            drawTest1.lightworld = !drawTest1.lightworld;
            drawTest1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(128, 128);
            drawTest1.DrawPalettes(Graphics.FromImage(pictureBox1.Image));
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            drawTest1.setMode(radioButton1.Checked, radioButton2.Checked, radioButton3.Checked,checkBox1.Checked,radioButton4.Checked);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            drawTest1.SaveEntrances();
        }
    }


}
