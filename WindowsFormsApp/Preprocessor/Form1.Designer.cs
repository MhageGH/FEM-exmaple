namespace Preprocessor
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.radioButton_node = new System.Windows.Forms.RadioButton();
            this.radioButton_triangle = new System.Windows.Forms.RadioButton();
            this.radioButton_fixX = new System.Windows.Forms.RadioButton();
            this.radioButton_fixY = new System.Windows.Forms.RadioButton();
            this.radioButton_forceY = new System.Windows.Forms.RadioButton();
            this.radioButton_forceX = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton_quadrangle = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton_align = new System.Windows.Forms.RadioButton();
            this.radioButton_add = new System.Windows.Forms.RadioButton();
            this.radioButton_move = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox_VisibleQuadrangleNumber = new System.Windows.Forms.CheckBox();
            this.checkBox_VisibleTriangleNumber = new System.Windows.Forms.CheckBox();
            this.checkBox_VisibleNodeNumber = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parameterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unitLengthmmpixelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scaleToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButton_node
            // 
            this.radioButton_node.AutoSize = true;
            this.radioButton_node.Checked = true;
            this.radioButton_node.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton_node.Location = new System.Drawing.Point(6, 25);
            this.radioButton_node.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_node.Name = "radioButton_node";
            this.radioButton_node.Size = new System.Drawing.Size(54, 22);
            this.radioButton_node.TabIndex = 0;
            this.radioButton_node.TabStop = true;
            this.radioButton_node.Text = "node";
            this.radioButton_node.UseVisualStyleBackColor = true;
            this.radioButton_node.CheckedChanged += new System.EventHandler(this.radioButton_node_CheckedChanged);
            // 
            // radioButton_triangle
            // 
            this.radioButton_triangle.AutoSize = true;
            this.radioButton_triangle.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton_triangle.Location = new System.Drawing.Point(6, 55);
            this.radioButton_triangle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_triangle.Name = "radioButton_triangle";
            this.radioButton_triangle.Size = new System.Drawing.Size(70, 22);
            this.radioButton_triangle.TabIndex = 1;
            this.radioButton_triangle.Text = "triangle";
            this.radioButton_triangle.UseVisualStyleBackColor = true;
            this.radioButton_triangle.CheckedChanged += new System.EventHandler(this.radioButton_triangle_CheckedChanged);
            // 
            // radioButton_fixX
            // 
            this.radioButton_fixX.AutoSize = true;
            this.radioButton_fixX.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton_fixX.Location = new System.Drawing.Point(6, 114);
            this.radioButton_fixX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_fixX.Name = "radioButton_fixX";
            this.radioButton_fixX.Size = new System.Drawing.Size(52, 22);
            this.radioButton_fixX.TabIndex = 3;
            this.radioButton_fixX.Text = "fix X";
            this.radioButton_fixX.UseVisualStyleBackColor = true;
            this.radioButton_fixX.CheckedChanged += new System.EventHandler(this.radioButton_fixX_CheckedChanged);
            // 
            // radioButton_fixY
            // 
            this.radioButton_fixY.AutoSize = true;
            this.radioButton_fixY.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton_fixY.Location = new System.Drawing.Point(6, 144);
            this.radioButton_fixY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_fixY.Name = "radioButton_fixY";
            this.radioButton_fixY.Size = new System.Drawing.Size(52, 22);
            this.radioButton_fixY.TabIndex = 4;
            this.radioButton_fixY.Text = "fix Y";
            this.radioButton_fixY.UseVisualStyleBackColor = true;
            this.radioButton_fixY.CheckedChanged += new System.EventHandler(this.radioButton_fixY_CheckedChanged);
            // 
            // radioButton_forceY
            // 
            this.radioButton_forceY.AutoSize = true;
            this.radioButton_forceY.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton_forceY.Location = new System.Drawing.Point(6, 204);
            this.radioButton_forceY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_forceY.Name = "radioButton_forceY";
            this.radioButton_forceY.Size = new System.Drawing.Size(67, 22);
            this.radioButton_forceY.TabIndex = 6;
            this.radioButton_forceY.Text = "force Y";
            this.radioButton_forceY.UseVisualStyleBackColor = true;
            this.radioButton_forceY.CheckedChanged += new System.EventHandler(this.radioButton_forceY_CheckedChanged);
            // 
            // radioButton_forceX
            // 
            this.radioButton_forceX.AutoSize = true;
            this.radioButton_forceX.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton_forceX.Location = new System.Drawing.Point(6, 174);
            this.radioButton_forceX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_forceX.Name = "radioButton_forceX";
            this.radioButton_forceX.Size = new System.Drawing.Size(67, 22);
            this.radioButton_forceX.TabIndex = 5;
            this.radioButton_forceX.Text = "force X";
            this.radioButton_forceX.UseVisualStyleBackColor = true;
            this.radioButton_forceX.CheckedChanged += new System.EventHandler(this.radioButton_forceX_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton_quadrangle);
            this.groupBox1.Controls.Add(this.radioButton_node);
            this.groupBox1.Controls.Add(this.radioButton_forceY);
            this.groupBox1.Controls.Add(this.radioButton_triangle);
            this.groupBox1.Controls.Add(this.radioButton_forceX);
            this.groupBox1.Controls.Add(this.radioButton_fixX);
            this.groupBox1.Controls.Add(this.radioButton_fixY);
            this.groupBox1.Location = new System.Drawing.Point(511, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(142, 236);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "mode";
            // 
            // radioButton_quadrangle
            // 
            this.radioButton_quadrangle.AutoSize = true;
            this.radioButton_quadrangle.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton_quadrangle.Location = new System.Drawing.Point(6, 84);
            this.radioButton_quadrangle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_quadrangle.Name = "radioButton_quadrangle";
            this.radioButton_quadrangle.Size = new System.Drawing.Size(90, 22);
            this.radioButton_quadrangle.TabIndex = 7;
            this.radioButton_quadrangle.Text = "quadrangle";
            this.radioButton_quadrangle.UseVisualStyleBackColor = true;
            this.radioButton_quadrangle.CheckedChanged += new System.EventHandler(this.radioButton_quadrangle_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton_align);
            this.groupBox2.Controls.Add(this.radioButton_add);
            this.groupBox2.Controls.Add(this.radioButton_move);
            this.groupBox2.Location = new System.Drawing.Point(511, 269);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(142, 120);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "operation";
            // 
            // radioButton_align
            // 
            this.radioButton_align.AutoSize = true;
            this.radioButton_align.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton_align.Location = new System.Drawing.Point(6, 85);
            this.radioButton_align.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_align.Name = "radioButton_align";
            this.radioButton_align.Size = new System.Drawing.Size(53, 22);
            this.radioButton_align.TabIndex = 4;
            this.radioButton_align.Text = "align";
            this.radioButton_align.UseVisualStyleBackColor = true;
            this.radioButton_align.CheckedChanged += new System.EventHandler(this.radioButton_align_CheckedChanged);
            // 
            // radioButton_add
            // 
            this.radioButton_add.AutoSize = true;
            this.radioButton_add.Checked = true;
            this.radioButton_add.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton_add.Location = new System.Drawing.Point(6, 25);
            this.radioButton_add.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_add.Name = "radioButton_add";
            this.radioButton_add.Size = new System.Drawing.Size(105, 22);
            this.radioButton_add.TabIndex = 2;
            this.radioButton_add.TabStop = true;
            this.radioButton_add.Text = "add / remove";
            this.radioButton_add.UseVisualStyleBackColor = true;
            this.radioButton_add.CheckedChanged += new System.EventHandler(this.radioButton_add_CheckedChanged);
            // 
            // radioButton_move
            // 
            this.radioButton_move.AutoSize = true;
            this.radioButton_move.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton_move.Location = new System.Drawing.Point(6, 55);
            this.radioButton_move.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton_move.Name = "radioButton_move";
            this.radioButton_move.Size = new System.Drawing.Size(59, 22);
            this.radioButton_move.TabIndex = 3;
            this.radioButton_move.Text = "move";
            this.radioButton_move.UseVisualStyleBackColor = true;
            this.radioButton_move.CheckedChanged += new System.EventHandler(this.radioButton_move_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox_VisibleQuadrangleNumber);
            this.groupBox3.Controls.Add(this.checkBox_VisibleTriangleNumber);
            this.groupBox3.Controls.Add(this.checkBox_VisibleNodeNumber);
            this.groupBox3.Location = new System.Drawing.Point(511, 395);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(142, 115);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "visible";
            // 
            // checkBox_VisibleQuadrangleNumber
            // 
            this.checkBox_VisibleQuadrangleNumber.AutoSize = true;
            this.checkBox_VisibleQuadrangleNumber.Location = new System.Drawing.Point(6, 80);
            this.checkBox_VisibleQuadrangleNumber.Name = "checkBox_VisibleQuadrangleNumber";
            this.checkBox_VisibleQuadrangleNumber.Size = new System.Drawing.Size(140, 22);
            this.checkBox_VisibleQuadrangleNumber.TabIndex = 2;
            this.checkBox_VisibleQuadrangleNumber.Text = "quadrangle number";
            this.checkBox_VisibleQuadrangleNumber.UseVisualStyleBackColor = true;
            this.checkBox_VisibleQuadrangleNumber.CheckedChanged += new System.EventHandler(this.checkBox_VisibleQuadrangleNumber_CheckedChanged);
            // 
            // checkBox_VisibleTriangleNumber
            // 
            this.checkBox_VisibleTriangleNumber.AutoSize = true;
            this.checkBox_VisibleTriangleNumber.Location = new System.Drawing.Point(6, 52);
            this.checkBox_VisibleTriangleNumber.Name = "checkBox_VisibleTriangleNumber";
            this.checkBox_VisibleTriangleNumber.Size = new System.Drawing.Size(120, 22);
            this.checkBox_VisibleTriangleNumber.TabIndex = 1;
            this.checkBox_VisibleTriangleNumber.Text = "triangle number";
            this.checkBox_VisibleTriangleNumber.UseVisualStyleBackColor = true;
            this.checkBox_VisibleTriangleNumber.CheckedChanged += new System.EventHandler(this.checkBox_VisibleTriangleNumber_CheckedChanged);
            // 
            // checkBox_VisibleNodeNumber
            // 
            this.checkBox_VisibleNodeNumber.AutoSize = true;
            this.checkBox_VisibleNodeNumber.Location = new System.Drawing.Point(6, 24);
            this.checkBox_VisibleNodeNumber.Name = "checkBox_VisibleNodeNumber";
            this.checkBox_VisibleNodeNumber.Size = new System.Drawing.Size(104, 22);
            this.checkBox_VisibleNodeNumber.TabIndex = 0;
            this.checkBox_VisibleNodeNumber.Text = "node number";
            this.checkBox_VisibleNodeNumber.UseVisualStyleBackColor = true;
            this.checkBox_VisibleNodeNumber.CheckedChanged += new System.EventHandler(this.checkBox_VisibleNodeNumber_CheckedChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 584);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(665, 22);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(175, 17);
            this.toolStripStatusLabel1.Text = "Left click to put or remove node";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.parameterToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(665, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // parameterToolStripMenuItem
            // 
            this.parameterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.unitLengthmmpixelToolStripMenuItem});
            this.parameterToolStripMenuItem.Name = "parameterToolStripMenuItem";
            this.parameterToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.parameterToolStripMenuItem.Text = "Parameter";
            // 
            // unitLengthmmpixelToolStripMenuItem
            // 
            this.unitLengthmmpixelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scaleToolStripTextBox});
            this.unitLengthmmpixelToolStripMenuItem.Name = "unitLengthmmpixelToolStripMenuItem";
            this.unitLengthmmpixelToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.unitLengthmmpixelToolStripMenuItem.Text = "Scale [mm/pixel]";
            // 
            // scaleToolStripTextBox
            // 
            this.scaleToolStripTextBox.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.scaleToolStripTextBox.Name = "scaleToolStripTextBox";
            this.scaleToolStripTextBox.Size = new System.Drawing.Size(100, 23);
            this.scaleToolStripTextBox.Text = "1.0";
            this.scaleToolStripTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.scaleToolStripTextBox_KeyPress);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 606);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Mesh Editor";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButton_node;
        private System.Windows.Forms.RadioButton radioButton_triangle;
        private System.Windows.Forms.RadioButton radioButton_fixX;
        private System.Windows.Forms.RadioButton radioButton_fixY;
        private System.Windows.Forms.RadioButton radioButton_forceY;
        private System.Windows.Forms.RadioButton radioButton_forceX;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton_quadrangle;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButton_align;
        private System.Windows.Forms.RadioButton radioButton_add;
        private System.Windows.Forms.RadioButton radioButton_move;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBox_VisibleNodeNumber;
        private System.Windows.Forms.CheckBox checkBox_VisibleTriangleNumber;
        private System.Windows.Forms.CheckBox checkBox_VisibleQuadrangleNumber;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem parameterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unitLengthmmpixelToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox scaleToolStripTextBox;
    }
}

