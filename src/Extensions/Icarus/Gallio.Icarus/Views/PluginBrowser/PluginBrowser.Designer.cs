namespace Gallio.Icarus.Views.PluginBrowser
{
    partial class PluginBrowser
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
            this.pluginBrowserTreeView = new Gallio.Icarus.Controls.PluginTreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pluginDetailsTreeView = new Aga.Controls.Tree.TreeViewAdv();
            this.treeColumn1 = new Aga.Controls.Tree.TreeColumn();
            this.treeColumn2 = new Aga.Controls.Tree.TreeColumn();
            this.nodeIcon1 = new Aga.Controls.Tree.NodeControls.NodeIcon();
            this.nodeTextBox1 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox2 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pluginBrowserTreeView
            // 
            this.pluginBrowserTreeView.BackColor = System.Drawing.SystemColors.Window;
            this.pluginBrowserTreeView.DefaultToolTipProvider = null;
            this.pluginBrowserTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pluginBrowserTreeView.DragDropMarkColor = System.Drawing.Color.Black;
            this.pluginBrowserTreeView.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pluginBrowserTreeView.Location = new System.Drawing.Point(0, 0);
            this.pluginBrowserTreeView.Model = null;
            this.pluginBrowserTreeView.Name = "pluginBrowserTreeView";
            this.pluginBrowserTreeView.SelectedNode = null;
            this.pluginBrowserTreeView.Size = new System.Drawing.Size(243, 486);
            this.pluginBrowserTreeView.TabIndex = 0;
            this.pluginBrowserTreeView.SelectionChanged += new System.EventHandler(this.pluginBrowserTreeView_SelectionChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pluginBrowserTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pluginDetailsTreeView);
            this.splitContainer1.Size = new System.Drawing.Size(729, 486);
            this.splitContainer1.SplitterDistance = 243;
            this.splitContainer1.TabIndex = 1;
            // 
            // pluginDetailsTreeView
            // 
            this.pluginDetailsTreeView.BackColor = System.Drawing.SystemColors.Window;
            this.pluginDetailsTreeView.DefaultToolTipProvider = null;
            this.pluginDetailsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pluginDetailsTreeView.DragDropMarkColor = System.Drawing.Color.Black;
            this.pluginDetailsTreeView.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pluginDetailsTreeView.Location = new System.Drawing.Point(0, 0);
            this.pluginDetailsTreeView.Model = null;
            this.pluginDetailsTreeView.Name = "pluginDetailsTreeView";
            this.pluginDetailsTreeView.SelectedNode = null;
            this.pluginDetailsTreeView.Size = new System.Drawing.Size(482, 486);
            this.pluginDetailsTreeView.TabIndex = 1;
            this.pluginDetailsTreeView.UseColumns = true;
            this.pluginDetailsTreeView.NodeControls.Add(this.nodeIcon1);
            this.pluginDetailsTreeView.NodeControls.Add(this.nodeTextBox1);
            this.pluginDetailsTreeView.NodeControls.Add(this.nodeTextBox2);
            this.pluginDetailsTreeView.SelectedNode = null;
            this.pluginDetailsTreeView.Size = new System.Drawing.Size(622, 356);
            this.pluginDetailsTreeView.TabIndex = 0;
            this.pluginDetailsTreeView.UseColumns = true;
            // 
            // treeColumn1
            // 
            this.treeColumn1.Header = "";
            this.treeColumn1.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn1.TooltipText = null;
            this.treeColumn1.Width = 200;
            // 
            // treeColumn2
            // 
            this.treeColumn2.Header = "";
            this.treeColumn2.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn2.TooltipText = null;
            this.treeColumn2.Width = 300;
            // 
            // nodeIcon1
            // 
            this.nodeIcon1.LeftMargin = 1;
            this.nodeIcon1.ParentColumn = this.treeColumn1;
            // 
            // nodeTextBox1
            // 
            this.nodeTextBox1.DataPropertyName = "Name";
            this.nodeTextBox1.EditEnabled = false;
            this.nodeTextBox1.IncrementalSearchEnabled = true;
            this.nodeTextBox1.LeftMargin = 3;
            this.nodeTextBox1.ParentColumn = this.treeColumn1;
            // 
            // nodeTextBox2
            // 
            this.nodeTextBox2.DataPropertyName = "Value";
            this.nodeTextBox2.EditEnabled = false;
            this.nodeTextBox2.IncrementalSearchEnabled = true;
            this.nodeTextBox2.LeftMargin = 3;
            this.nodeTextBox2.ParentColumn = this.treeColumn2;
            // 
            // PluginBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "PluginBrowser";
            this.Size = new System.Drawing.Size(729, 486);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Gallio.Icarus.Controls.PluginTreeView pluginBrowserTreeView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Aga.Controls.Tree.TreeViewAdv pluginDetailsTreeView;
        private Aga.Controls.Tree.TreeColumn treeColumn1;
        private Aga.Controls.Tree.TreeColumn treeColumn2;
        private Aga.Controls.Tree.NodeControls.NodeIcon nodeIcon1;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox1;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox2;
    }
}
