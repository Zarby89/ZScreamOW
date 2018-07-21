using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
            overworldForm.Enabled = false;


        }
        OverworldForm overworldForm = new OverworldForm();
        string loadedProject = "";
        string emulatorPath = "";
        string[] configs;
        string[] allScriptsFiles;
        List<string> scriptsActive = new List<string>();
        List<string> scriptsDetected = new List<string>();
        List<string> scriptsInformation = new List<string>();
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "ZScream Project (*.zscr)|*.zscr";
            if (of.ShowDialog() == DialogResult.OK)
            {

                loadedProject = Path.GetDirectoryName(of.FileName);
                configs = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(loadedProject + "//Project.zscr"));
                emulatorPath = configs[0];
                JsonData data = new JsonData(Path.GetDirectoryName(of.FileName));
                overworldForm.setData(data);
                overworldForm.Enabled = true;
                allScriptsFiles = Directory.EnumerateFiles(loadedProject + "\\ASM").ToArray();
                foreach(string script in allScriptsFiles)
                {
                    string f = Path.GetFileName(script);
                    if (f == "Main.asm")
                    {
                        string[] usedFiles = File.ReadAllLines(script);
                        foreach (string s in usedFiles)
                        {
                            if (s.Contains("incsrc"))
                            {
                                string a = s.Remove(0, 7);
                                scriptsActive.Add(a);
                            }
                        }
                        continue;
                    }
                    if (f == "Readme.txt") { continue; }
                    scriptsDetected.Add(f);
                }
            }
            else
            {
                return;
            }

            foreach(string s in scriptsDetected)
            {
                //Create information of each scripts
                StringBuilder information = new StringBuilder();
                bool description = false;
                string[] asmfileLines = File.ReadAllLines(loadedProject + "\\ASM\\" + s);
                Console.WriteLine("Lines : " + asmfileLines.Length);
                int id = 0;
                foreach (string line in asmfileLines)
                {
                    
                    if (line.Contains(";#="))
                    {
                        if (description == true)
                        {
                            description = false;
                        }
                        else
                        {
                            description = true;
                        }
                        continue;
                    }
                    if (description == true)
                    {
                        information.AppendLine(line);
                    }
                    else
                    {
                        if (line.Length > 0)
                        {
                            if (line[0] != ';') //if line is a comment
                            {
                                if (line.Contains("org $"))
                                {

                                    string addr = line.Substring(line.IndexOf('$') + 1, 6);
                                    string comment = "";
                                    if (addr.Contains(";")) //address is shorter than 6
                                    {
                                        addr = line.Substring(line.IndexOf('$') + 1, line.IndexOf(';') - line.IndexOf('$')); ;
                                        
                                    }
                                    if (line.Contains(";"))
                                    {
                                        comment = line.Substring(line.IndexOf(';'), line.Length - line.IndexOf(';'));
                                    }
                                    int snesAddr = ParseHexString(addr);
                                    int pcAddr = ZScream_Exporter.Addresses.snestopc(snesAddr);
                                    information.AppendLine("SNES:"+snesAddr.ToString("X6") + " PC:" + pcAddr.ToString("X6") + " ;" + comment);
                                }
                            }
                        }
                    }
                    
                }
                scriptsInformation.Add(information.ToString());





                //add the script in the script list
                ToolStripItem item = scriptsToolStripMenuItem.DropDownItems.Add(s);
                if (scriptsActive.Contains(s))
                {
                    //check the script if it active
                    (item as ToolStripMenuItem).Checked = true;
                    (item as ToolStripMenuItem).CheckOnClick = true;
                }
                if (item.Text == "EditorCore.asm")
                {
                    (item as ToolStripMenuItem).CheckOnClick = false;
                }

                ToolStripItem itemInfo = scriptsToolStripMenuItem.DropDownItems.Add("Information");
                itemInfo.Tag = id;
                itemInfo.Click += ItemInfo_Click;
                (item as ToolStripMenuItem).DropDownItems.Add(itemInfo);

                //add an information tab with tag value


                id++;
            }
            Console.WriteLine(scriptsInformation[0]);

        }
        ScriptInformationForm sif = new ScriptInformationForm();
        private void ItemInfo_Click(object sender, EventArgs e)
        {
            sif.richTextBox1.Clear();
            sif.richTextBox1.Text = scriptsInformation[(int)(sender as ToolStripItem).Tag];
            sif.ShowDialog();
        }

        private static int ParseHexString(string hexNumber)
        {
            hexNumber = hexNumber.Replace("x", string.Empty);
            long result = 0;
            long.TryParse(hexNumber, System.Globalization.NumberStyles.HexNumber, null, out result);
            return (int)result;
        }

        private void overworldEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loadedProject != "")
            {
            
            overworldForm.MdiParent = this;
            overworldForm.Show();
            }
        }

        private void runTesttoolStripMenu_Click(object sender, EventArgs e)
        {
            

            if (loadedProject == "")
            {
                MessageBox.Show("You must have a project loaded first!", "Error");
                return;
            }

            overworldForm.saveOverworld();

            if (!File.Exists(loadedProject + "//TestROM//test.sfc"))
            {
                MessageBox.Show("To run a test you must have a zelda ROM called 'test.sfc' in your TestROM project folder", "Error");
                return;
            }

            if (emulatorPath == "")
            {
                MessageBox.Show("You must set an emulator first under Options menu!", "Error");
                return;
            }
            if (File.Exists(loadedProject + "//TestROM//working.sfc"))
            {
                File.Delete(loadedProject + "//TestROM//working.sfc");
            }



            FileStream fs = new FileStream(loadedProject + "//TestROM//test.sfc", FileMode.Open, FileAccess.Read);
            int size = (int)fs.Length;
            if (size < 0x200000)
            {
                size = 0x200000;
            }
            byte[] temp = new byte[size];
            fs.Read(temp, 0, (int)fs.Length);
            fs.Close();



            ZScream_Exporter.ROM.SetRom(temp, out bool isHeadered);

            temp = null;

            ZScream_Exporter.Importer importer = new ZScream_Exporter.Importer(loadedProject, ZScream_Exporter.ROM.DATA);
            


            Process.Start(emulatorPath, loadedProject + "//TestROM//working.sfc");


        }

        private void setEmulatorPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Snes Emulator (*.exe)|*.exe";
            if (of.ShowDialog() == DialogResult.OK)
            {
                configs[0] = of.FileName;
                emulatorPath = configs[0];
                File.WriteAllText(loadedProject + "//Project.zscr", JsonConvert.SerializeObject(configs));

            }
        }

        private void createNewProjectFromROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Select the rom you want to use to create your project");
            ZScream_Exporter.Exporter exporter;
            string rompath = "";
            using (OpenFileDialog of = new OpenFileDialog())
            {
                if (of.ShowDialog() == DialogResult.OK)
                {
                    FileStream fs = new FileStream(of.FileName, FileMode.Open, FileAccess.Read);
                    int size = (int)fs.Length;
                    if (size < 0x200000)
                    {
                        size = 0x200000;
                    }
                    byte[] temp = new byte[size];
                    fs.Read(temp, 0, (int)size);
                    fs.Close();
                    rompath = of.FileName;
                    ZScream_Exporter.ROM.SetRom(temp, out bool isHeadered);

                    temp = null;

                    //read the ROM if you selected OK

                }
                else
                {
                    return;
                }
            }
            MessageBox.Show("Select the folder you want to create your project in (multiples files will be created)");

            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {


                    exporter = new ZScream_Exporter.Exporter(ZScream_Exporter.ROM.DATA,fbd.SelectedPath,rompath);
                }
                else
                {
                    return;
                }
            }

            exporter = null;
           

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutForm = new AboutBox1();
            aboutForm.ShowDialog();
        }

        private void patchROMWithLoadedProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (loadedProject == "")
            {
                MessageBox.Show("You must have a project loaded first!", "Error");
                return;
            }

            overworldForm.saveOverworld();
            string fname = "";
            using (OpenFileDialog of = new OpenFileDialog())
            {
                of.ShowDialog();
                of.Filter = "Snes ROM to patch (*.sfc)|*.sfc";
                fname = of.FileName;
                if (!File.Exists(of.FileName))
                {
                    return;
                }
            }

            FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read);
            int size = (int)fs.Length;
            if (size < 0x200000)
            {
                size = 0x200000;
            }
            byte[] temp = new byte[size];
            fs.Read(temp, 0, (int)fs.Length);
            fs.Close();

            ZScream_Exporter.ROM.SetRom(temp, out bool isHeadered);

            temp = null;

            ZScream_Exporter.Importer importer = new ZScream_Exporter.Importer(loadedProject, ZScream_Exporter.ROM.DATA,fname);

            MessageBox.Show("Patched successfully " + fname.ToString());
        }
    }
}
