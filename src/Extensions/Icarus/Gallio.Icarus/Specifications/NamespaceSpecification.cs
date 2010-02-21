using Gallio.Common.Reflection;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;

namespace Gallio.Icarus.Specifications
{
    public class NamespaceSpecification : Specification<TestTreeNode>
    {
        private readonly string @namespace;

        public NamespaceSpecification(string @namespace)
        {
            this.@namespace = @namespace;
        }

        public string MatchText
        {
            get { return @namespace; }
        }

        public override bool Matches(TestTreeNode item)
        {
            var node = item as TestDataNode;

            if (node == null)
                return false;

            return Matches(node);
        }

        private bool Matches(TestDataNode node)
        {
            if (node.CodeReference == CodeReference.Unknown)
                return false;

            var namespaceName = node.CodeReference.NamespaceName;

            if (namespaceName == null)
                return false;

            return CaseInsensitiveContains(namespaceName, 
                @namespace);
        }
    }
}
