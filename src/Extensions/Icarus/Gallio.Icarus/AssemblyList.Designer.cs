// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Gallio.Icarus
{
    partial class AssemblyList
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssemblyList));
            this.assembliesListView = new System.Windows.Forms.ListView();
            this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader11 = new System.Windows.Forms.ColumnHeader();
            this.assemblyListMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeAssemblyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assemblyListMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // assembliesListView
            // 
            this.assembliesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11});
            this.assembliesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assembliesListView.FullRowSelect = true;
            this.assembliesListView.Location = new System.Drawing.Point(0, 0);
            this.assembliesListView.MultiSelect = false;
            this.assembliesListView.Name = "assembliesListView";
            this.assembliesListView.Size = new System.Drawing.Size(292, 273);
            this.assembliesListView.TabIndex = 1;
            this.assembliesListView.UseCompatibleStateImageBehavior = false;
            this.assembliesListView.View = System.Windows.Forms.View.Details;
            this.assembliesListView.SelectedIndexChanged += new System.EventHandler(this.assembliesListView_SelectedIndexChanged);
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Name";
            this.columnHeader9.Width = 99;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Version";
            this.columnHeader10.Width = 70;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Location";
            this.columnHeader11.Width = 128;
            // 
            // assemblyListMenuStrip
            // 
            this.assemblyListMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeAssemblyToolStripMenuItem});
            this.assemblyListMenuStrip.Name = "assemblyListMenuStrip";
            this.assemblyListMenuStrip.Size = new System.Drawing.Size(173, 48);
            // 
            // removeAssemblyToolStripMenuItem
            // 
            this.removeAssemblyToolStripMenuItem.Enabled = false;
            this.removeAssemblyToolStripMenuItem.Name = "removeAssemblyToolStripMenuItem";
            this.removeAssemblyToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.removeAssemblyToolStripMenuItem.Text = "Remove Assembly";
            this.removeAssemblyToolStripMenuItem.Click += new System.EventHandler(this.removeAssemblyToolStripMenuItem_Click);
            // 
            // AssemblyList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.assembliesListView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AssemblyList";
            this.TabText = "Assemblies";
            this.Text = "Assemblies";
            this.assemblyListMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView assembliesListView;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ContextMenuStrip assemblyListMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem removeAssemblyToolStripMenuItem;
    }
}