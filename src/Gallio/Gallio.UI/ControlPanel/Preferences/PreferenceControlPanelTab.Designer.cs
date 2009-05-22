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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferenceControlPanelTab));
            this.preferencePaneTree = new System.Windows.Forms.TreeView();
            this.preferencePaneIconImageList = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // preferencePaneTree
            // 
            this.preferencePaneTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.preferencePaneTree.ImageIndex = 0;
            this.preferencePaneTree.ImageList = this.preferencePaneIconImageList;
            this.preferencePaneTree.Location = new System.Drawing.Point(0, 0);
            this.preferencePaneTree.Name = "preferencePaneTree";
            this.preferencePaneTree.SelectedImageIndex = 0;
            this.preferencePaneTree.Size = new System.Drawing.Size(232, 500);
            this.preferencePaneTree.TabIndex = 0;
            this.preferencePaneTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.preferencePaneTree_AfterSelect);
            // 
            // preferencePaneIconImageList
            // 
            this.preferencePaneIconImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("preferencePaneIconImageList.ImageStream")));
            this.preferencePaneIconImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.preferencePaneIconImageList.Images.SetKeyName(0, "DefaultPreferencePaneIcon.ico");
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.preferencePaneTree);
            this.splitContainer1.Size = new System.Drawing.Size(696, 500);
            this.splitContainer1.SplitterDistance = 232;
            this.splitContainer1.TabIndex = 1;
            // 
            // PreferenceControlPanelTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "PreferenceControlPanelTab";
            this.Size = new System.Drawing.Size(696, 500);
            this.Load += new System.EventHandler(this.PreferenceControlPanelTab_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView preferencePaneTree;
        private System.Windows.Forms.ImageList preferencePaneIconImageList;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
