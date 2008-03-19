// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Aga.Controls.Tree;

using Gallio.Icarus.Controls;
using Gallio.Icarus.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Icarus.Core.CustomEventArgs;

namespace Gallio.Icarus.AdapterModel
{
    /// <summary>
    /// Adapter Model for the Project Triad
    /// </summary>
    public class ProjectAdapterModel : IProjectAdapterModel
    {
        private TestTreeModel testTreeModel;
        private SortedTreeModel sortedTreeModel;

        public ITreeModel TreeModel
        {
            get { return sortedTreeModel; }
        }

        public ProjectAdapterModel()
        {
            testTreeModel = new TestTreeModel();
            sortedTreeModel = new SortedTreeModel(testTreeModel);
        }

        /// <summary>
        /// Builds the tree model from a Gallio test model.
        /// </summary>
        /// <param name="testModelData">Gallio test model data</param>
        /// <param name="mode">How to group the tree.</param>
        public void BuildTestTree(TestModelData testModelData, string mode)
        {
            testTreeModel.Nodes.Clear();
            TestTreeNode root = new TestTreeNode(testModelData.RootTest.Name, testModelData.RootTest.Id, TestKinds.Root);
            testTreeModel.Nodes.Add(root);
            switch (mode)
            {
                case "Namespaces":
                    PopulateNamespaceTree(testModelData.RootTest.Children, root);
                    break;

                case "Authors":
                    PopulateMetadataTree(MetadataKeys.AuthorName, testModelData.RootTest.Children, root);
                    break;

                case "Categories":
                    PopulateMetadataTree(MetadataKeys.CategoryName, testModelData.RootTest.Children, root);
                    break;
                
                case "Importance":
                    PopulateMetadataTree(MetadataKeys.Importance, testModelData.RootTest.Children, root);
                    break;

                case "TestsOn":
                    PopulateMetadataTree(MetadataKeys.TestsOn, testModelData.RootTest.Children, root);
                    break;
            }
            testTreeModel.OnTestCountChanged(EventArgs.Empty);
        }

        private void PopulateNamespaceTree(List<TestData> list, TestTreeNode parent)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TestData td = list[i];
                string componentKind = td.Metadata.GetValue(MetadataKeys.TestKind);
                if (componentKind != null)
                {
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
                        string @namespace = td.CodeReference.NamespaceName ?? "";

                        // find the namespace node (or add if it doesn't exist)
                        TestTreeNode nsNode;
                        List<TestTreeNode> nodes = parent.Find(@namespace, true);
                        if (nodes.Count > 0)
                            nsNode = nodes[0] as TestTreeNode;
                        else
                        {
                            nsNode = new TestTreeNode(@namespace, @namespace, "Namespace");
                            parent.Nodes.Add(nsNode);
                        }

                        // add the fixture to the namespace
                        ttnode = new TestTreeNode(td.Name, td.Id, componentKind);
                        nsNode.Nodes.Add(ttnode);
                    }
                    ttnode.SourceCodeAvailable = (td.CodeLocation != null);
                    ttnode.IsTest = td.IsTestCase;
                    
