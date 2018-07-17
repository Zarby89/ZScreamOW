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
    public partial class AddSpriteForm : Form
    {
        public AddSpriteForm()
        {
            InitializeComponent();
        }

        private void AddSpriteForm_Load(object sender, EventArgs e)
        {
            spritesNames.Clear();
            for(int i = 0;i<Sprites_Names.name.Length;i++)
            {
                spritesNames.Add(Sprites_Names.name[i],(byte)i);
                listBox1.Items.Add(Sprites_Names.name[i]);
            }
        }

        public void sortObject()
        {

            listBox1.BeginUpdate();
            listBox1.Items.Clear();
            string searchText = textBox1.Text.ToLower();
            listBox1.Items.AddRange(Sprites_Names.name
                .Where(x => x != null)
                .Where(x => (x.ToLower().Contains(searchText)))
                .Select(x => x) //?
                .ToArray());
            listBox1.EndUpdate();
        }

        public Dictionary<string,byte> spritesNames = new Dictionary<string, byte>();
        public byte pickedSprite = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            pickedSprite = spritesNames[listBox1.Items[listBox1.SelectedIndex].ToString()];
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            sortObject();
        }
    }
}
