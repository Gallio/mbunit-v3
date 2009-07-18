namespace Gallio.Copy
{
    partial class CopyForm
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
            this.pluginIdColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.closeButton = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.taskNameStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.sourceGroupBox = new System.Windows.Forms.GroupBox();
            this.sourcePluginTreeView = new Gallio.Copy.PluginTreeView();
            this.selectSourcePluginFolderButton = new System.Windows.Forms.Button();
            this.sourcePluginFolderTextBox = new System.Windows.Forms.TextBox();
            this.targetGroupBox = new System.Windows.Forms.GroupBox();
            this.targetPluginFolderTextBox = new System.Windows.Forms.TextBox();
            this.targetPluginTreeView = new Gallio.Copy.PluginTreeView();
            this.selectTargetPluginFolderButton = new System.Windows.Forms.Button();
            this.selectAllButton = new System.Windows.Forms.Button();
            this.toolStripContainer.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.sourceGroupBox.SuspendLayout();
            this.targetGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // pluginIdColumnHeader
            // 
            this.pluginIdColumnHeader.Text = "Plugin Id";
            this.pluginIdColumnHeader.Width = 300;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(516, 341);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // copyButton
            // 
            this.copyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.copyButton.Location = new System.Drawing.Point(435, 341);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(75, 23);
            this.copyButton.TabIndex = 1;
            this.copyButton.Text = "Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.BottomToolStripPanel
            // 
            this.toolStripContainer.BottomToolStripPanel.Controls.Add(this.statusStrip);
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Controls.Add(this.splitContainer);
            this.toolStripContainer.ContentPanel.Controls.Add(this.closeButton);
            this.toolStripContainer.ContentPanel.Controls.Add(this.selectAllButton);
            this.toolStripContainer.ContentPanel.Controls.Add(this.copyButton);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(603, 376);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.Size = new System.Drawing.Size(603, 398);
            this.toolStripContainer.TabIndex = 2;
            this.toolStripContainer.Text = "toolStripContainer1";
            // 
            // statusStrip
            // 
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.taskNameStatusLabel,
            this.toolStripProgressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 0);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(603, 22);
            this.statusStrip.TabIndex = 2;
            // 
            // taskNameStatusLabel
            // 
            this.taskNameStatusLabel.Name = "taskNameStatusLabel";
            this.taskNameStatusLabel.Size = new System.Drawing.Size(486, 17);
            this.taskNameStatusLabel.Spring = true;
            this.taskNameStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(12, 12);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.sourceGroupBox);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.targetGroupBox);
            this.splitContainer.Size = new System.Drawing.Size(579, 318);
            this.splitContainer.SplitterDistance = 289;
            this.splitContainer.TabIndex = 2;
            // 
            // sourceGroupBox
            // 
            this.sourceGroupBox.Controls.Add(this.sourcePluginTreeView);
            this.sourceGroupBox.Controls.Add(this.selectSourcePluginFolderButton);
            this.sourceGroupBox.Controls.Add(this.sourcePluginFolderTextBox);
            this.sourceGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceGroupBox.Location = new System.Drawing.Point(0, 0);
            this.sourceGroupBox.Name = "sourceGroupBox";
            this.sourceGroupBox.Size = new System.Drawing.Size(289, 318);
            this.sourceGroupBox.TabIndex = 8;
            this.sourceGroupBox.TabStop = false;
            this.sourceGroupBox.Text = "Source";
            // 
            // sourcePluginTreeView
            // 
            this.sourcePluginTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sourcePluginTreeView.BackColor = System.Drawing.SystemColors.Window;
            this.sourcePluginTreeView.DefaultToolTipProvider = null;
            this.sourcePluginTreeView.DragDropMarkColor = System.Drawing.Color.Black;
            this.sourcePluginTreeView.FullRowSelect = true;
            this.sourcePluginTreeView.LineColor = System.Drawing.SystemColors.ControlDark;
            this.sourcePluginTreeView.Location = new System.Drawing.Point(10, 48);
            this.sourcePluginTreeView.Model = null;
            this.sourcePluginTreeView.Name = "sourcePluginTreeView";
            this.sourcePluginTreeView.SelectedNode = null;
            this.sourcePluginTreeView.Size = new System.Drawing.Size(268, 262);
            this.sourcePluginTreeView.TabIndex = 4;
            // 
            // selectSourcePluginFolderButton
            // 
            this.selectSourcePluginFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectSourcePluginFolderButton.Location = new System.Drawing.Point(249, 19);
            this.selectSourcePluginFolderButton.Name = "selectSourcePluginFolderButton";
            this.selectSourcePluginFolderButton.Size = new System.Drawing.Size(29, 23);
            this.selectSourcePluginFolderButton.TabIndex = 6;
            this.selectSourcePluginFolderButton.Text = "...";
            this.selectSourcePluginFolderButton.UseVisualStyleBackColor = true;
            // 
            // sourcePluginFolderTextBox
            // 
            this.sourcePluginFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sourcePluginFolderTextBox.Location = new System.Drawing.Point(10, 19);
            this.sourcePluginFolderTextBox.Name = "sourcePluginFolderTextBox";
            this.sourcePluginFolderTextBox.ReadOnly = true;
            this.sourcePluginFolderTextBox.Size = new System.Drawing.Size(229, 20);
            this.sourcePluginFolderTextBox.TabIndex = 5;
            // 
            // targetGroupBox
            // 
            this.targetGroupBox.Controls.Add(this.targetPluginFolderTextBox);
            this.targetGroupBox.Controls.Add(this.targetPluginTreeView);
            this.targetGroupBox.Controls.Add(this.selectTargetPluginFolderButton);
            this.targetGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.targetGroupBox.Location = new System.Drawing.Point(0, 0);
            this.targetGroupBox.Name = "targetGroupBox";
            this.targetGroupBox.Size = new System.Drawing.Size(286, 318);
            this.targetGroupBox.TabIndex = 11;
            this.targetGroupBox.TabStop = false;
            this.targetGroupBox.Text = "Target";
            // 
            // targetPluginFolderTextBox
            // 
            this.targetPluginFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.targetPluginFolderTextBox.Location = new System.Drawing.Point(10, 19);
            this.targetPluginFolderTextBox.Name = "targetPluginFolderTextBox";
            this.targetPluginFolderTextBox.ReadOnly = true;
            this.targetPluginFolderTextBox.Size = new System.Drawing.Size(229, 20);
            this.targetPluginFolderTextBox.TabIndex = 9;
            // 
            // targetPluginTreeView
            // 
            this.targetPluginTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.targetPluginTreeView.BackColor = System.Drawing.SystemColors.Window;
            this.targetPluginTreeView.DefaultToolTipProvider = null;
            this.targetPluginTreeView.DragDropMarkColor = System.Drawing.Color.Black;
            this.targetPluginTreeView.FullRowSelect = true;
            this.targetPluginTreeView.LineColor = System.Drawing.SystemColors.ControlDark;
            this.targetPluginTreeView.Location = new System.Drawing.Point(10, 48);
            this.targetPluginTreeView.Model = null;
            this.targetPluginTreeView.Name = "targetPluginTreeView";
            this.targetPluginTreeView.SelectedNode = null;
            this.targetPluginTreeView.Size = new System.Drawing.Size(264, 262);
            this.targetPluginTreeView.TabIndex = 8;
            // 
            // selectTargetPluginFolderButton
            // 
            this.selectTargetPluginFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectTargetPluginFolderButton.Location = new System.Drawing.Point(245, 19);
            this.selectTargetPluginFolderButton.Name = "selectTargetPluginFolderButton";
            this.selectTargetPluginFolderButton.Size = new System.Drawing.Size(29, 23);
            this.selectTargetPluginFolderButton.TabIndex = 10;
            this.selectTargetPluginFolderButton.Text = "...";
            this.selectTargetPluginFolderButton.UseVisualStyleBackColor = true;
            // 
            // selectAllButton
            // 
            this.selectAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.selectAllButton.Location = new System.Drawing.Point(12, 341);
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.Size = new System.Drawing.Size(75, 23);
            this.selectAllButton.TabIndex = 1;
            this.selectAllButton.Text = "Select All";
            this.selectAllButton.UseVisualStyleBackColor = true;
            // 
            // CopyForm
            // 
            this.AcceptButton = this.copyButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(603, 398);
            this.Controls.Add(this.toolStripContainer);
            this.Name = "CopyForm";
            this.Text = "Gallio Copy";
            this.toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.sourceGroupBox.ResumeLayout(false);
            this.sourceGroupBox.PerformLayout();
            this.targetGroupBox.ResumeLayout(false);
            this.targetGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.ColumnHeader pluginIdColumnHeader;
        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel taskNameStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.Button selectAllButton;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Button selectSourcePluginFolderButton;
        private System.Windows.Forms.TextBox sourcePluginFolderTextBox;
        private PluginTreeView sourcePluginTreeView;
        private System.Windows.Forms.Button selectTargetPluginFolderButton;
        private System.Windows.Forms.TextBox targetPluginFolderTextBox;
        private PluginTreeView targetPluginTreeView;
        private System.Windows.Forms.GroupBox sourceGroupBox;
        private System.Windows.Forms.GroupBox targetGroupBox;
    }
}

