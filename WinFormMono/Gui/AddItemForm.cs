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
    public partial class AddItemForm : Form
    {
        public AddItemForm()
        {
            InitializeComponent();
        }
        public Dictionary<string, byte> itemsNames = new Dictionary<string, byte>();
        public byte pickedItem = 0;
        private void AddItemForm_Load(object sender, EventArgs e)
        {
            itemsNames.Clear();
            for (int i = 0; i < ItemsNames.name.Length; i++)
            {
                byte nid = (byte)i;
                if (nid >= 0x17)
                {
                    nid -= 0x17;
                    nid *= 2;
                    nid += 0x80;
                    
                }
                itemsNames.Add(ItemsNames.name[i], (byte)nid);
                listBox1.Items.Add(ItemsNames.name[i]);
            }
        }

        public void sortObject()
        {

            listBox1.BeginUpdate();
            listBox1.Items.Clear();
            string searchText = textBox1.Text.ToLower();
            listBox1.Items.AddRange(ItemsNames.name
                .Where(x => x != null)
                .Where(x => (x.ToLower().Contains(searchText)))
                .Select(x => x) //?
                .ToArray());
            listBox1.EndUpdate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pickedItem = itemsNames[listBox1.Items[listBox1.SelectedIndex].ToString()];
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            sortObject();
        }
    }
}
