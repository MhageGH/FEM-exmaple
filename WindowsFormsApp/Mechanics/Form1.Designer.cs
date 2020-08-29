namespace Mechanics
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
            this.label_parameter = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMeshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.materialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ElasticityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ElasticityToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.PoissonsRatioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PoissonsRatioToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.thicknessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ThicknessToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.unitForceNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UnitForceToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.mmpixelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scaleToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.solveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elementInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meshEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parameterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripComboBox();
            this.meshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elementNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nodeNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elementNumberToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label_state = new System.Windows.Forms.Label();
            this.label_mesh = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_parameter
            // 
            this.label_parameter.AutoSize = true;
            this.label_parameter.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_parameter.Location = new System.Drawing.Point(570, 167);
            this.label_parameter.Name = "label_parameter";
            this.label_parameter.Size = new System.Drawing.Size(13, 18);
            this.label_parameter.TabIndex = 1;
            this.label_parameter.Text = "-";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.solverToolStripMenuItem,
            this.toolToolStripMenuItem,
            this.displayToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(665, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadMeshToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem1.Text = "File";
            // 
            // loadMeshToolStripMenuItem
            // 
            this.loadMeshToolStripMenuItem.Name = "loadMeshToolStripMenuItem";
            this.loadMeshToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.loadMeshToolStripMenuItem.Text = "Open";
            this.loadMeshToolStripMenuItem.Click += new System.EventHandler(this.loadMeshToolStripMenuItem_Click);
            // 
            // solverToolStripMenuItem
            // 
            this.solverToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.materialToolStripMenuItem,
            this.solveToolStripMenuItem,
            this.elementInformationToolStripMenuItem});
            this.solverToolStripMenuItem.Name = "solverToolStripMenuItem";
            this.solverToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.solverToolStripMenuItem.Text = "Calculation";
            // 
            // materialToolStripMenuItem
            // 
            this.materialToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ElasticityToolStripMenuItem,
            this.PoissonsRatioToolStripMenuItem,
            this.thicknessToolStripMenuItem,
            this.unitForceNToolStripMenuItem,
            this.mmpixelToolStripMenuItem});
            this.materialToolStripMenuItem.Name = "materialToolStripMenuItem";
            this.materialToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.materialToolStripMenuItem.Text = "Parameter";
            // 
            // ElasticityToolStripMenuItem
            // 
            this.ElasticityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ElasticityToolStripTextBox});
            this.ElasticityToolStripMenuItem.Name = "ElasticityToolStripMenuItem";
            this.ElasticityToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.ElasticityToolStripMenuItem.Text = "Elasticity [GPa]";
            // 
            // ElasticityToolStripTextBox
            // 
            this.ElasticityToolStripTextBox.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.ElasticityToolStripTextBox.Name = "ElasticityToolStripTextBox";
            this.ElasticityToolStripTextBox.Size = new System.Drawing.Size(100, 23);
            this.ElasticityToolStripTextBox.Text = "1.0";
            // 
            // PoissonsRatioToolStripMenuItem
            // 
            this.PoissonsRatioToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PoissonsRatioToolStripTextBox});
            this.PoissonsRatioToolStripMenuItem.Name = "PoissonsRatioToolStripMenuItem";
            this.PoissonsRatioToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.PoissonsRatioToolStripMenuItem.Text = "Poissons ratio";
            // 
            // PoissonsRatioToolStripTextBox
            // 
            this.PoissonsRatioToolStripTextBox.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.PoissonsRatioToolStripTextBox.Name = "PoissonsRatioToolStripTextBox";
            this.PoissonsRatioToolStripTextBox.Size = new System.Drawing.Size(100, 23);
            this.PoissonsRatioToolStripTextBox.Text = "0.3";
            // 
            // thicknessToolStripMenuItem
            // 
            this.thicknessToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ThicknessToolStripTextBox});
            this.thicknessToolStripMenuItem.Name = "thicknessToolStripMenuItem";
            this.thicknessToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.thicknessToolStripMenuItem.Text = "Thickness [mm]";
            // 
            // ThicknessToolStripTextBox
            // 
            this.ThicknessToolStripTextBox.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.ThicknessToolStripTextBox.Name = "ThicknessToolStripTextBox";
            this.ThicknessToolStripTextBox.Size = new System.Drawing.Size(100, 23);
            this.ThicknessToolStripTextBox.Text = "1.0";
            // 
            // unitForceNToolStripMenuItem
            // 
            this.unitForceNToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UnitForceToolStripTextBox});
            this.unitForceNToolStripMenuItem.Name = "unitForceNToolStripMenuItem";
            this.unitForceNToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.unitForceNToolStripMenuItem.Text = "Unit force [N]";
            // 
            // UnitForceToolStripTextBox
            // 
            this.UnitForceToolStripTextBox.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.UnitForceToolStripTextBox.Name = "UnitForceToolStripTextBox";
            this.UnitForceToolStripTextBox.Size = new System.Drawing.Size(100, 23);
            this.UnitForceToolStripTextBox.Text = "50";
            // 
            // mmpixelToolStripMenuItem
            // 
            this.mmpixelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scaleToolStripTextBox});
            this.mmpixelToolStripMenuItem.Name = "mmpixelToolStripMenuItem";
            this.mmpixelToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.mmpixelToolStripMenuItem.Text = "Scale [mm/pixel]";
            // 
            // scaleToolStripTextBox
            // 
            this.scaleToolStripTextBox.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.scaleToolStripTextBox.Name = "scaleToolStripTextBox";
            this.scaleToolStripTextBox.Size = new System.Drawing.Size(100, 23);
            this.scaleToolStripTextBox.Text = "1.0";
            this.scaleToolStripTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.scaleToolStripTextBox_KeyPress);
            // 
            // solveToolStripMenuItem
            // 
            this.solveToolStripMenuItem.Name = "solveToolStripMenuItem";
            this.solveToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.solveToolStripMenuItem.Text = "Solve";
            this.solveToolStripMenuItem.Click += new System.EventHandler(this.solveToolStripMenuItem_Click);
            // 
            // elementInformationToolStripMenuItem
            // 
            this.elementInformationToolStripMenuItem.Name = "elementInformationToolStripMenuItem";
            this.elementInformationToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.elementInformationToolStripMenuItem.Text = "Element information";
            this.elementInformationToolStripMenuItem.Click += new System.EventHandler(this.elementInformationToolStripMenuItem_Click);
            // 
            // toolToolStripMenuItem
            // 
            this.toolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.meshEditorToolStripMenuItem});
            this.toolToolStripMenuItem.Name = "toolToolStripMenuItem";
            this.toolToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.toolToolStripMenuItem.Text = "Tool";
            // 
            // meshEditorToolStripMenuItem
            // 
            this.meshEditorToolStripMenuItem.Name = "meshEditorToolStripMenuItem";
            this.meshEditorToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.meshEditorToolStripMenuItem.Text = "Mesh Editor";
            this.meshEditorToolStripMenuItem.Click += new System.EventHandler(this.meshEditorToolStripMenuItem_Click);
            // 
            // displayToolStripMenuItem
            // 
            this.displayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.parameterToolStripMenuItem,
            this.meshToolStripMenuItem});
            this.displayToolStripMenuItem.Name = "displayToolStripMenuItem";
            this.displayToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.displayToolStripMenuItem.Text = "View";
            // 
            // parameterToolStripMenuItem
            // 
            this.parameterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2});
            this.parameterToolStripMenuItem.Name = "parameterToolStripMenuItem";
            this.parameterToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.parameterToolStripMenuItem.Text = "Parameter";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Items.AddRange(new object[] {
            "u",
            "v",
            "ε_x",
            "ε_y",
            "γ_xy",
            "σ_x",
            "σ_y",
            "τ_xy"});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(80, 23);
            this.toolStripMenuItem2.Text = "-";
            this.toolStripMenuItem2.SelectedIndexChanged += new System.EventHandler(this.toolStripMenuItem2_SelectedIndexChanged);
            // 
            // meshToolStripMenuItem
            // 
            this.meshToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nodeToolStripMenuItem,
            this.elementNumberToolStripMenuItem,
            this.nodeNumberToolStripMenuItem,
            this.elementNumberToolStripMenuItem1,
            this.fixToolStripMenuItem,
            this.forceToolStripMenuItem});
            this.meshToolStripMenuItem.Name = "meshToolStripMenuItem";
            this.meshToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.meshToolStripMenuItem.Text = "Mesh";
            // 
            // nodeToolStripMenuItem
            // 
            this.nodeToolStripMenuItem.Name = "nodeToolStripMenuItem";
            this.nodeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.nodeToolStripMenuItem.Text = "Node";
            // 
            // elementNumberToolStripMenuItem
            // 
            this.elementNumberToolStripMenuItem.Name = "elementNumberToolStripMenuItem";
            this.elementNumberToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.elementNumberToolStripMenuItem.Text = "Element";
            // 
            // nodeNumberToolStripMenuItem
            // 
            this.nodeNumberToolStripMenuItem.Name = "nodeNumberToolStripMenuItem";
            this.nodeNumberToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.nodeNumberToolStripMenuItem.Text = "Node number";
            // 
            // elementNumberToolStripMenuItem1
            // 
            this.elementNumberToolStripMenuItem1.Name = "elementNumberToolStripMenuItem1";
            this.elementNumberToolStripMenuItem1.Size = new System.Drawing.Size(160, 22);
            this.elementNumberToolStripMenuItem1.Text = "Element number";
            // 
            // fixToolStripMenuItem
            // 
            this.fixToolStripMenuItem.Name = "fixToolStripMenuItem";
            this.fixToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.fixToolStripMenuItem.Text = "Fix";
            // 
            // forceToolStripMenuItem
            // 
            this.forceToolStripMenuItem.Name = "forceToolStripMenuItem";
            this.forceToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.forceToolStripMenuItem.Text = "Force";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(524, 167);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 18);
            this.label1.TabIndex = 12;
            this.label1.Text = "View";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(522, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 18);
            this.label2.TabIndex = 13;
            this.label2.Text = "Mesh";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(524, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 18);
            this.label3.TabIndex = 14;
            this.label3.Text = "State";
            // 
            // label_state
            // 
            this.label_state.AutoSize = true;
            this.label_state.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_state.Location = new System.Drawing.Point(570, 149);
            this.label_state.Name = "label_state";
            this.label_state.Size = new System.Drawing.Size(13, 18);
            this.label_state.TabIndex = 15;
            this.label_state.Text = "-";
            // 
            // label_mesh
            // 
            this.label_mesh.AutoSize = true;
            this.label_mesh.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_mesh.Location = new System.Drawing.Point(570, 131);
            this.label_mesh.Name = "label_mesh";
            this.label_mesh.Size = new System.Drawing.Size(13, 18);
            this.label_mesh.TabIndex = 16;
            this.label_mesh.Text = "-";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 550);
            this.Controls.Add(this.label_mesh);
            this.Controls.Add(this.label_state);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label_parameter);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Mechanical Analysis";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label_parameter;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem loadMeshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem solverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem solveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meshEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem parameterToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem nodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem elementNumberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nodeNumberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem elementNumberToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fixToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_state;
        private System.Windows.Forms.Label label_mesh;
        private System.Windows.Forms.ToolStripMenuItem materialToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ElasticityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PoissonsRatioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem thicknessToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox ElasticityToolStripTextBox;
        private System.Windows.Forms.ToolStripTextBox PoissonsRatioToolStripTextBox;
        private System.Windows.Forms.ToolStripTextBox ThicknessToolStripTextBox;
        private System.Windows.Forms.ToolStripMenuItem unitForceNToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox UnitForceToolStripTextBox;
        private System.Windows.Forms.ToolStripMenuItem mmpixelToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox scaleToolStripTextBox;
        private System.Windows.Forms.ToolStripMenuItem elementInformationToolStripMenuItem;
    }
}

