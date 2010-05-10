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
using Aga.Controls.Tree;
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
                root, options);

            return root;
        }

        private static void PopulateNamespaceTree(IProgressMonitor progressMonitor, IList<TestData> list,
            TestTreeNode parent, TreeBuilderOptions options)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var testData = list[i];

                var testTreeNode = AddNode(testData, parent, 
                    options);

                // process child nodes
                PopulateNamespaceTree(progressMonitor, testData.Children, testTreeNode,
                    options);

                progressMonitor.Worked(1);
            }
        }

        private static TestTreeNode AddNode(TestData testData, TestTreeNode parent,
            TreeBuilderOptions options)
        {
            var testTreeNode = new TestDataNode(testData);
            
            if (FixtureNode(testData))
                parent = BuildNamespaceNode(testData, parent, options);
            
            parent.Nodes.Add(testTreeNode);
         
            return testTreeNode;
        }

        private static bool FixtureNode(TestComponentData testData)
        {
            return testData.CodeReference.MemberName == null && testData.CodeReference.NamespaceName != null;
        }

        private static TestTreeNode BuildNamespaceNode(TestComponentData testData, 
            TestTreeNode parent, TreeBuilderOptions options)
        {
            var @namespace = testData.CodeReference.NamespaceName;

            if (string.IsNullOrEmpty(@namespace))
                return parent;

            if (options.NamespaceHierarchy == NamespaceHierarchy.Flat)
            {
                parent = FindOrAddNamespaceNode(parent, @namespace, @namespace);
                return parent;
            }

            var namespaces = @namespace.Split('.');

            foreach (var namespaceSegment in namespaces)
            {
                if (namespaceSegment.Length == 0)
                    continue;

                parent = FindOrAddNamespaceNode(parent, @namespace, namespaceSegment);
            }

            return parent;
        }

        private static TestTreeNode FindOrAddNamespaceNode(TestTreeNode parent, string @namespace, string text)
        {
            var namespaceNode = FindNamespaceNode(parent, text) 
                ?? new NamespaceNode(@namespace, text);

            parent.Nodes.Add(namespaceNode);
            parent = namespaceNode;

            return parent;
        }

        private static TestTreeNode FindNamespaceNode(Node parent, string text)
        {
            foreach (var node in parent.Nodes)
            {
                var testTreeNode = node as TestTreeNode;
                if (testTreeNode == null)
                    continue;

                if (testTreeNode.Text == text)
                {
                    return testTreeNode;
                }

                var namespaceNode = FindNamespaceNode(testTreeNode, text);
                if (namespaceNode != null)
                    return namespaceNode;
            }
            
            return null;
        }
    }
}
