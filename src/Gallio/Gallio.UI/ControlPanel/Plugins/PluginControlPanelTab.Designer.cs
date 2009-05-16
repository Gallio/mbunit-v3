namespace Gallio.UI.ControlPanel.Plugins
{
    partial class PluginControlPanelTab
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
            this.pluginGrid = new System.Windows.Forms.DataGridView();
            this.pluginIconColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.pluginNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pluginVersionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.outerTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pluginIconPictureBox = new System.Windows.Forms.PictureBox();
            this.pluginDescriptionLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pluginGrid)).BeginInit();
            this.outerTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pluginIconPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pluginGrid
            // 
            this.pluginGrid.AllowUserToAddRows = false;
            this.pluginGrid.AllowUserToDeleteRows = false;
            this.pluginGrid.AllowUserToResizeRows = false;
            this.pluginGrid.CausesValidation = false;
            this.pluginGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.pluginGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.pluginIconColumn,
            this.pluginNameColumn,
            this.pluginVersionColumn});
            this.outerTableLayoutPanel.SetColumnSpan(this.pluginGrid, 2);
            this.pluginGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pluginGrid.Location = new System.Drawing.Point(3, 3);
            this.pluginGrid.MultiSelect = false;
            this.pluginGrid.Name = "pluginGrid";
            this.pluginGrid.ReadOnly = true;
            this.pluginGrid.RowHeadersVisible = false;
            this.pluginGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.pluginGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.pluginGrid.ShowCellErrors = false;
            this.pluginGrid.ShowEditingIcon = false;
            this.pluginGrid.ShowRowErrors = false;
            this.pluginGrid.Size = new System.Drawing.Size(394, 354);
            this.pluginGrid.StandardTab = true;
            this.pluginGrid.TabIndex = 0;
            this.pluginGrid.SelectionChanged += new System.EventHandler(this.pluginGrid_SelectionChanged);
            // 
            // pluginIconColumn
            // 
            this.pluginIconColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.pluginIconColumn.DataPropertyName = "SmallIcon";
            this.pluginIconColumn.HeaderText = "";
            this.pluginIconColumn.Name = "pluginIconColumn";
            this.pluginIconColumn.ReadOnly = true;
            this.pluginIconColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.pluginIconColumn.ToolTipText = "Plugin icon";
            this.pluginIconColumn.Width = 20;
            // 
            // pluginNameColumn
            // 
            this.pluginNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.pluginNameColumn.DataPropertyName = "Name";
            this.pluginNameColumn.HeaderText = "Name";
            this.pluginNameColumn.Name = "pluginNameColumn";
            this.pluginNameColumn.ReadOnly = true;
            this.pluginNameColumn.ToolTipText = "Plugin name";
            // 
            // pluginVersionColumn
            // 
            this.pluginVersionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.pluginVersionColumn.DataPropertyName = "Version";
            this.pluginVersionColumn.FillWeight = 30F;
            this.pluginVersionColumn.HeaderText = "Version";
            this.pluginVersionColumn.Name = "pluginVersionColumn";
            this.pluginVersionColumn.ReadOnly = true;
            this.pluginVersionColumn.ToolTipText = "Plugin version";
            // 
            // outerTableLayoutPanel
            // 
            this.outerTableLayoutPanel.AutoSize = true;
            this.outerTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.outerTableLayoutPanel.ColumnCount = 2;
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outerTableLayoutPanel.Controls.Add(this.pluginGrid, 0, 0);
            this.outerTableLayoutPanel.Controls.Add(this.pluginIconPictureBox, 0, 1);
            this.outerTableLayoutPanel.Controls.Add(this.pluginDescriptionLabel, 1, 1);
            this.outerTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outerTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.outerTableLayoutPanel.Name = "outerTableLayoutPanel";
            this.outerTableLayoutPanel.RowCount = 2;
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.outerTableLayoutPanel.Size = new System.Drawing.Size(400, 400);
            this.outerTableLayoutPanel.TabIndex = 1;
            // 
            // pluginIconPictureBox
            // 
            this.pluginIconPictureBox.ErrorImage = null;
            this.pluginIconPictureBox.InitialImage = null;
            this.pluginIconPictureBox.Location = new System.Drawing.Point(4, 364);
            this.pluginIconPictureBox.Margin = new System.Windows.Forms.Padding(4);
            this.pluginIconPictureBox.Name = "pluginIconPictureBox";
            this.pluginIconPictureBox.Size = new System.Drawing.Size(32, 32);
            this.pluginIconPictureBox.TabIndex = 1;
            this.pluginIconPictureBox.TabStop = false;
            // 
            // pluginDescriptionLabel
            // 
            this.pluginDescriptionLabel.AutoEllipsis = true;
            this.pluginDescriptionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pluginDescriptionLabel.Location = new System.Drawing.Point(43, 360);
            this.pluginDescriptionLabel.Name = "pluginDescriptionLabel";
            this.pluginDescriptionLabel.Size = new System.Drawing.Size(354, 40);
            this.pluginDescriptionLabel.TabIndex = 2;
            this.pluginDescriptionLabel.Text = "about description";
            this.pluginDescriptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PluginControlPanelTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.outerTableLayoutPanel);
            this.Name = "PluginControlPanelTab";
            this.Size = new System.Drawing.Size(400, 400);
            ((System.ComponentModel.ISupportInitialize)(this.pluginGrid)).EndInit();
            this.outerTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pluginIconPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView pluginGrid;
        private System.Windows.Forms.TableLayoutPanel outerTableLayoutPanel;
        private System.Windows.Forms.Label pluginDescriptionLabel;
        private System.Windows.Forms.PictureBox pluginIconPictureBox;
        private System.Windows.Forms.DataGridViewImageColumn pluginIconColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pluginNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pluginVersionColumn;

    }
}
