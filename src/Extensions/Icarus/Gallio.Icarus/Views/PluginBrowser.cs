using System.Windows.Forms;
using Gallio.Icarus.Models;

namespace Gallio.Icarus.Views
{
    public partial class PluginBrowser : UserControl
    {
        public PluginBrowser()
        {
            InitializeComponent();

            pluginBrowserTreeView.Model = new PluginTreeModel();
        }
    }
}
