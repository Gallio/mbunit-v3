namespace Gallio.Icarus.Views.PluginBrowser
{
    partial class PluginDetails
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
            this.pluginDetailsTreeView = new Aga.Controls.Tree.TreeViewAdv();
            this.treeColumn1 = new Aga.Controls.Tree.TreeColumn();
            this.treeColumn2 = new Aga.Controls.Tree.TreeColumn();
            this.nodeIcon1 = new Aga.Controls.Tree.NodeControls.NodeIcon();
            this.nodeTextBox1 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox2 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.SuspendLayout();
            // 
            // pluginDetailsTreeView
            // 
            this.pluginDetailsTreeView.BackColor = System.Drawing.SystemColors.Window;
            this.pluginDetailsTreeView.Columns.Add(this.treeColumn1);
            this.pluginDetailsTreeView.Columns.Add(this.treeColumn2);
            this.pluginDetailsTreeView.DefaultToolTipProvider = null;
            this.pluginDetailsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pluginDetailsTreeView.DragDropMarkColor = System.Drawing.Color.Black;
            this.pluginDetailsTreeView.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pluginDetailsTreeView.Location = new System.Drawing.Point(0, 0);
            this.pluginDetailsTreeView.Model = null;
            this.pluginDetailsTreeView.Name = "pluginDetailsTreeView";
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
            // PluginDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pluginDetailsTreeView);
            this.Name = "PluginDetails";
            this.Size = new System.Drawing.Size(622, 356);
            this.ResumeLayout(false);

        }

        #endregion

        private Aga.Controls.Tree.TreeViewAdv pluginDetailsTreeView;
        private Aga.Controls.Tree.TreeColumn treeColumn1;
        private Aga.Controls.Tree.TreeColumn treeColumn2;
        private Aga.Controls.Tree.NodeControls.NodeIcon nodeIcon1;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox1;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox2;
    }
}
