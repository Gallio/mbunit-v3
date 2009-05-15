using System.Windows.Forms;
using Aga.Controls.Tree;

namespace Gallio.Icarus.Views.PluginBrowser
{
    public partial class PluginDetails : UserControl
    {
        public ITreeModel Model
        {
            get { return pluginDetailsTreeView.Model; }
            set { pluginDetailsTreeView.Model = value; }
        }

        public PluginDetails()
        {
            InitializeComponent();
        }
    }
}
