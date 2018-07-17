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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            //SUB.w #$0400 : AND.w #$0F80 : ASL A : XBA : STA $88
            //LDA $84 : SUB.w #$0010 : AND.w #$003E : LSR A : STA $86
            

        }
        OverworldForm overworldForm = new OverworldForm();
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            if (of.ShowDialog() == DialogResult.OK)
            {
                JsonData data = new JsonData(Path.GetDirectoryName(of.FileName));
                overworldForm.setData(data);
                overworldForm.Enabled = true;
                

            }



        }

        private void overworldEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (overworldForm.Visible == false)
            //{
                overworldForm.MdiParent = this;
                overworldForm.Show();
           // }
        }
    }
}
