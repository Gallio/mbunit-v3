namespace Gallio.Icarus.Views
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
            this.SuspendLayout();
            // 
            // pluginBrowserTreeView
            // 
            this.pluginBrowserTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pluginBrowserTreeView.Location = new System.Drawing.Point(0, 0);
            this.pluginBrowserTreeView.Name = "pluginBrowserTreeView";
            this.pluginBrowserTreeView.Size = new System.Drawing.Size(150, 150);
            this.pluginBrowserTreeView.TabIndex = 0;
            // 
            // PluginBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pluginBrowserTreeView);
            this.Name = "PluginBrowser";
            this.ResumeLayout(false);

        }

        #endregion

        private Gallio.Icarus.Controls.PluginTreeView pluginBrowserTreeView;
    }
}
