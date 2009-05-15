using Gallio.Model.Serialization;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;
using Gallio.Icarus.Models;
using System.Collections.Generic;
using Gallio.Common.Reflection;

namespace Gallio.Icarus.Helpers
{
    internal class TestTreeBuilder
    {
        public static TestTreeNode BuildTestTree(IProgressMonitor progressMonitor, TestModelData testModelData, 
            TestTreeBuilderOptions options)
        {
            var root = new TestTreeNode(testModelData.RootTest.Name, testModelData.RootTest.Id, 
                TestKinds.Root);

            progressMonitor.Worked(1);

            if (options.TreeViewCategory == "Namespace")
            {
                PopulateNamespaceTree(progressMonitor, testModelData.RootTest.Children, 
                    root, options.SplitNamespaces);
            }
            else
            {
                PopulateMetadataTree(progressMonitor, options.TreeViewCategory, 
                    testModelData.RootTest.Children, root, root);
            }

            return root;
        }

        private static void PopulateNamespaceTree(IProgressMonitor progressMonitor, IList<TestData> list, 
            TestTreeNode parent, bool splitNamespaces)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TestData td = list[i];
                string componentKind = td.Metadata.GetValue(MetadataKeys.TestKind);
                if (componentKind == null)
                    continue;
                TestTreeNode ttnode;
                if (componentKind != TestKinds.Fixture)
                {
                    // create an appropriate node
                    ttnode = new TestTreeNode(td.Name, td.Id, componentKind);
                    parent.Nodes.Add(ttnode);
                }
                else
                {
                    // fixtures need special treatment to insert the namespace layer!
                    ttnode = BuildNamespaceNode(parent, td, componentKind, splitNamespaces);
                }
                ttnode.SourceCodeAvailable = (td.CodeLocation != CodeLocation.Unknown);
                ttnode.IsTest = td.IsTestCase;

                // process child nodes
                PopulateNamespaceTree(progressMonitor, td.Children, ttnode, splitNamespaces);

                progressMonitor.Worked(1);
            }
        }

        private static TestTreeNode BuildNamespaceNode(TestTreeNode parent, TestData td, string componentKind, 
            bool splitNamespaces)
        {
            TestTreeNode ttnode;
            string @namespace = td.CodeReference.NamespaceName ?? "";

            string[] namespaceArray;
            if (splitNamespaces)
            {
                // we want a namespace node for each segment
                // of the namespace
                namespaceArray = @namespace.Split('.');
            }
            else
            {
                namespaceArray = new[] { @namespace };
            }

            TestTreeNode nsNode = null;
            foreach (string ns in namespaceArray)
            {
                // find the namespace node (or add if it doesn't exist)
                List<TestTreeNode> nodes = parent.Find(ns, true);
                if (nodes.Count > 0)
                {
                    nsNode = nodes[0];
                }
                else
                {
                    nsNode = new TestTreeNode(ns, ns, "Namespace");
                    parent.Nodes.Add(nsNode);
                }
                parent = nsNode;
            }

            // add the fixture to the namespace
            ttnode = new TestTreeNode(td.Name, td.Id, componentKind);
            nsNode.Nodes.Add(ttnode);
            return ttnode;
        }

        private static void PopulateMetadataTree(IProgressMonitor progressMonitor, string key,
            IList<TestData> list, TestTreeNode parent, TestTreeNode root)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TestData td = list[i];
                string componentKind = td.Metadata.GetValue(MetadataKeys.TestKind);
                if (componentKind == null)
                    continue;
                switch (componentKind)
                {
                    case TestKinds.Fixture:
                    case TestKinds.Test:
                        IList<string> metadata = td.Metadata[key];
                        if (metadata.Count == 0)
                            metadata = new List<string> { "None" };

                        foreach (string m in metadata)
                        {
                            // find metadata node (or add if it doesn't exist)
                            TestTreeNode metadataNode;
                            List<TestTreeNode> nodes = root.Find(m, false);
                            if (nodes.Count > 0)
                                metadataNode = nodes[0];
                            else
                            {
                                metadataNode = new TestTreeNode(m, m, key);
                                root.Nodes.Add(metadataNode);
                            }

                            // add node in the appropriate place
                            if (componentKind == TestKinds.Fixture)
                            {
                                TestTreeNode ttnode = new TestTreeNode(td.Name, td.Id, componentKind);
                                metadataNode.Nodes.Add(ttnode);
                                PopulateMetadataTree(progressMonitor, key, td.Children, ttnode, root);
                            }
                            else
                            {
                                // test
                                TestTreeNode ttnode = new TestTreeNode(td.Name, td.Id, componentKind)
                                {
                                    SourceCodeAvailable =
                                        (td.CodeLocation != CodeLocation.Unknown),
                                    IsTest = td.IsTestCase
                                };
                                if (m != "None")
                                    metadataNode.Nodes.Add(ttnode);
                                else
                                    parent.Nodes.Add(ttnode);
                            }
                        }
                        break;
                }
                if (componentKind != TestKinds.Fixture)
                    PopulateMetadataTree(progressMonitor, key, td.Children, parent, root);

                progressMonitor.Worked(1);
            }
        }
    }
}
