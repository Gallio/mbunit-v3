namespace Gallio.UI.ControlPanel.Preferences
{
    partial class PreferenceControlPanelTab
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
            this.outerSplitContainer = new System.Windows.Forms.SplitContainer();
            this.preferencePaneTree = new System.Windows.Forms.TreeView();
            this.preferencePanePanel = new System.Windows.Forms.Panel();
            this.outerSplitContainer.Panel1.SuspendLayout();
            this.outerSplitContainer.Panel2.SuspendLayout();
            this.outerSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // outerSplitContainer
            // 
            this.outerSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outerSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.outerSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.outerSplitContainer.Name = "outerSplitContainer";
            // 
            // outerSplitContainer.Panel1
            // 
            this.outerSplitContainer.Panel1.Controls.Add(this.preferencePaneTree);
            this.outerSplitContainer.Panel1MinSize = 100;
            // 
            // outerSplitContainer.Panel2
            // 
            this.outerSplitContainer.Panel2.Controls.Add(this.preferencePanePanel);
            this.outerSplitContainer.Panel2MinSize = 100;
            this.outerSplitContainer.Size = new System.Drawing.Size(150, 150);
            this.outerSplitContainer.SplitterDistance = 46;
            this.outerSplitContainer.TabIndex = 0;
            // 
            // preferencePaneTree
            // 
            this.preferencePaneTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.preferencePaneTree.Location = new System.Drawing.Point(0, 0);
            this.preferencePaneTree.Name = "preferencePaneTree";
            this.preferencePaneTree.Size = new System.Drawing.Size(46, 150);
            this.preferencePaneTree.TabIndex = 0;
            this.preferencePaneTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.preferencePaneTree_AfterSelect);
            // 
            // preferencePanePanel
            // 
            this.preferencePanePanel.AutoSize = true;
            this.preferencePanePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.preferencePanePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.preferencePanePanel.Location = new System.Drawing.Point(0, 0);
            this.preferencePanePanel.Name = "preferencePanePanel";
            this.preferencePanePanel.Size = new System.Drawing.Size(100, 150);
            this.preferencePanePanel.TabIndex = 0;
            // 
            // PreferenceControlPanelTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.outerSplitContainer);
            this.Name = "PreferenceControlPanelTab";
            this.Load += new System.EventHandler(this.PreferenceControlPanelTab_Load);
            this.outerSplitContainer.Panel1.ResumeLayout(false);
            this.outerSplitContainer.Panel2.ResumeLayout(false);
            this.outerSplitContainer.Panel2.PerformLayout();
            this.outerSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer outerSplitContainer;
        private System.Windows.Forms.TreeView preferencePaneTree;
        private System.Windows.Forms.Panel preferencePanePanel;
    }
}
