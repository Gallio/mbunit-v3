// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Gallio.Icarus.Controls;
using Gallio.Icarus.Controls.Enums;
using Gallio.Icarus.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Reflection;

namespace Gallio.Icarus.AdapterModel
{
    /// <summary>
    /// Adapter Model for the Project Triad
    /// </summary>
    public class ProjectAdapterModel : IProjectAdapterModel
    {
        /// <summary>
        /// Builds a winforms test tree from a gallio test tree
        /// </summary>
        /// <param name="testModelData">gallio test tree</param>
        /// <returns></returns>
        public TreeNode[] BuildTestTree(TestModelData testModelData, string mode, bool initialCheckState)
        {
            TestTreeNode root = new TestTreeNode(testModelData.RootTest.Name, testModelData.RootTest.Id, 0, initialCheckState);
            switch (mode)
            {
                case "Namespaces":
                    PopulateNamespaceTree(testModelData.RootTest.Children, root, initialCheckState);
                    break;

                case "Authors":
                    PopulateMetadataTree(MetadataKeys.AuthorName, testModelData.RootTest.Children, root, root, initialCheckState);
                    break;

                case "Categories":
                    PopulateMetadataTree(MetadataKeys.CategoryName, testModelData.RootTest.Children, root, root, initialCheckState);
                    break;
                
                case "Importance":
                    PopulateMetadataTree(MetadataKeys.Importance, testModelData.RootTest.Children, root, root, initialCheckState);
                    break;

                case "TestsOn":
                    PopulateMetadataTree(MetadataKeys.TestsOn, testModelData.RootTest.Children, root, root, initialCheckState);
                    break;
            }
            root.ExpandAll();
            return new TreeNode[] { root };
        }

        private void PopulateNamespaceTree(List<TestData> list, TestTreeNode parent, bool initialCheckState)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TestData td = list[i];
                string componentKind = td.Metadata.GetValue(MetadataKeys.TestKind);
                if (componentKind != null)
                {
                    int imgIndex = GetImageIndex(componentKind);
                    TestTreeNode ttnode;
                    if (componentKind != TestKinds.Fixture)
                    {
                        // create an appropriate node
                        ttnode = new TestTreeNode(td.Name, td.Id, imgIndex, initialCheckState);
                        string codeBase = td.Metadata.GetValue(MetadataKeys.CodeBase);
                        if (codeBase != null)
                        {
                            ttnode.CodeBase = codeBase;
                        }
                        parent.Nodes.Add(ttnode);
                    }
                    else
                    {
                        // fixtures need special treatment to insert the namespace layer!
                        string @namespace = (td.CodeReference ?? CodeReference.Unknown).NamespaceName ?? "";

                        // find the namespace node (or add if it doesn't exist)
                        TestTreeNode nsNode;
                        if (parent.Nodes.ContainsKey(@namespace))
                        {
                            nsNode = parent.Nodes.Find(@namespace, false)[0] as TestTreeNode;
                        }
                        else
                        {
                            nsNode = new TestTreeNode(@namespace, @namespace, 2, initialCheckState);
                            parent.Nodes.Add(nsNode);
                        }

                        // add the fixture to the namespace
                        ttnode = new TestTreeNode(td.Name, td.Id, 3, initialCheckState);
                        nsNode.Nodes.Add(ttnode);
                    }
                    // process child nodes
                    PopulateNamespaceTree(td.Children, ttnode, initialCheckState);
                }
            }
        }

        private void PopulateMetadataTree(string key, List<TestData> list, TestTreeNode root, TestTreeNode parent, bool initialCheckState)
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
                            int imgIndex = GetImageIndex(componentKind);
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
                                if (root.Nodes.ContainsKey(m))
                                {
                                    metadataNode = root.Nodes.Find(m, false)[0] as TestTreeNode;
                                }
                                else
                                {
                                    metadataNode = new TestTreeNode(m, m, 0, initialCheckState);
                                    root.Nodes.Add(metadataNode);
                                }
                                // add node in the appropriate place
                                if (componentKind == TestKinds.Fixture)
                                {
                                    TestTreeNode ttnode = new TestTreeNode(td.Name, td.Id, imgIndex, initialCheckState);
                                    metadataNode.Nodes.Add(ttnode);
                                    PopulateMetadataTree(key, td.Children, root, ttnode, initialCheckState);
                                }
                                else
                                {
                                    TestTreeNode ttnode = new TestTreeNode(td.Name, td.Id, imgIndex, initialCheckState);
                                    if (m != "None")
                                    {
                                        metadataNode.Nodes.Add(ttnode);
                                    }
                                    else
                                    {
                                        parent.Nodes.Add(ttnode);
                                    }
                                }
                            }
                            break;
                    }
                    if (componentKind != TestKinds.Fixture)
                    {
                        PopulateMetadataTree(key, td.Children, root, parent, initialCheckState);
                    }
                }
            }
        }

        private int GetImageIndex(string componentKind)
        {
            switch (componentKind)
            {
                case TestKinds.Root:
                case TestKinds.Framework:
                    return 0;
                case TestKinds.Assembly:
                    return 1;
                case TestKinds.Fixture:
                    return 3;
                case TestKinds.Test:
                    return 4;
                default:
                    return 0;
            }
        }

        public int CountTests(TestModelData testModelData)
        {
            return CountTests(testModelData.RootTest);
        }

        private int CountTests(TestData td)
        {
            int testCount = 0;
            if (td.IsTestCase)
                testCount++;
            foreach (TestData child in td.Children)
                testCount += CountTests(child);
            return testCount;
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

        public Filter<ITest> GetFilter(TreeNodeCollection treeNodeCollection)
        {
            List<Filter<ITest>> filters = new List<Filter<ITest>>();
            foreach (TestTreeNode node in treeNodeCollection)
            {
                switch (node.CheckState)
                {
                    case CheckBoxStates.Checked:
                        {
                            if (node.SelectedImageIndex != 2)
                            {
                                filters.Add(new IdFilter<ITest>(new EqualityFilter<string>(node.Name)));
                            }
                            else
                            {
                                Filter<ITest> childFilters = GetFilter(node.Nodes);
                                if (childFilters != null)
                                {
                                    filters.Add(childFilters);
                                }
                            }
                            break;
                        }
                    case CheckBoxStates.Indeterminate:
                        {
                            Filter<ITest> childFilters = GetFilter(node.Nodes);
                            if (childFilters != null)
                            {
                                filters.Add(childFilters);
                            }
                            break;
                        }
                }
            }
            if (filters.Count > 0)
            {
                return new OrFilter<ITest>(filters.ToArray());
            }
            else
            {
                return null;
            }
        }
    }
}