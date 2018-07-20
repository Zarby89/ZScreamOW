namespace WinFormMono
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createNewProjectFromROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.patchROMWithLoadedProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setEmulatorPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.overworldEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dungeonEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runTesttoolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.editorsToolStripMenuItem,
            this.runTesttoolStripMenu,
            this.scriptsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.createNewProjectFromROMToolStripMenuItem,
            this.patchROMWithLoadedProjectToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.loadToolStripMenuItem.Text = "Load Project .zscr";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.saveToolStripMenuItem.Text = "Save Project .zscr";
            // 
            // createNewProjectFromROMToolStripMenuItem
            // 
            this.createNewProjectFromROMToolStripMenuItem.Name = "createNewProjectFromROMToolStripMenuItem";
            this.createNewProjectFromROMToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.createNewProjectFromROMToolStripMenuItem.Text = "Create new project from ROM";
            this.createNewProjectFromROMToolStripMenuItem.Click += new System.EventHandler(this.createNewProjectFromROMToolStripMenuItem_Click);
            // 
            // patchROMWithLoadedProjectToolStripMenuItem
            // 
            this.patchROMWithLoadedProjectToolStripMenuItem.Name = "patchROMWithLoadedProjectToolStripMenuItem";
            this.patchROMWithLoadedProjectToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.patchROMWithLoadedProjectToolStripMenuItem.Text = "Patch ROM with current project";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setEmulatorPathToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // setEmulatorPathToolStripMenuItem
            // 
            this.setEmulatorPathToolStripMenuItem.Name = "setEmulatorPathToolStripMenuItem";
            this.setEmulatorPathToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.setEmulatorPathToolStripMenuItem.Text = "Set testing emulator";
            this.setEmulatorPathToolStripMenuItem.Click += new System.EventHandler(this.setEmulatorPathToolStripMenuItem_Click);
            // 
            // editorsToolStripMenuItem
            // 
            this.editorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.overworldEditorToolStripMenuItem,
            this.dungeonEditorToolStripMenuItem});
            this.editorsToolStripMenuItem.Name = "editorsToolStripMenuItem";
            this.editorsToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.editorsToolStripMenuItem.Text = "Editors";
            // 
            // overworldEditorToolStripMenuItem
            // 
            this.overworldEditorToolStripMenuItem.Name = "overworldEditorToolStripMenuItem";
            this.overworldEditorToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.overworldEditorToolStripMenuItem.Text = "Overworld Editor";
            this.overworldEditorToolStripMenuItem.Click += new System.EventHandler(this.overworldEditorToolStripMenuItem_Click);
            // 
            // dungeonEditorToolStripMenuItem
            // 
            this.dungeonEditorToolStripMenuItem.Name = "dungeonEditorToolStripMenuItem";
            this.dungeonEditorToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.dungeonEditorToolStripMenuItem.Text = "Dungeon Editor";
            // 
            // runTesttoolStripMenu
            // 
            this.runTesttoolStripMenu.Image = ((System.Drawing.Image)(resources.GetObject("runTesttoolStripMenu.Image")));
            this.runTesttoolStripMenu.Name = "runTesttoolStripMenu";
            this.runTesttoolStripMenu.Size = new System.Drawing.Size(80, 20);
            this.runTesttoolStripMenu.Text = "Run Test";
            this.runTesttoolStripMenu.Click += new System.EventHandler(this.runTesttoolStripMenu_Click);
            // 
            // scriptsToolStripMenuItem
            // 
            this.scriptsToolStripMenuItem.Name = "scriptsToolStripMenuItem";
            this.scriptsToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.scriptsToolStripMenuItem.Text = "Asm Scripts";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "ZScream Magic";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem overworldEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dungeonEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createNewProjectFromROMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem patchROMWithLoadedProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runTesttoolStripMenu;
        private System.Windows.Forms.ToolStripMenuItem setEmulatorPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scriptsToolStripMenuItem;
    }
}