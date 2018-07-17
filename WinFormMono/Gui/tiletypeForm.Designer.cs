namespace WinFormMono
{
    partial class tiletypeForm
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
            this.blocksetpictureBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tiletypesetUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.tileUpDown = new System.Windows.Forms.NumericUpDown();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.selectedtypeUpDown = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.gfxUpDown = new System.Windows.Forms.NumericUpDown();
            this.paletteUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.blocksetpictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tiletypesetUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tileUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.selectedtypeUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gfxUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // blocksetpictureBox
            // 
            this.blocksetpictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.blocksetpictureBox.Location = new System.Drawing.Point(258, 9);
            this.blocksetpictureBox.Name = "blocksetpictureBox";
            this.blocksetpictureBox.Size = new System.Drawing.Size(256, 512);
            this.blocksetpictureBox.TabIndex = 0;
            this.blocksetpictureBox.TabStop = false;
            this.blocksetpictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.blocksetpictureBox_Paint);
            this.blocksetpictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.blocksetpictureBox_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Tiletype Set : ";
            // 
            // tiletypesetUpDown
            // 
            this.tiletypesetUpDown.Location = new System.Drawing.Point(12, 25);
            this.tiletypesetUpDown.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.tiletypesetUpDown.Name = "tiletypesetUpDown";
            this.tiletypesetUpDown.Size = new System.Drawing.Size(114, 20);
            this.tiletypesetUpDown.TabIndex = 2;
            this.tiletypesetUpDown.ValueChanged += new System.EventHandler(this.tiletypesetUpDown_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(129, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Tile : ";
            // 
            // tileUpDown
            // 
            this.tileUpDown.Location = new System.Drawing.Point(132, 25);
            this.tileUpDown.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.tileUpDown.Name = "tileUpDown";
            this.tileUpDown.Size = new System.Drawing.Size(114, 20);
            this.tileUpDown.TabIndex = 4;
            this.tileUpDown.ValueChanged += new System.EventHandler(this.tileUpDown_ValueChanged);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 126);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(234, 394);
            this.listBox1.TabIndex = 5;
            this.listBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDoubleClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Tile type for the selected Tile :";
            // 
            // selectedtypeUpDown
            // 
            this.selectedtypeUpDown.Location = new System.Drawing.Point(12, 104);
            this.selectedtypeUpDown.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.selectedtypeUpDown.Name = "selectedtypeUpDown";
            this.selectedtypeUpDown.Size = new System.Drawing.Size(234, 20);
            this.selectedtypeUpDown.TabIndex = 7;
            this.selectedtypeUpDown.ValueChanged += new System.EventHandler(this.selectedtypeUpDown_ValueChanged);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(439, 527);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(358, 527);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(230, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Gfx, Palette (Visual only - no effect on anything)";
            // 
            // gfxUpDown
            // 
            this.gfxUpDown.Location = new System.Drawing.Point(12, 64);
            this.gfxUpDown.Maximum = new decimal(new int[] {
            79,
            0,
            0,
            0});
            this.gfxUpDown.Name = "gfxUpDown";
            this.gfxUpDown.Size = new System.Drawing.Size(114, 20);
            this.gfxUpDown.TabIndex = 11;
            this.gfxUpDown.ValueChanged += new System.EventHandler(this.gfxUpDown_ValueChanged);
            // 
            // paletteUpDown
            // 
            this.paletteUpDown.Location = new System.Drawing.Point(132, 64);
            this.paletteUpDown.Maximum = new decimal(new int[] {
            79,
            0,
            0,
            0});
            this.paletteUpDown.Name = "paletteUpDown";
            this.paletteUpDown.Size = new System.Drawing.Size(114, 20);
            this.paletteUpDown.TabIndex = 12;
            this.paletteUpDown.ValueChanged += new System.EventHandler(this.paletteUpDown_ValueChanged);
            // 
            // tiletypeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 558);
            this.Controls.Add(this.paletteUpDown);
            this.Controls.Add(this.gfxUpDown);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.selectedtypeUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.tileUpDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tiletypesetUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.blocksetpictureBox);
            this.Name = "tiletypeForm";
            this.Text = "Tile Type Set Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.tiletypeForm_FormClosed);
            this.Load += new System.EventHandler(this.tiletypeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.blocksetpictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tiletypesetUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tileUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.selectedtypeUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gfxUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox blocksetpictureBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown tiletypesetUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown tileUpDown;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown selectedtypeUpDown;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown gfxUpDown;
        private System.Windows.Forms.NumericUpDown paletteUpDown;
    }
}