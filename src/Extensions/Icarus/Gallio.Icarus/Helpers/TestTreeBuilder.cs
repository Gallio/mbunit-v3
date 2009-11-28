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
using Aga.Controls.Tree;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Icarus.TreeBuilders;
using Gallio.Model.Schema;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Helpers
{
    internal class TestTreeBuilder
    {
        public static TestTreeNode BuildTestTree(IProgressMonitor progressMonitor, 
            TestModelData testModelData, TreeBuilderOptions options)
        {
            var root = new TestDataNode(testModelData.RootTest);

            progressMonitor.Worked(1);

            if (options.TreeViewCategory == "Namespace")
            {
                PopulateNamespaceTree(progressMonitor, testModelData.RootTest.Children, 
                    root, options.NamespaceHierarchy, root);
            }
            else
            {
                var metadataTreeBuilder = new MetadataTreeBuilder();
                return metadataTreeBuilder.BuildTree(progressMonitor, testModelData, options);
            }

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

        private static void PopulateMetadataTree(IProgressMonitor progressMonitor, string metadataType, Node parentTestDataNode, 
            TestData parentTestData, TestTreeNode rootTestDataNode)
        {
            foreach (TestData childTestData in parentTestData.Children)
            {
                var metadataList = childTestData.Metadata[metadataType];
                if (metadataList.Count == 0)
                    metadataList = new[] { "None" };

                foreach (string metadata in metadataList)
                {
                    // if already within the right metadata node, move on
                    if (parentTestData.Metadata[metadataType].Contains(metadata)
                        || metadata == "None" && parentTestDataNode != rootTestDataNode)
                        continue;

                    // find appropriate metadata node (or add if it doesn't exist)
                    TestTreeNode metadataNode;
                    List<TestTreeNode> nodes = rootTestDataNode.Find(metadata, false);
                    if (nodes.Count > 0)
                    {
                        metadataNode = nodes[0];
                    }
                    else
                    {
                        metadataNode = new MetadataNode(metadata, metadataType);
                        rootTestDataNode.Nodes.Add(metadataNode);
                    }

                    // add node
                    var indirectChildTestDataNode = new TestDataNode(childTestData);
                    metadataNode.Nodes.Add(indirectChildTestDataNode);
                    PopulateMetadataTree(progressMonitor, metadataType, indirectChildTestDataNode, childTestData, rootTestDataNode);
                }

                if (parentTestDataNode != rootTestDataNode)
                {
                    var directChildTestDataNode = new TestDataNode(childTestData);
                    parentTestDataNode.Nodes.Add(directChildTestDataNode);
                    PopulateMetadataTree(progressMonitor, metadataType, directChildTestDataNode, childTestData, rootTestDataNode);
                }

                progressMonitor.Worked(1);
            }
        }
    }
}
