using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Reflection;

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
                if (plugin.Instance.OptionsMenu != null)
                    optionCategoryTree.Nodes.Add(plugin.Instance.OptionsMenu);
            }

            optionCategoryTree.ExpandAll();
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

        private void optionCategoryTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            panel2.Controls.Clear();

            if (e.Node is OptionsTreeNode)
            {
                OptionsTreeNode node = (OptionsTreeNode)e.Node;
                if (node.OptionsPanel != null)
                    panel2.Controls.Add(node.OptionsPanel);
                else
                {
                    foreach (OptionsTreeNode childNode in node.Nodes)
                    {
                        if (childNode.OptionsPanel != null)
                        {
                            panel2.Controls.Add(childNode.OptionsPanel);
                            break;
                        }
                    }
                }
            }
        }
    }
}