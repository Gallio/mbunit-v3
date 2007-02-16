using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using MbUnit.Icarus.Plugins;

namespace MbUnit.Icarus
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            // Go through all the plugins and get the options menu items.
            foreach (AvailablePlugin plugin in Program.Plugins.AvailablePlugins)
            {
                TreeNode node = new TreeNode(plugin.Instance.Name);
                optionCategoryTree.Nodes.Add(PluginOptionsMenu(node, plugin.Instance.OptionsMenu));
            }

            optionCategoryTree.ExpandAll();
        }

        private TreeNode PluginOptionsMenu(TreeNode node, string[] menuItems)
        {
            foreach (string item in menuItems)
            {
                node.Nodes.Add(item);
            }

            return node;
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
    }
}