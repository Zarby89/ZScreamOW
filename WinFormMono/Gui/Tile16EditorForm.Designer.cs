namespace WinFormMono
{
    partial class Tile16EditorForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.tilePicturebox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tileidUpDown = new System.Windows.Forms.NumericUpDown();
            this.paletteUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.mirrorxCheckbox = new System.Windows.Forms.CheckBox();
            this.mirroryCheckbox = new System.Windows.Forms.CheckBox();
            this.ontopCheckbox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.panel2 = new System.Windows.Forms.Panel();
            this.blocksetPicturebox = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tiletypeButton = new System.Windows.Forms.Button();
            this.showpalettesButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tilePicturebox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tileidUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blocksetPicturebox)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.tilePicturebox);
            this.panel1.Location = new System.Drawing.Point(12, 90);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(68, 68);
            this.panel1.TabIndex = 0;
            // 
            // tilePicturebox
            // 
            this.tilePicturebox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tilePicturebox.Location = new System.Drawing.Point(0, 0);
            this.tilePicturebox.Name = "tilePicturebox";
            this.tilePicturebox.Size = new System.Drawing.Size(64, 64);
            this.tilePicturebox.TabIndex = 3;
            this.tilePicturebox.TabStop = false;
            this.tilePicturebox.Paint += new System.Windows.Forms.PaintEventHandler(this.tilePicturebox_Paint);
            this.tilePicturebox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tilePicturebox_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Tile ID : ";
            // 
            // tileidUpDown
            // 
            this.tileidUpDown.Location = new System.Drawing.Point(12, 25);
            this.tileidUpDown.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.tileidUpDown.Name = "tileidUpDown";
            this.tileidUpDown.Size = new System.Drawing.Size(68, 20);
            this.tileidUpDown.TabIndex = 2;
            this.tileidUpDown.ValueChanged += new System.EventHandler(this.tileidUpDown_ValueChanged);
            // 
            // paletteUpDown
            // 
            this.paletteUpDown.Location = new System.Drawing.Point(12, 177);
            this.paletteUpDown.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.paletteUpDown.Name = "paletteUpDown";
            this.paletteUpDown.Size = new System.Drawing.Size(68, 20);
            this.paletteUpDown.TabIndex = 3;
            this.paletteUpDown.ValueChanged += new System.EventHandler(this.paletteUpDown_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Palette : ";
            // 
            // mirrorxCheckbox
            // 
            this.mirrorxCheckbox.AutoSize = true;
            this.mirrorxCheckbox.Location = new System.Drawing.Point(12, 203);
            this.mirrorxCheckbox.Name = "mirrorxCheckbox";
            this.mirrorxCheckbox.Size = new System.Drawing.Size(62, 17);
            this.mirrorxCheckbox.TabIndex = 5;
            this.mirrorxCheckbox.Text = "Mirror X";
            this.mirrorxCheckbox.UseVisualStyleBackColor = true;
            this.mirrorxCheckbox.CheckedChanged += new System.EventHandler(this.mirrorxCheckbox_CheckedChanged);
            // 
            // mirroryCheckbox
            // 
            this.mirroryCheckbox.AutoSize = true;
            this.mirroryCheckbox.Location = new System.Drawing.Point(12, 226);
            this.mirroryCheckbox.Name = "mirroryCheckbox";
            this.mirroryCheckbox.Size = new System.Drawing.Size(62, 17);
            this.mirroryCheckbox.TabIndex = 6;
            this.mirroryCheckbox.Text = "Mirror Y";
            this.mirroryCheckbox.UseVisualStyleBackColor = true;
            this.mirroryCheckbox.CheckedChanged += new System.EventHandler(this.mirrorxCheckbox_CheckedChanged);
            // 
            // ontopCheckbox
            // 
            this.ontopCheckbox.AutoSize = true;
            this.ontopCheckbox.Location = new System.Drawing.Point(12, 249);
            this.ontopCheckbox.Name = "ontopCheckbox";
            this.ontopCheckbox.Size = new System.Drawing.Size(62, 17);
            this.ontopCheckbox.TabIndex = 7;
            this.ontopCheckbox.Text = "On Top";
            this.ontopCheckbox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Tile Type :";
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Location = new System.Drawing.Point(12, 64);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(68, 20);
            this.numericUpDown3.TabIndex = 9;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel2.AutoScroll = true;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.blocksetPicturebox);
            this.panel2.Location = new System.Drawing.Point(136, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(277, 255);
            this.panel2.TabIndex = 10;
            // 
            // blocksetPicturebox
            // 
            this.blocksetPicturebox.Location = new System.Drawing.Point(0, 0);
            this.blocksetPicturebox.Name = "blocksetPicturebox";
            this.blocksetPicturebox.Size = new System.Drawing.Size(256, 1024);
            this.blocksetPicturebox.TabIndex = 0;
            this.blocksetPicturebox.TabStop = false;
            this.blocksetPicturebox.Paint += new System.Windows.Forms.PaintEventHandler(this.blocksetPicturebox_Paint);
            this.blocksetPicturebox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.blocksetPicturebox_MouseDown);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(338, 273);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(257, 273);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // tiletypeButton
            // 
            this.tiletypeButton.Location = new System.Drawing.Point(86, 64);
            this.tiletypeButton.Name = "tiletypeButton";
            this.tiletypeButton.Size = new System.Drawing.Size(44, 20);
            this.tiletypeButton.TabIndex = 0;
            this.tiletypeButton.Text = "Show";
            this.tiletypeButton.UseVisualStyleBackColor = true;
            this.tiletypeButton.Click += new System.EventHandler(this.tiletypeButton_Click);
            // 
            // showpalettesButton
            // 
            this.showpalettesButton.Location = new System.Drawing.Point(86, 177);
            this.showpalettesButton.Name = "showpalettesButton";
            this.showpalettesButton.Size = new System.Drawing.Size(44, 20);
            this.showpalettesButton.TabIndex = 13;
            this.showpalettesButton.Text = "Show";
            this.showpalettesButton.UseVisualStyleBackColor = true;
            // 
            // Tile16EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 298);
            this.Controls.Add(this.showpalettesButton);
            this.Controls.Add(this.tiletypeButton);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.numericUpDown3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ontopCheckbox);
            this.Controls.Add(this.mirroryCheckbox);
            this.Controls.Add(this.mirrorxCheckbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.paletteUpDown);
            this.Controls.Add(this.tileidUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.MaximumSize = new System.Drawing.Size(436, 1024);
            this.Name = "Tile16EditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tile16EditorForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Tile16EditorForm_FormClosed);
            this.Load += new System.EventHandler(this.Tile16EditorForm_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tilePicturebox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tileidUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.blocksetPicturebox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox tilePicturebox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown tileidUpDown;
        private System.Windows.Forms.NumericUpDown paletteUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox mirrorxCheckbox;
        private System.Windows.Forms.CheckBox mirroryCheckbox;
        private System.Windows.Forms.CheckBox ontopCheckbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button tiletypeButton;
        private System.Windows.Forms.Button showpalettesButton;
        private System.Windows.Forms.PictureBox blocksetPicturebox;
    }
}