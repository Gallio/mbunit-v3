namespace Gallio.UI.ControlPanel.Preferences
{
    partial class RuntimePreferencePane
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
            this.runtimePathTextBox = new System.Windows.Forms.TextBox();
            this.pluginPathLabel = new System.Windows.Forms.Label();
            this.installationPathLabel = new System.Windows.Forms.Label();
            this.outerTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pluginPathListBox = new System.Windows.Forms.ListBox();
            this.pluginPathEditButtonLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.addPluginPathButton = new System.Windows.Forms.Button();
            this.removePluginPathButton = new System.Windows.Forms.Button();
            this.outerTableLayoutPanel.SuspendLayout();
            this.pluginPathEditButtonLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // runtimePathTextBox
            // 
            this.runtimePathTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.runtimePathTextBox.Location = new System.Drawing.Point(20, 16);
            this.runtimePathTextBox.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.runtimePathTextBox.Name = "runtimePathTextBox";
            this.runtimePathTextBox.ReadOnly = true;
            this.runtimePathTextBox.Size = new System.Drawing.Size(277, 20);
            this.runtimePathTextBox.TabIndex = 2;
            // 
            // pluginPathLabel
            // 
            this.pluginPathLabel.AutoSize = true;
            this.pluginPathLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pluginPathLabel.Location = new System.Drawing.Point(3, 39);
            this.pluginPathLabel.Name = "pluginPathLabel";
            this.pluginPathLabel.Size = new System.Drawing.Size(294, 13);
            this.pluginPathLabel.TabIndex = 1;
            this.pluginPathLabel.Text = "Additional plugin paths:";
            // 
            // installationPathLabel
            // 
            this.installationPathLabel.AutoSize = true;
            this.installationPathLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.installationPathLabel.Location = new System.Drawing.Point(3, 0);
            this.installationPathLabel.Name = "installationPathLabel";
            this.installationPathLabel.Size = new System.Drawing.Size(294, 13);
            this.installationPathLabel.TabIndex = 0;
            this.installationPathLabel.Text = "Installation path:";
            // 
            // outerTableLayoutPanel
            // 
            this.outerTableLayoutPanel.AutoSize = true;
            this.outerTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.outerTableLayoutPanel.ColumnCount = 1;
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outerTableLayoutPanel.Controls.Add(this.installationPathLabel, 0, 0);
            this.outerTableLayoutPanel.Controls.Add(this.pluginPathLabel, 0, 2);
            this.outerTableLayoutPanel.Controls.Add(this.runtimePathTextBox, 0, 1);
            this.outerTableLayoutPanel.Controls.Add(this.pluginPathListBox, 0, 3);
            this.outerTableLayoutPanel.Controls.Add(this.pluginPathEditButtonLayoutPanel, 0, 4);
            this.outerTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outerTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.outerTableLayoutPanel.Name = "outerTableLayoutPanel";
            this.outerTableLayoutPanel.RowCount = 5;
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.Size = new System.Drawing.Size(300, 350);
            this.outerTableLayoutPanel.TabIndex = 0;
            // 
            // pluginPathListBox
            // 
            this.pluginPathListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pluginPathListBox.FormattingEnabled = true;
            this.pluginPathListBox.Location = new System.Drawing.Point(20, 55);
            this.pluginPathListBox.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.pluginPathListBox.Name = "pluginPathListBox";
            this.pluginPathListBox.Size = new System.Drawing.Size(277, 251);
            this.pluginPathListBox.TabIndex = 3;
            this.pluginPathListBox.SelectedIndexChanged += new System.EventHandler(this.pluginPathListBox_SelectedIndexChanged);
            // 
            // pluginPathEditButtonLayoutPanel
            // 
            this.pluginPathEditButtonLayoutPanel.AutoSize = true;
            this.pluginPathEditButtonLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pluginPathEditButtonLayoutPanel.Controls.Add(this.addPluginPathButton);
            this.pluginPathEditButtonLayoutPanel.Controls.Add(this.removePluginPathButton);
            this.pluginPathEditButtonLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pluginPathEditButtonLayoutPanel.Location = new System.Drawing.Point(20, 318);
            this.pluginPathEditButtonLayoutPanel.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.pluginPathEditButtonLayoutPanel.Name = "pluginPathEditButtonLayoutPanel";
            this.pluginPathEditButtonLayoutPanel.Size = new System.Drawing.Size(277, 29);
            this.pluginPathEditButtonLayoutPanel.TabIndex = 4;
            // 
            // addPluginPathButton
            // 
            this.addPluginPathButton.Location = new System.Drawing.Point(3, 3);
            this.addPluginPathButton.Name = "addPluginPathButton";
            this.addPluginPathButton.Size = new System.Drawing.Size(75, 23);
            this.addPluginPathButton.TabIndex = 0;
            this.addPluginPathButton.Text = "Add...";
            this.addPluginPathButton.UseVisualStyleBackColor = true;
            this.addPluginPathButton.Click += new System.EventHandler(this.addPluginPathButton_Click);
            // 
            // removePluginPathButton
            // 
            this.removePluginPathButton.Enabled = false;
            this.removePluginPathButton.Location = new System.Drawing.Point(84, 3);
            this.removePluginPathButton.Name = "removePluginPathButton";
            this.removePluginPathButton.Size = new System.Drawing.Size(75, 23);
            this.removePluginPathButton.TabIndex = 1;
            this.removePluginPathButton.Text = "Remove";
            this.removePluginPathButton.UseVisualStyleBackColor = true;
            this.removePluginPathButton.Click += new System.EventHandler(this.removePluginPathButton_Click);
            // 
            // RuntimePreferencePane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.outerTableLayoutPanel);
            this.Name = "RuntimePreferencePane";
            this.Load += new System.EventHandler(this.PathPreferencePane_Load);
            this.outerTableLayoutPanel.ResumeLayout(false);
            this.outerTableLayoutPanel.PerformLayout();
            this.pluginPathEditButtonLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox runtimePathTextBox;
        private System.Windows.Forms.Label pluginPathLabel;
        private System.Windows.Forms.Label installationPathLabel;
        private System.Windows.Forms.TableLayoutPanel outerTableLayoutPanel;
        private System.Windows.Forms.ListBox pluginPathListBox;
        private System.Windows.Forms.FlowLayoutPanel pluginPathEditButtonLayoutPanel;
        private System.Windows.Forms.Button addPluginPathButton;
        private System.Windows.Forms.Button removePluginPathButton;


    }
}
