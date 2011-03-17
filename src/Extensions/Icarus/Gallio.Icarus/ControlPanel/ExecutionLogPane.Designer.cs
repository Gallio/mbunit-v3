namespace Gallio.Icarus.ControlPanel
{
    internal partial class ExecutionLogPane
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.recursivelyDisplayChildrenCheckBox = new System.Windows.Forms.CheckBox();
            this.generalGroupBox = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.generalGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // recursivelyDisplayChildrenCheckBox
            // 
            this.recursivelyDisplayChildrenCheckBox.AutoSize = true;
            this.recursivelyDisplayChildrenCheckBox.Location = new System.Drawing.Point(17, 28);
            this.recursivelyDisplayChildrenCheckBox.Name = "recursivelyDisplayChildrenCheckBox";
            this.recursivelyDisplayChildrenCheckBox.Size = new System.Drawing.Size(156, 17);
            this.recursivelyDisplayChildrenCheckBox.TabIndex = 5;
            this.recursivelyDisplayChildrenCheckBox.Text = "Recursively display children";
            this.recursivelyDisplayChildrenCheckBox.UseVisualStyleBackColor = true;
            this.recursivelyDisplayChildrenCheckBox.CheckedChanged += new System.EventHandler(this.recursivelyDisplayChildrenCheckBox_CheckedChanged);
            // 
            // generalGroupBox
            // 
            this.generalGroupBox.Controls.Add(this.checkBox1);
            this.generalGroupBox.Controls.Add(this.recursivelyDisplayChildrenCheckBox);
            this.generalGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generalGroupBox.Location = new System.Drawing.Point(0, 0);
            this.generalGroupBox.Name = "generalGroupBox";
            this.generalGroupBox.Size = new System.Drawing.Size(397, 320);
            this.generalGroupBox.TabIndex = 8;
            this.generalGroupBox.TabStop = false;
            this.generalGroupBox.Text = "General";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(-15, -15);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // ExecutionLogPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.generalGroupBox);
            this.Name = "ExecutionLogPane";
            this.Size = new System.Drawing.Size(397, 320);
            this.generalGroupBox.ResumeLayout(false);
            this.generalGroupBox.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.CheckBox recursivelyDisplayChildrenCheckBox;
        private System.Windows.Forms.GroupBox generalGroupBox;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}
