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

using System;
using System.Collections.Generic;
using Aga.Controls.Tree;
using Gallio.Icarus.Events;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Icarus.TreeBuilders;
using Gallio.Model.Schema;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.DataBinding;
using Gallio.UI.Events;

namespace Gallio.Icarus.Models
{
    public class TestTreeModel : TreeModelDecorator, ITestTreeModel, Handles<TestStepFinished>
    {
        private readonly IList<ITreeBuilder> treeBuilders;

        public Observable<int> TestCount { get; private set; }

        public TestTreeModel(ComponentHandle<ITreeBuilder, TreeBuilderTraits>[] treeBuilders)
            : base(new TreeModel())
        {
            this.treeBuilders = new List<ITreeBuilder>(OrderBuilders(treeBuilders));

            TestCount = new Observable<int>();
        }

        private static IEnumerable<ITreeBuilder> OrderBuilders(IEnumerable<ComponentHandle<ITreeBuilder, TreeBuilderTraits>> componentHandles)
        {
            var handles = new List<ComponentHandle<ITreeBuilder, TreeBuilderTraits>>(componentHandles);
            
            handles.Sort((l, r) => l.GetTraits().Priority.CompareTo(r.GetTraits().Priority));
            
            foreach (var handle in handles)
                yield return handle.GetComponent();
        }

        public void UpdateTestCount()
        {
            // (this event is fired when a node checkstate changes)
            var testCount = 0;
            
            foreach (Node node in innerTreeModel.GetChildren(new TreePath()))
                testCount += CountTests(node);

            TestCount.Value = testCount;
        }

        private static int CountTests(Node node)
        {
            int count = 0;

            var testDataNode = node as TestDataNode;
            
            if (NodeIsASelectedTest(testDataNode))
                count += 1;

            foreach (var n in node.Nodes)
                count += CountTests(n);

            return count;
        }

        private static bool NodeIsASelectedTest(TestDataNode testDataNode)
        {
            return testDataNode != null && testDataNode.IsTest && testDataNode.IsChecked;
        }

        public TestTreeNode Root
        {
            get
            {
                return (TestTreeNode)GetRoot();
            }
        }

        public void ResetTestStatus(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Resetting test statuses", 100))
            {
                foreach (Node node in innerTreeModel.GetChildren(new TreePath()))
                    ((TestTreeNode)node).Reset();

                OnNodesChanged(new TreeModelEventArgs(new TreePath(Root), new object[] {}));
            }
        }

        public void BuildTestTree(IProgressMonitor progressMonitor, TestModelData testModelData, 
            TreeBuilderOptions options)
        {
            if (string.IsNullOrEmpty(options.TreeViewCategory))
                throw new ArgumentException("Tree view category cannot be null or empty.");

            int count = CountTestData(testModelData.RootTest);

            using (progressMonitor.BeginTask("Building test tree", count))
            {
                ((TreeModel)innerTreeModel).Root.Nodes.Clear();

                TestTreeNode root = null;
                foreach (var treeBuilder in treeBuilders)
                {
                    if (!treeBuilder.CanHandle(options.TreeViewCategory)) 
                        continue;

                    root = treeBuilder.BuildTree(progressMonitor, testModelData, options);
                    break;
                }

                if (root == null)
                    throw new Exception(string.Format("Could not find a tree builder for {0}", 
                        options.TreeViewCategory));

                ((TreeModel)innerTreeModel).Root.Nodes.Add(root);

                OnStructureChanged(new TreePathEventArgs(new TreePath(root)));
            }
        }

        private static int CountTestData(TestData testData)
        {
            int count = 1;
            foreach (var td in testData.Children)
                count += CountTestData(td);
            return count;
        }

        public void Handle(TestStepFinished @event)
        {
            if (Root == null)
                return;

            var nodes = Root.Find(@event.TestData.Id, true);
            foreach (var node in nodes) // there should only be one
            {
                node.AddTestStepRun(@event.TestStepRun);
            }
        }
    }
}
