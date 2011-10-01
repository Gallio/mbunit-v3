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

using Aga.Controls.Tree;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Model.Schema;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.TreeBuilders
{
    internal class MetadataTreeBuilder : ITreeBuilder
    {
        public bool CanHandle(string treeViewCategory)
        {
            return treeViewCategory != "Namespace";
        }

        public TestTreeNode BuildTree(IProgressMonitor progressMonitor, 
            TestModelData testModelData, TreeBuilderOptions options)
        {
            var root = new TestDataNode(testModelData.RootTest);

            progressMonitor.Worked(1);

            foreach (var childTestData in testModelData.RootTest.Children)
            {
                PopulateMetadataTree(progressMonitor, options.TreeViewCategory, 
                    childTestData, null, root);
            }

            return root;
        }

        private static void PopulateMetadataTree(IProgressMonitor progressMonitor, string metadataType,
            TestData testData, Node parentNode, TestTreeNode rootNode)
        {
            TestDataNode testDataNode = AddMetadataNode(testData, metadataType, 
                rootNode, parentNode);

            foreach (var childTestData in testData.Children)
            {
                PopulateMetadataTree(progressMonitor, metadataType, childTestData,
                    testDataNode, rootNode);
            }

            progressMonitor.Worked(1);
        }

        private static TestDataNode AddMetadataNode(TestData testData, string metadataType, 
            TestTreeNode rootNode, Node parentNode)
        {
            var testDataNode = new TestDataNode(testData);
            
            var metadataList = testData.Metadata[metadataType];
            if (metadataList.Count > 0)
            {
                foreach (string metadata in metadataList)
                {
                    var metadataNode = FindOrAddMetadataNode(metadataType, rootNode, metadata);
                    metadataNode.Nodes.Add(testDataNode);
                    testDataNode = new TestDataNode(testData); // can't re-use node
                }
            }
            else if (parentNode != null)
            {
                parentNode.Nodes.Add(testDataNode);
            }
            else
            {
                var metadataNode = FindOrAddMetadataNode(metadataType, rootNode, "None");
                metadataNode.Nodes.Add(testDataNode);
            }
            return testDataNode;
        }

        private static TestTreeNode FindOrAddMetadataNode(string metadataType, 
            TestTreeNode rootTestDataNode, string metadata)
        {
            TestTreeNode metadataNode;
            var nodes = rootTestDataNode.Find(metadata, false);
            if (nodes.Count > 0)
            {
                metadataNode = nodes[0];
            }
            else
            {
                metadataNode = new MetadataNode(metadata, metadataType);
                rootTestDataNode.Nodes.Add(metadataNode);
            }
            return metadataNode;
        }
    }
}