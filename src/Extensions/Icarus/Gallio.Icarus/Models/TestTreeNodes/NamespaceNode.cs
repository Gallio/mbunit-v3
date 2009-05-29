using Gallio.Model;

namespace Gallio.Icarus.Models.TestTreeNodes
{
    internal class NamespaceNode : TestTreeNode
    {
        public NamespaceNode(string id, string name)
            : base(id, name)
        {
            NodeTypeIcon = Properties.Resources.Namespace;
            TestKind = TestKinds.Namespace;
        }
    }
}
