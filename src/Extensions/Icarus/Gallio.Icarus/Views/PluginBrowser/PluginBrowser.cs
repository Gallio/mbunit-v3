using System.Windows.Forms;
using Gallio.Icarus.Models;
using Gallio.Runtime;
using Gallio.Icarus.Models.PluginNodes;
using Aga.Controls.Tree;

namespace Gallio.Icarus.Views.PluginBrowser
{
    public partial class PluginBrowser : UserControl
    {
        private readonly PluginTreeModel model;
        private readonly PluginDetails pluginDetails = new PluginDetails();

        public PluginBrowser()
        {
            InitializeComponent();

            model = new PluginTreeModel(RuntimeAccessor.Registry);
            pluginBrowserTreeView.Model = model;

            splitContainer1.Panel2.Controls.Add(pluginDetails);
            pluginDetails.Dock = DockStyle.Fill;
        }

        private void pluginBrowserTreeView_SelectionChanged(object sender, System.EventArgs e)
        {
            if (pluginBrowserTreeView.SelectedNode == null || pluginBrowserTreeView.SelectedNode.Tag == null)
                return;

            ITreeModel detailsModel = null;
            if (pluginBrowserTreeView.SelectedNode.Tag is PluginNode)
            {
                var node = (PluginNode)pluginBrowserTreeView.SelectedNode.Tag;
                var details = model.GetPluginDetails(node.Text);
                detailsModel = new PluginDetailsTreeModel(details);
            }
            else if (pluginBrowserTreeView.SelectedNode.Tag is ServiceNode)
            {
                var node = (ServiceNode)pluginBrowserTreeView.SelectedNode.Tag;
                var details = model.GetServiceDetails(node.Text);
                detailsModel = new ServiceDetailsTreeModel(details);
            }
            else if (pluginBrowserTreeView.SelectedNode.Tag is ComponentNode)
            {
                var node = (ComponentNode)pluginBrowserTreeView.SelectedNode.Tag;
                var details = model.GetComponentDetails(node.Text);
                detailsModel = new ComponentDetailsTreeModel(details);
            }
            pluginDetails.Model = detailsModel;
        }
    }
}
