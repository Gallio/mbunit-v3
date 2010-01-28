// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
                var testData = list[i];

                var testTreeNode = AddNode(testData, parent, 
                    namespaceHierarchy, rootNode);

                // process child nodes
                PopulateNamespaceTree(progressMonitor, testData.Children, testTreeNode,
                    namespaceHierarchy, rootNode);

                progressMonitor.Worked(1);
            }
        }

        private static TestTreeNode AddNode(TestData testData, TestTreeNode parent, 
            NamespaceHierarchy namespaceHierarchy, TestTreeNode rootNode)
        {
            TestTreeNode testTreeNode = new TestDataNode(testData);
            if (FixtureNode(testData))
            {
                // fixtures need special treatment to insert the namespace layer!
                var namespaces = GetNamespaceArray(testData, namespaceHierarchy);

                testTreeNode = BuildNamespaceNode(parent, testTreeNode, 
                    rootNode, namespaces);
            }
            else
            {
                parent.Nodes.Add(testTreeNode);
            }
            return testTreeNode;
        }

        private static bool FixtureNode(TestComponentData testData)
        {
            return testData.CodeReference.MemberName == null && testData.CodeReference.NamespaceName != null;
        }

        private static TestTreeNode BuildNamespaceNode(TestTreeNode namespaceNode, TestTreeNode fixtureNode, 
            TestTreeNode rootNode, IEnumerable<string> namespaces)
        {
            foreach (var @namespace in namespaces)
            {
                if (@namespace.Length == 0)
                    continue;

                namespaceNode = FindNamespaceNode(@namespace, rootNode, 
                    namespaceNode);
            }

            namespaceNode.Nodes.Add(fixtureNode);
            return fixtureNode;
        }

        private static TestTreeNode FindNamespaceNode(string @namespace, 
            TestTreeNode rootNode, TestTreeNode parent)
        {
            // find the namespace node (or add if it doesn't exist)
            var nodes = rootNode.Find(@namespace, true);

            if (nodes.Count > 0)
            {
                parent = nodes[0];
            }
            else
            {
                var namespaceNode = new NamespaceNode(@namespace);
                parent.Nodes.Add(namespaceNode);
                parent = namespaceNode;
            }
            
            return parent;
        }

        private static IEnumerable<string> GetNamespaceArray(TestComponentData testComponentData, 
            NamespaceHierarchy namespaceHierarchy)
        {
            var @namespace = testComponentData.CodeReference.NamespaceName;

            return namespaceHierarchy == NamespaceHierarchy.Tree ? @namespace.Split('.')
                       : new[] { @namespace };
        }
    }
}
