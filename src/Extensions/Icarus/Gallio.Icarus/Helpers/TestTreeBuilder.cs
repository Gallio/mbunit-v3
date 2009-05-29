// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Model.Serialization;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Helpers
{
    internal class TestTreeBuilder
    {
        public static TestTreeNode BuildTestTree(IProgressMonitor progressMonitor, TestModelData testModelData, 
            TestTreeBuilderOptions options)
        {
            var root = TestTreeNodeFactory.CreateNode(testModelData.RootTest);

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

            root.CheckState = CheckState.Checked;
            root.UpdateStateOfRelatedNodes();
            return root;
        }

        private static void PopulateNamespaceTree(IProgressMonitor progressMonitor, IList<TestData> list, 
            TestTreeNode parent, bool splitNamespaces)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TestData testData = list[i];

                TestTreeNode testTreeNode = TestTreeNodeFactory.CreateNode(testData);
                if (testTreeNode is FixtureNode)
                {
                    // fixtures need special treatment to insert the namespace layer!
                    testTreeNode = BuildNamespaceNode(parent, testData, testTreeNode, splitNamespaces);
                }
                else
                {
                    parent.Nodes.Add(testTreeNode);
                }

                // process child nodes
                PopulateNamespaceTree(progressMonitor, testData.Children, testTreeNode, splitNamespaces);

                progressMonitor.Worked(1);
            }
        }

        private static TestTreeNode BuildNamespaceNode(TestTreeNode parent, TestComponentData testComponentData,
            TestTreeNode fixtureNode, bool splitNamespaces)
        {
            string @namespace = testComponentData.CodeReference.NamespaceName ?? "";

            string[] namespaceArray = splitNamespaces ? @namespace.Split('.') 
                : new[] { @namespace };

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
                    nsNode = new NamespaceNode(ns, ns);
                    parent.Nodes.Add(nsNode);
                }
                parent = nsNode;
            }
            
            if (nsNode != null) 
                nsNode.Nodes.Add(fixtureNode);

            return fixtureNode;
        }

        private static void PopulateMetadataTree(IProgressMonitor progressMonitor, string metadataType, 
            IEnumerable<TestData> list, Node parent, TestTreeNode root)
        {
            foreach (var testData in list)
            {
                var testTreeNode = TestTreeNodeFactory.CreateNode(testData);

                if (testTreeNode is FixtureNode || testTreeNode is TestNode)
                {
                    var metadataList = testData.Metadata[metadataType];
                    if (metadataList.Count == 0)
                        metadataList = new List<string> { "None" };

                    foreach (string metadata in metadataList)
                    {
                        // find metadata node (or add if it doesn't exist)
                        TestTreeNode metadataNode;
                        List<TestTreeNode> nodes = root.Find(metadata, false);
                        if (nodes.Count > 0)
                        {
                            metadataNode = nodes[0];
                        }
                        else
                        {
                            metadataNode = new MetadataNode(metadata, metadataType);
                            root.Nodes.Add(metadataNode);
                        }

                        // add node in the appropriate place
                        if (testTreeNode is FixtureNode)
                        {
                            metadataNode.Nodes.Add(testTreeNode);
                            PopulateMetadataTree(progressMonitor, metadataType, testData.Children, 
                                testTreeNode, root);
                        }
                        else
                        {
                            if (metadata != "None")
                                metadataNode.Nodes.Add(testTreeNode);
                            else
                                parent.Nodes.Add(testTreeNode);
                        }

                        // add children, if there are any (fixtures)
                        foreach (var child in testData.Children)
                        {
                            var childNode = TestTreeNodeFactory.CreateNode(child);
                            testTreeNode.Nodes.Add(childNode);
                        }
                    }
                }
                else
                {
                    PopulateMetadataTree(progressMonitor, metadataType, testData.Children, parent, root);
                }

                progressMonitor.Worked(1);
            }
        }
    }
}
