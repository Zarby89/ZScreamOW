using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormMono
{
    public partial class EntranceEditorForm : Form
    {
        public EntranceEditorForm()
        {
            InitializeComponent();
        }
        public int choice;
        private void button1_Click(object sender, EventArgs e)
        {
            choice = listBox1.SelectedIndex;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
