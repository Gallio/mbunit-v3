using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Gallio.Icarus.Interfaces;

using WeifenLuo.WinFormsUI.Docking;

namespace Gallio.Icarus
{
    public partial class AssemblyList : DockContent
    {
        private IProjectAdapterView projectAdapterView;

        public AssemblyList(IProjectAdapterView projectAdapterView)
        {
            this.projectAdapterView = projectAdapterView;
            InitializeComponent();
        }

        public void DataBind(ListViewItem[] assemblies)
        {
            assembliesListView.Items.Clear();
            assembliesListView.Items.AddRange(assemblies);
        }

        private void assembliesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeAssemblyToolStripMenuItem.Enabled = (assembliesListView.SelectedItems.Count > 0);
        }

        private void removeAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projectAdapterView != null)
                projectAdapterView.ThreadedRemoveAssembly(assembliesListView.SelectedItems[0].SubItems[2].Text);
        }
    }
}