using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using System.Reflection;

namespace MbUnit.Icarus.Plugins
{
    public delegate UserControl OptionsTreeNodeSelectedHandler();

    public class OptionsTreeNode : TreeNode 
    {
        private OptionsTreeNodeSelectedHandler onSelection;

        public OptionsTreeNode(string displayText)
        {
            this.Text = displayText;
        }

        public OptionsTreeNode(string displayText, OptionsTreeNodeSelectedHandler onSelection)
            : this(displayText)
        {
            this.onSelection = onSelection;
        }

        public UserControl OptionsPanel
        {
            get
            {
                if (onSelection != null)
                {
                    UserControl optionsPanel = onSelection();
                    optionsPanel.Dock = DockStyle.Fill;

                    return optionsPanel;
                }
                else
                    return null;
            }
        }
    }
}
