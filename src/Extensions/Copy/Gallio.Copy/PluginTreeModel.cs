using System.Collections;
using Aga.Controls.Tree;
using Gallio.Runtime.Extensibility;

namespace Gallio.Copy
{
    internal class PluginTreeModel : TreeModelBase
    {
        private readonly Registry registry = new Registry();
        private readonly Node root;

        public PluginTreeModel(string pluginFolder)
        {
            var pluginLoader = new PluginLoader();
            pluginLoader.AddPluginPath(pluginFolder);

            var pluginCatalog = new PluginCatalog();
            pluginLoader.PopulateCatalog(pluginCatalog);

            pluginCatalog.ApplyTo(registry);

            root = new Node("Plugins");
        }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
            {
                yield return root;
            }
            else if (treePath.LastNode == root)
            {
                foreach (var pluginDescriptor in registry.Plugins)
                {
                    var pluginNode = new PluginNode(pluginDescriptor.PluginId);
                    foreach (var file in pluginDescriptor.FilePaths)
                    {
                        pluginNode.Nodes.Add(new FileNode(file));
                    }
                    yield return pluginNode;
                }
            }
            else if (treePath.LastNode is PluginNode)
            {
                var pluginNode = (PluginNode) treePath.LastNode;
                foreach (var child in pluginNode.Nodes)
                {
                    yield return child;
                }
            }
        }

        public override bool IsLeaf(TreePath treePath)
        {
            if (treePath.LastNode is FileNode)
                return true;

            return treePath.LastNode is PluginNode && ((PluginNode)treePath.LastNode).Nodes.Count == 0;
        }
    }
}
