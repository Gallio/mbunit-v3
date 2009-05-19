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
            this.preferencePanePanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // preferencePaneTree
            // 
            this.preferencePaneTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.preferencePaneTree.ImageIndex = 0;
            this.preferencePaneTree.ImageList = this.preferencePaneIconImageList;
            this.preferencePaneTree.Location = new System.Drawing.Point(3, 3);
            this.preferencePaneTree.Name = "preferencePaneTree";
            this.preferencePaneTree.SelectedImageIndex = 0;
            this.preferencePaneTree.Size = new System.Drawing.Size(144, 344);
            this.preferencePaneTree.TabIndex = 0;
            this.preferencePaneTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.preferencePaneTree_AfterSelect);
            // 
            // preferencePaneIconImageList
            // 
            this.preferencePaneIconImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("preferencePaneIconImageList.ImageStream")));
            this.preferencePaneIconImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.preferencePaneIconImageList.Images.SetKeyName(0, "DefaultPreferencePaneIcon.ico");
            // 
            // preferencePanePanel
            // 
            this.preferencePanePanel.AutoSize = true;
            this.preferencePanePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.preferencePanePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.preferencePanePanel.Location = new System.Drawing.Point(153, 3);
            this.preferencePanePanel.Name = "preferencePanePanel";
            this.preferencePanePanel.Size = new System.Drawing.Size(194, 344);
            this.preferencePanePanel.TabIndex = 0;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.preferencePanePanel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.preferencePaneTree, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(350, 350);
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // PreferenceControlPanelTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "PreferenceControlPanelTab";
            this.Size = new System.Drawing.Size(350, 350);
            this.Load += new System.EventHandler(this.PreferenceControlPanelTab_Load);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView preferencePaneTree;
        private System.Windows.Forms.Panel preferencePanePanel;
        private System.Windows.Forms.ImageList preferencePaneIconImageList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    }
}
