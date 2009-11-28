using System.Collections.Generic;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Model.Schema;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.TreeBuilders
{
    public class NamespaceTreeBuilder : ITreeBuilder
    {
        public bool CanHandle(string treeViewCategory)
        {
            return treeViewCategory == "Namespace";
        }

        public TestTreeNode BuildTree(IProgressMonitor progressMonitor, TestModelData testModelData,
            TreeBuilderOptions options)
        {
            var root = new TestDataNode(testModelData.RootTest);

            progressMonitor.Worked(1);

            PopulateNamespaceTree(progressMonitor, testModelData.RootTest.Children, 
                root, options.NamespaceHierarchy, root);

            return root;
        }

        private static void PopulateNamespaceTree(IProgressMonitor progressMonitor, IList<TestData> list,
            TestTreeNode parent, NamespaceHierarchy namespaceHierarchy, TestTreeNode rootNode)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TestData testData = list[i];

                TestTreeNode testTreeNode = new TestDataNode(testData);
                if (testData.CodeReference.MemberName == null && testData.CodeReference.NamespaceName != null)
                {
                    // fixtures need special treatment to insert the namespace layer!
                    testTreeNode = BuildNamespaceNode(parent, testData,
                        testTreeNode, namespaceHierarchy, rootNode);
                }
                else
                {
                    parent.Nodes.Add(testTreeNode);
                }

                // process child nodes
                PopulateNamespaceTree(progressMonitor, testData.Children, testTreeNode,
                    namespaceHierarchy, rootNode);

                progressMonitor.Worked(1);
            }
        }

        private static TestTreeNode BuildNamespaceNode(TestTreeNode parent, TestComponentData testComponentData,
            TestTreeNode fixtureNode, NamespaceHierarchy namespaceHierarchy, TestTreeNode rootNode)
        {
            string @namespace = testComponentData.CodeReference.NamespaceName;

            string[] namespaceArray = namespaceHierarchy == NamespaceHierarchy.Tree ? @namespace.Split('.')
                : new[] { @namespace };

            foreach (string ns in namespaceArray)
            {
                if (ns.Length == 0)
                    continue;

                // find the namespace node (or add if it doesn't exist)
                var nodes = rootNode.Find(ns, true);
                if (nodes.Count > 0)
                {
                    parent = nodes[0];
                }
                else
                {
                    var nsNode = new NamespaceNode(ns, ns);
                    parent.Nodes.Add(nsNode);
                    parent = nsNode;
                }
            }

            parent.Nodes.Add(fixtureNode);
            return fixtureNode;
        }
    }
}