                    // process child nodes
                    PopulateNamespaceTree(td.Children, ttnode);
                }
            }
        }

        private void PopulateMetadataTree(string key, List<TestData> list, TestTreeNode parent)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TestData td = list[i];
                string componentKind = td.Metadata.GetValue(MetadataKeys.TestKind);
                if (componentKind != null)
                {
                    switch (componentKind)
                    {
                        case TestKinds.Fixture:
                        case TestKinds.Test:
                            IList<string> metadata = td.Metadata[key];
                            if (metadata.Count == 0)
                            {
                                metadata = new List<string>();
                                metadata.Add("None");
                            }
                            foreach (string m in metadata)
                            {
                                // find metadata node (or add if it doesn't exist)
                                TestTreeNode metadataNode;
                                List<TestTreeNode> nodes = testTreeModel.Root.Find(m, false);
                                if (nodes.Count > 0)
                                    metadataNode = nodes[0] as TestTreeNode;
                                else
                                {
                                    metadataNode = new TestTreeNode(m, m, m);
                                    testTreeModel.Root.Nodes.Add(metadataNode);
                                }
                                // add node in the appropriate place
                                if (componentKind == TestKinds.Fixture)
                                {
                                    TestTreeNode ttnode = new TestTreeNode(td.Name, td.Id, componentKind);
                                    metadataNode.Nodes.Add(ttnode);
                                    PopulateMetadataTree(key, td.Children, ttnode);
                                }
                                else
                                {
                                    TestTreeNode ttnode = new TestTreeNode(td.Name, td.Id, componentKind);
                                    ttnode.SourceCodeAvailable = (td.CodeLocation != null);
                                    ttnode.IsTest = td.IsTestCase;
                                    if (m != "None")
                                        metadataNode.Nodes.Add(ttnode);
                                    else
                                        parent.Nodes.Add(ttnode);
                                }
                            }
                            break;
                    }
                    if (componentKind != TestKinds.Fixture)
                        PopulateMetadataTree(key, td.Children, parent);
                }
            }
        }

        private Image GetNodeTypeIcon(string componentKind)
        {
            switch (componentKind)
            {
                case TestKinds.Assembly:
                    return global::Gallio.Icarus.Properties.Resources.Assembly;
                case TestKinds.Fixture:
                    return global::Gallio.Icarus.Properties.Resources.Fixture;
                case TestKinds.Test:
                    return global::Gallio.Icarus.Properties.Resources.Test;
                default:
                    return null;
            }
        }

        public ListViewItem[] BuildAssemblyList(List<string> assemblyList)
        {
            ListViewItem[] assemblies = new ListViewItem[assemblyList.Count];
            for (int i = 0; i < assemblyList.Count; i++)
            {
                string assemblyPath = assemblyList[i];
                string assemblyName = Path.GetFileName(assemblyPath);
                string assemblyVersion = FileVersionInfo.GetVersionInfo(assemblyPath).FileVersion;
                string[] assemblyInfo = new string[] { assemblyName, assemblyVersion, assemblyPath };
                assemblies[i] = new ListViewItem(assemblyInfo);
            }
            return assemblies;
        }

        public void Update(TestData testData, TestStepRun testStepRun)
        {
            // update tree model
            testTreeModel.UpdateTestStatus(testData.Id, testStepRun.Result.Outcome.Status);
            // get code reference (if there is one)
            CodeReference codeReference = testData.CodeReference;
            // update test results
            double duration = (testStepRun.EndTime - testStepRun.StartTime).TotalMilliseconds;
            testTreeModel.OnTestResult(new TestResultEventArgs(testData.Name, testStepRun.Result.Outcome,
                duration, codeReference.TypeName, codeReference.NamespaceName, codeReference.AssemblyName));
        }

        public Filter<ITest> CreateFilter()
        {
            if (testTreeModel.Root.CheckState == CheckState.Checked)
                return new AnyFilter<ITest>();
            if (testTreeModel.Root.CheckState == CheckState.Unchecked)
                return new NoneFilter<ITest>();
            return CreateFilter(testTreeModel.Nodes);
        }

        private Filter<ITest> CreateFilter(Collection<Node> nodes)
        {
            List<Filter<ITest>> filters = new List<Filter<ITest>>();
            foreach (Node n in nodes)
            {
                if (n is TestTreeNode)
                {
                    TestTreeNode node = (TestTreeNode)n;
                    switch (node.CheckState)
                    {
                        case CheckState.Checked:
                            {
                                if (node.NodeTypeIcon != global::Gallio.Icarus.Properties.Resources.Namespace)
                                    filters.Add(new IdFilter<ITest>(new EqualityFilter<string>(node.Name)));
                                else
                                {
                                    Filter<ITest> childFilters = CreateFilter(node.Nodes);
                                    if (childFilters != null)
                                        filters.Add(childFilters);
                                }
                                break;
                            }
                        case CheckState.Indeterminate:
                            {
                                Filter<ITest> childFilters = CreateFilter(node.Nodes);
                                if (childFilters != null)
                                    filters.Add(childFilters);
                                break;
                            }
                    }
                }
            }
            return new OrFilter<ITest>(filters.ToArray());
        }

        public void ApplyFilter(Filter<ITest> filter)
        {
            if (!(filter is AnyFilter<ITest>))
            {
                // toggle root node
                foreach (Node node in testTreeModel.Nodes)
                {
                    if (node is TestTreeNode)
                    {
                        node.CheckState = CheckState.Unchecked;
                        ((TestTreeNode)node).UpdateStateOfRelatedNodes();
                    }
                }
                RecursivelyApplyFilter(filter);
                testTreeModel.OnTestCountChanged(EventArgs.Empty);
            }
        }

        private void RecursivelyApplyFilter(Filter<ITest> filter)
        {
            if (filter is NoneFilter<ITest>)
                return;
            if (filter is OrFilter<ITest>)
            {
                OrFilter<ITest> orFilter = (OrFilter<ITest>)filter;
                foreach (Filter<ITest> childFilter in orFilter.Filters)
                    RecursivelyApplyFilter(childFilter);
            }
            else if (filter is IdFilter<ITest>)
            {
                IdFilter<ITest> idFilter = (IdFilter<ITest>)filter;
                EqualityFilter<string> equalityFilter = (EqualityFilter<string>)idFilter.ValueFilter;
                foreach (TestTreeNode n in testTreeModel.Root.Find(equalityFilter.Comparand, true))
                {
                    n.CheckState = CheckState.Checked;
                    n.UpdateStateOfRelatedNodes();
                }
            }
        }

        public void ResetTestStatus()
        {
            testTreeModel.ResetTestStatus();
        }
    }
}