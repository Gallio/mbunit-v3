namespace Gallio.Ambience.UI.Preferences
{
    partial class AmbienceServerPreferencePane
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
            this.outerTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.launchServicesTool = new System.Windows.Forms.Button();
            this.startModeLabel = new System.Windows.Forms.Label();
            this.outerTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // outerTableLayoutPanel
            // 
            this.outerTableLayoutPanel.AutoSize = true;
            this.outerTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.outerTableLayoutPanel.ColumnCount = 1;
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.outerTableLayoutPanel.Controls.Add(this.launchServicesTool, 0, 1);
            this.outerTableLayoutPanel.Controls.Add(this.startModeLabel, 0, 0);
            this.outerTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outerTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.outerTableLayoutPanel.Name = "outerTableLayoutPanel";
            this.outerTableLayoutPanel.Padding = new System.Windows.Forms.Padding(8);
            this.outerTableLayoutPanel.RowCount = 3;
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outerTableLayoutPanel.Size = new System.Drawing.Size(300, 320);
            this.outerTableLayoutPanel.TabIndex = 0;
            // 
            // launchServicesTool
            // 
            this.launchServicesTool.AutoSize = true;
            this.launchServicesTool.Location = new System.Drawing.Point(28, 24);
            this.launchServicesTool.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.launchServicesTool.Name = "launchServicesTool";
            this.launchServicesTool.Size = new System.Drawing.Size(153, 23);
            this.launchServicesTool.TabIndex = 0;
            this.launchServicesTool.Text = "Launch Services Admin Tool";
            this.launchServicesTool.UseVisualStyleBackColor = true;
            this.launchServicesTool.Click += new System.EventHandler(this.launchServicesTool_Click);
            // 
            // startModeLabel
            // 
            this.startModeLabel.AutoSize = true;
            this.startModeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startModeLabel.Location = new System.Drawing.Point(11, 8);
            this.startModeLabel.Name = "startModeLabel";
            this.startModeLabel.Size = new System.Drawing.Size(278, 13);
            this.startModeLabel.TabIndex = 1;
            this.startModeLabel.Text = "Configure Gallio Ambience service user and start mode:";
            // 
            // AmbienceServerPreferencePane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.outerTableLayoutPanel);
            this.Name = "AmbienceServerPreferencePane";
            this.outerTableLayoutPanel.ResumeLayout(false);
            this.outerTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel outerTableLayoutPanel;
        private System.Windows.Forms.Button launchServicesTool;
        private System.Windows.Forms.Label startModeLabel;


    }
}