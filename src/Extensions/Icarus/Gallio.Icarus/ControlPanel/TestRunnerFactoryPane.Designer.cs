namespace Gallio.Icarus.ControlPanel
{
    internal partial class TestRunnerFactoryPane
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
            this.testRunnerFactoryGroupBox = new System.Windows.Forms.GroupBox();
            this.testRunnerFactoryLabel = new System.Windows.Forms.Label();
            this.testRunnerFactories = new System.Windows.Forms.ComboBox();
            this.testRunnerFactoryGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // testRunnerFactoryGroupBox
            // 
            this.testRunnerFactoryGroupBox.Controls.Add(this.testRunnerFactoryLabel);
            this.testRunnerFactoryGroupBox.Controls.Add(this.testRunnerFactories);
            this.testRunnerFactoryGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testRunnerFactoryGroupBox.Location = new System.Drawing.Point(0, 0);
            this.testRunnerFactoryGroupBox.Name = "testRunnerFactoryGroupBox";
            this.testRunnerFactoryGroupBox.Size = new System.Drawing.Size(397, 320);
            this.testRunnerFactoryGroupBox.TabIndex = 9;
            this.testRunnerFactoryGroupBox.TabStop = false;
            // 
            // testRunnerFactoryLabel
            // 
            this.testRunnerFactoryLabel.AutoSize = true;
            this.testRunnerFactoryLabel.Location = new System.Drawing.Point(14, 28);
            this.testRunnerFactoryLabel.Name = "testRunnerFactoryLabel";
            this.testRunnerFactoryLabel.Size = new System.Drawing.Size(102, 13);
            this.testRunnerFactoryLabel.TabIndex = 4;
            this.testRunnerFactoryLabel.Text = "Test runner factory: ";
            // 
            // testRunnerFactories
            // 
            this.testRunnerFactories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.testRunnerFactories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.testRunnerFactories.DropDownWidth = 200;
            this.testRunnerFactories.FormattingEnabled = true;
            this.testRunnerFactories.Location = new System.Drawing.Point(122, 25);
            this.testRunnerFactories.Name = "testRunnerFactories";
            this.testRunnerFactories.Size = new System.Drawing.Size(256, 21);
            this.testRunnerFactories.TabIndex = 3;
            this.testRunnerFactories.SelectedIndexChanged += new System.EventHandler(this.testRunnerFactories_SelectedIndexChanged);
            // 
            // TestRunnerFactoryPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.testRunnerFactoryGroupBox);
            this.Name = "TestRunnerFactoryPane";
            this.Size = new System.Drawing.Size(397, 320);
            this.testRunnerFactoryGroupBox.ResumeLayout(false);
            this.testRunnerFactoryGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox testRunnerFactoryGroupBox;
        private System.Windows.Forms.Label testRunnerFactoryLabel;
        private System.Windows.Forms.ComboBox testRunnerFactories;
    }
}
