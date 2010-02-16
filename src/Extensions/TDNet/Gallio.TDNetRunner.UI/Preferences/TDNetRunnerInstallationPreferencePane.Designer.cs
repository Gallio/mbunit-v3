namespace Gallio.TDNetRunner.UI.Preferences
{
    partial class TDNetRunnerInstallationPreferencePane
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
            this.label1 = new System.Windows.Forms.Label();
            this.frameworkGridView = new System.Windows.Forms.DataGridView();
            this.FrameworkNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FrameworkModeColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.outerTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.frameworkGridView)).BeginInit();
            this.outerTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(258, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Specify the installation mode for Gallio supported test frameworks in TestDriven." +
                "Net:";
            // 
            // frameworkGridView
            // 
            this.frameworkGridView.AllowUserToAddRows = false;
            this.frameworkGridView.AllowUserToDeleteRows = false;
            this.frameworkGridView.AllowUserToResizeRows = false;
            this.frameworkGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.frameworkGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FrameworkNameColumn,
            this.FrameworkModeColumn});
            this.frameworkGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frameworkGridView.Location = new System.Drawing.Point(23, 29);
            this.frameworkGridView.Margin = new System.Windows.Forms.Padding(23, 3, 3, 3);
            this.frameworkGridView.MultiSelect = false;
            this.frameworkGridView.Name = "frameworkGridView";
            this.frameworkGridView.RowHeadersVisible = false;
            this.frameworkGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.frameworkGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.frameworkGridView.ShowCellErrors = false;
            this.frameworkGridView.ShowEditingIcon = false;
            this.frameworkGridView.ShowRowErrors = false;
            this.frameworkGridView.Size = new System.Drawing.Size(274, 288);
            this.frameworkGridView.TabIndex = 1;
            this.frameworkGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.frameworkGridView_CellValueChanged);
            // 
            // FrameworkNameColumn
            // 
            this.FrameworkNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FrameworkNameColumn.HeaderText = "Test Framework";
            this.FrameworkNameColumn.Name = "FrameworkNameColumn";
            // 
            // FrameworkModeColumn
            // 
            this.FrameworkModeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FrameworkModeColumn.FillWeight = 50F;
            this.FrameworkModeColumn.HeaderText = "Mode";
            this.FrameworkModeColumn.Items.AddRange(new object[] {
            "Default",
            "Preferred",
            "Disabled"});
            this.FrameworkModeColumn.Name = "FrameworkModeColumn";
            this.FrameworkModeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.FrameworkModeColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // outerTableLayoutPanel
            // 
            this.outerTableLayoutPanel.ColumnCount = 1;
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outerTableLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.outerTableLayoutPanel.Controls.Add(this.frameworkGridView, 0, 1);
            this.outerTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outerTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.outerTableLayoutPanel.Name = "outerTableLayoutPanel";
            this.outerTableLayoutPanel.RowCount = 2;
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outerTableLayoutPanel.Size = new System.Drawing.Size(300, 320);
            this.outerTableLayoutPanel.TabIndex = 2;
            // 
            // TDNetRunnerInstallationPreferencePane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.outerTableLayoutPanel);
            this.Name = "TDNetRunnerInstallationPreferencePane";
            this.Load += new System.EventHandler(this.TDNetPreferencePane_Load);
            ((System.ComponentModel.ISupportInitialize)(this.frameworkGridView)).EndInit();
            this.outerTableLayoutPanel.ResumeLayout(false);
            this.outerTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView frameworkGridView;
        private System.Windows.Forms.TableLayoutPanel outerTableLayoutPanel;
        private System.Windows.Forms.DataGridViewTextBoxColumn FrameworkNameColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn FrameworkModeColumn;

    }
}
