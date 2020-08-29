namespace Mechanics
{
    partial class Form2
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.NodePositionListLabel = new System.Windows.Forms.Label();
            this.NodeNumberListLabel = new System.Windows.Forms.Label();
            this.ElementNumberLabel = new System.Windows.Forms.Label();
            this.ElementTypeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Element type : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "Element number : ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "Node number list : ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 18);
            this.label4.TabIndex = 3;
            this.label4.Text = "Node position list : ";
            // 
            // NodePositionListLabel
            // 
            this.NodePositionListLabel.AutoSize = true;
            this.NodePositionListLabel.Location = new System.Drawing.Point(136, 63);
            this.NodePositionListLabel.Name = "NodePositionListLabel";
            this.NodePositionListLabel.Size = new System.Drawing.Size(13, 18);
            this.NodePositionListLabel.TabIndex = 7;
            this.NodePositionListLabel.Text = "-";
            // 
            // NodeNumberListLabel
            // 
            this.NodeNumberListLabel.AutoSize = true;
            this.NodeNumberListLabel.Location = new System.Drawing.Point(136, 45);
            this.NodeNumberListLabel.Name = "NodeNumberListLabel";
            this.NodeNumberListLabel.Size = new System.Drawing.Size(13, 18);
            this.NodeNumberListLabel.TabIndex = 6;
            this.NodeNumberListLabel.Text = "-";
            // 
            // ElementNumberLabel
            // 
            this.ElementNumberLabel.AutoSize = true;
            this.ElementNumberLabel.Location = new System.Drawing.Point(136, 27);
            this.ElementNumberLabel.Name = "ElementNumberLabel";
            this.ElementNumberLabel.Size = new System.Drawing.Size(13, 18);
            this.ElementNumberLabel.TabIndex = 5;
            this.ElementNumberLabel.Text = "-";
            // 
            // ElementTypeLabel
            // 
            this.ElementTypeLabel.AutoSize = true;
            this.ElementTypeLabel.Location = new System.Drawing.Point(136, 9);
            this.ElementTypeLabel.Name = "ElementTypeLabel";
            this.ElementTypeLabel.Size = new System.Drawing.Size(13, 18);
            this.ElementTypeLabel.TabIndex = 4;
            this.ElementTypeLabel.Text = "-";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 367);
            this.Controls.Add(this.NodePositionListLabel);
            this.Controls.Add(this.NodeNumberListLabel);
            this.Controls.Add(this.ElementNumberLabel);
            this.Controls.Add(this.ElementTypeLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form2";
            this.Text = "Element information";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.Label ElementTypeLabel;
        internal System.Windows.Forms.Label NodePositionListLabel;
        internal System.Windows.Forms.Label NodeNumberListLabel;
        internal System.Windows.Forms.Label ElementNumberLabel;
    }
}