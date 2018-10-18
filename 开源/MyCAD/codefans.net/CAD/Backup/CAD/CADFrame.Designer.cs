namespace CAD
{
    partial class CADFrame
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnLine = new System.Windows.Forms.Button();
            this.btnHand = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnRedo = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSave = new System.Windows.Forms.ToolStripMenuItem();
            this.btnExit = new System.Windows.Forms.ToolStripMenuItem();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.btnRectangle = new System.Windows.Forms.Button();
            this.btnEllipse = new System.Windows.Forms.Button();
            this.btnCircle = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.picLineWidth = new System.Windows.Forms.PictureBox();
            this.tbarLineWidth = new System.Windows.Forms.TrackBar();
            this.txtLineWidth = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.White = new System.Windows.Forms.Button();
            this.Blue = new System.Windows.Forms.Button();
            this.Green = new System.Windows.Forms.Button();
            this.Yellow = new System.Windows.Forms.Button();
            this.Black = new System.Windows.Forms.Button();
            this.Magente = new System.Windows.Forms.Button();
            this.Red = new System.Windows.Forms.Button();
            this.Cyan = new System.Windows.Forms.Button();
            this.btnMoreColor = new System.Windows.Forms.Button();
            this.picCurrentColor = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLineWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbarLineWidth)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCurrentColor)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLine
            // 
            this.btnLine.Location = new System.Drawing.Point(6, 40);
            this.btnLine.Name = "btnLine";
            this.btnLine.Size = new System.Drawing.Size(46, 23);
            this.btnLine.TabIndex = 0;
            this.btnLine.Text = "直线";
            this.btnLine.UseVisualStyleBackColor = true;
            this.btnLine.Click += new System.EventHandler(this.btnLine_Click);
            // 
            // btnHand
            // 
            this.btnHand.Location = new System.Drawing.Point(6, 127);
            this.btnHand.Name = "btnHand";
            this.btnHand.Size = new System.Drawing.Size(46, 23);
            this.btnHand.TabIndex = 0;
            this.btnHand.Text = "变换";
            this.btnHand.UseVisualStyleBackColor = true;
            this.btnHand.Click += new System.EventHandler(this.btnHand_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(6, 98);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(46, 23);
            this.btnUndo.TabIndex = 0;
            this.btnUndo.Text = "&<-";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnRedo
            // 
            this.btnRedo.Location = new System.Drawing.Point(64, 98);
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(46, 23);
            this.btnRedo.TabIndex = 0;
            this.btnRedo.Text = "&->";
            this.btnRedo.UseVisualStyleBackColor = true;
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(64, 127);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(46, 23);
            this.btnClear.TabIndex = 0;
            this.btnClear.Text = "清空";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBox1.Location = new System.Drawing.Point(119, 40);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(537, 418);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "CAD|*.CAD|ALL|*.*";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "CAD|*.CAD|ALL|*.*";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(662, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpen,
            this.btnSave,
            this.btnExit});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // btnOpen
            // 
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(94, 22);
            this.btnOpen.Text = "打开";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click_1);
            // 
            // btnSave
            // 
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(94, 22);
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
            // 
            // btnExit
            // 
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(94, 22);
            this.btnExit.Text = "退出";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnRectangle
            // 
            this.btnRectangle.Location = new System.Drawing.Point(64, 40);
            this.btnRectangle.Name = "btnRectangle";
            this.btnRectangle.Size = new System.Drawing.Size(46, 23);
            this.btnRectangle.TabIndex = 0;
            this.btnRectangle.Text = "矩形";
            this.btnRectangle.UseVisualStyleBackColor = true;
            this.btnRectangle.Click += new System.EventHandler(this.btnRectangle_Click);
            // 
            // btnEllipse
            // 
            this.btnEllipse.Location = new System.Drawing.Point(6, 69);
            this.btnEllipse.Name = "btnEllipse";
            this.btnEllipse.Size = new System.Drawing.Size(46, 23);
            this.btnEllipse.TabIndex = 0;
            this.btnEllipse.Text = "椭圆";
            this.btnEllipse.UseVisualStyleBackColor = true;
            this.btnEllipse.Click += new System.EventHandler(this.btnEllipse_Click);
            // 
            // btnCircle
            // 
            this.btnCircle.Location = new System.Drawing.Point(64, 69);
            this.btnCircle.Name = "btnCircle";
            this.btnCircle.Size = new System.Drawing.Size(46, 23);
            this.btnCircle.TabIndex = 0;
            this.btnCircle.Text = "圆形";
            this.btnCircle.UseVisualStyleBackColor = true;
            this.btnCircle.Click += new System.EventHandler(this.btnCircle_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.picLineWidth);
            this.groupBox1.Controls.Add(this.tbarLineWidth);
            this.groupBox1.Controls.Add(this.txtLineWidth);
            this.groupBox1.Location = new System.Drawing.Point(2, 317);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(111, 141);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "线宽";
            // 
            // picLineWidth
            // 
            this.picLineWidth.Location = new System.Drawing.Point(6, 90);
            this.picLineWidth.Name = "picLineWidth";
            this.picLineWidth.Size = new System.Drawing.Size(99, 39);
            this.picLineWidth.TabIndex = 2;
            this.picLineWidth.TabStop = false;
            // 
            // tbarLineWidth
            // 
            this.tbarLineWidth.Location = new System.Drawing.Point(3, 48);
            this.tbarLineWidth.Maximum = 30;
            this.tbarLineWidth.Name = "tbarLineWidth";
            this.tbarLineWidth.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbarLineWidth.Size = new System.Drawing.Size(101, 45);
            this.tbarLineWidth.TabIndex = 1;
            this.tbarLineWidth.Scroll += new System.EventHandler(this.tbarLineWidth_Scroll);
            // 
            // txtLineWidth
            // 
            this.txtLineWidth.Location = new System.Drawing.Point(13, 21);
            this.txtLineWidth.Name = "txtLineWidth";
            this.txtLineWidth.ReadOnly = true;
            this.txtLineWidth.Size = new System.Drawing.Size(83, 21);
            this.txtLineWidth.TabIndex = 0;
            this.txtLineWidth.Text = "1";
            this.txtLineWidth.TextChanged += new System.EventHandler(this.txtLineWidth_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.White);
            this.groupBox2.Controls.Add(this.Blue);
            this.groupBox2.Controls.Add(this.Green);
            this.groupBox2.Controls.Add(this.Yellow);
            this.groupBox2.Controls.Add(this.Black);
            this.groupBox2.Controls.Add(this.Magente);
            this.groupBox2.Controls.Add(this.Red);
            this.groupBox2.Controls.Add(this.Cyan);
            this.groupBox2.Controls.Add(this.btnMoreColor);
            this.groupBox2.Controls.Add(this.picCurrentColor);
            this.groupBox2.Location = new System.Drawing.Point(2, 156);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(111, 155);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "颜色";
            // 
            // White
            // 
            this.White.BackColor = System.Drawing.Color.White;
            this.White.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.White.Location = new System.Drawing.Point(75, 84);
            this.White.Name = "White";
            this.White.Size = new System.Drawing.Size(30, 27);
            this.White.TabIndex = 2;
            this.White.UseVisualStyleBackColor = false;
            this.White.Click += new System.EventHandler(this.White_Click);
            // 
            // Blue
            // 
            this.Blue.BackColor = System.Drawing.Color.Blue;
            this.Blue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Blue.Location = new System.Drawing.Point(39, 117);
            this.Blue.Name = "Blue";
            this.Blue.Size = new System.Drawing.Size(30, 27);
            this.Blue.TabIndex = 2;
            this.Blue.UseVisualStyleBackColor = false;
            this.Blue.Click += new System.EventHandler(this.Blue_Click);
            // 
            // Green
            // 
            this.Green.BackColor = System.Drawing.Color.Green;
            this.Green.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Green.Location = new System.Drawing.Point(39, 84);
            this.Green.Name = "Green";
            this.Green.Size = new System.Drawing.Size(30, 27);
            this.Green.TabIndex = 2;
            this.Green.UseVisualStyleBackColor = false;
            this.Green.Click += new System.EventHandler(this.Green_Click);
            // 
            // Yellow
            // 
            this.Yellow.BackColor = System.Drawing.Color.Yellow;
            this.Yellow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Yellow.Location = new System.Drawing.Point(4, 117);
            this.Yellow.Name = "Yellow";
            this.Yellow.Size = new System.Drawing.Size(30, 27);
            this.Yellow.TabIndex = 2;
            this.Yellow.UseVisualStyleBackColor = false;
            this.Yellow.Click += new System.EventHandler(this.Yellow_Click);
            // 
            // Black
            // 
            this.Black.BackColor = System.Drawing.Color.Black;
            this.Black.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Black.Location = new System.Drawing.Point(75, 52);
            this.Black.Name = "Black";
            this.Black.Size = new System.Drawing.Size(30, 27);
            this.Black.TabIndex = 2;
            this.Black.UseVisualStyleBackColor = false;
            this.Black.Click += new System.EventHandler(this.Black_Click);
            // 
            // Magente
            // 
            this.Magente.BackColor = System.Drawing.Color.Magenta;
            this.Magente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Magente.Location = new System.Drawing.Point(4, 84);
            this.Magente.Name = "Magente";
            this.Magente.Size = new System.Drawing.Size(30, 27);
            this.Magente.TabIndex = 2;
            this.Magente.UseVisualStyleBackColor = false;
            this.Magente.Click += new System.EventHandler(this.Magente_Click);
            // 
            // Red
            // 
            this.Red.BackColor = System.Drawing.Color.Red;
            this.Red.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Red.Location = new System.Drawing.Point(39, 52);
            this.Red.Name = "Red";
            this.Red.Size = new System.Drawing.Size(30, 27);
            this.Red.TabIndex = 2;
            this.Red.UseVisualStyleBackColor = false;
            this.Red.Click += new System.EventHandler(this.Red_Click);
            // 
            // Cyan
            // 
            this.Cyan.BackColor = System.Drawing.Color.Cyan;
            this.Cyan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Cyan.Location = new System.Drawing.Point(4, 52);
            this.Cyan.Name = "Cyan";
            this.Cyan.Size = new System.Drawing.Size(30, 27);
            this.Cyan.TabIndex = 2;
            this.Cyan.UseVisualStyleBackColor = false;
            this.Cyan.Click += new System.EventHandler(this.Cyan_Click);
            // 
            // btnMoreColor
            // 
            this.btnMoreColor.BackColor = System.Drawing.Color.Transparent;
            this.btnMoreColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoreColor.Location = new System.Drawing.Point(75, 117);
            this.btnMoreColor.Name = "btnMoreColor";
            this.btnMoreColor.Size = new System.Drawing.Size(30, 27);
            this.btnMoreColor.TabIndex = 2;
            this.btnMoreColor.Text = ">>";
            this.btnMoreColor.UseVisualStyleBackColor = false;
            this.btnMoreColor.Click += new System.EventHandler(this.btnMoreColor_Click);
            // 
            // picCurrentColor
            // 
            this.picCurrentColor.Location = new System.Drawing.Point(38, 11);
            this.picCurrentColor.Name = "picCurrentColor";
            this.picCurrentColor.Size = new System.Drawing.Size(31, 32);
            this.picCurrentColor.TabIndex = 0;
            this.picCurrentColor.TabStop = false;
            // 
            // CADFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 466);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnRedo);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.btnHand);
            this.Controls.Add(this.btnCircle);
            this.Controls.Add(this.btnEllipse);
            this.Controls.Add(this.btnRectangle);
            this.Controls.Add(this.btnLine);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(670, 500);
            this.MinimumSize = new System.Drawing.Size(670, 500);
            this.Name = "CADFrame";
            this.Load += new System.EventHandler(this.CADFrame_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLineWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbarLineWidth)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picCurrentColor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLine;
        private System.Windows.Forms.Button btnHand;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnRedo;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnOpen;
        private System.Windows.Forms.ToolStripMenuItem btnSave;
        private System.Windows.Forms.ToolStripMenuItem btnExit;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button btnRectangle;
        private System.Windows.Forms.Button btnEllipse;
        private System.Windows.Forms.Button btnCircle;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TrackBar tbarLineWidth;
        private System.Windows.Forms.TextBox txtLineWidth;
        private System.Windows.Forms.PictureBox picLineWidth;
        private System.Windows.Forms.PictureBox picCurrentColor;
        private System.Windows.Forms.Button btnMoreColor;
        private System.Windows.Forms.Button White;
        private System.Windows.Forms.Button Blue;
        private System.Windows.Forms.Button Green;
        private System.Windows.Forms.Button Yellow;
        private System.Windows.Forms.Button Black;
        private System.Windows.Forms.Button Magente;
        private System.Windows.Forms.Button Red;
        private System.Windows.Forms.Button Cyan;
    }
}

