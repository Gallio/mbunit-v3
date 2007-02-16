using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MbUnit.Icarus.Plugins
{
    public partial class PluginManager : Form
    {
        public PluginManager()
        {
            InitializeComponent();
        }

        private void PluginManager_Load(object sender, EventArgs e)
        {
            foreach (AvailablePlugin plugin in Program.Plugins.AvailablePlugins)
            {
                ListViewItem lvi = new ListViewItem(new string[] { plugin.Instance.Name, plugin.Instance.Version, plugin.Instance.Author });
                lvi.Tag = plugin;

                pluginList.Items.Add(lvi);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void pluginList_SelectedIndexChanged(object sender, EventArgs e)
        {
            descriptionTextBox.Text = "";

            if (pluginList.SelectedIndices.Count == 1)
            {
                AvailablePlugin plugin = (AvailablePlugin)pluginList.Items[pluginList.SelectedIndices[0]].Tag;
                descriptionTextBox.Text = plugin.Instance.Description;
            }
        }
    }
}