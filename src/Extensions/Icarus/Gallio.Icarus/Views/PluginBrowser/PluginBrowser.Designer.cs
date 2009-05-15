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
            this.splitContainer1.Panel1.SuspendLayout();
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
            this.splitContainer1.Size = new System.Drawing.Size(729, 486);
            this.splitContainer1.SplitterDistance = 243;
            this.splitContainer1.TabIndex = 1;
            // 
            // PluginBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "PluginBrowser";
            this.Size = new System.Drawing.Size(729, 486);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Gallio.Icarus.Controls.PluginTreeView pluginBrowserTreeView;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
