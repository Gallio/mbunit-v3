// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
        /// <param name="testModel">gallio test tree</param>
        /// <returns></returns>
        public TreeNode[] BuildTestTree(TestModel testModel, string mode)
        {
            TestTreeNode root = new TestTreeNode(testModel.RootTest.Name, testModel.RootTest.Id, 0);
            switch (mode)
            {
                case "Namespaces":
                    PopulateNamespaceTree(testModel.RootTest.Children, root);
                    break;

                case "Authors":
                    PopulateMetadataTree(MetadataKeys.AuthorName, testModel.RootTest.Children, root, root);
                    break;

                case "Categories":
                    PopulateMetadataTree(MetadataKeys.CategoryName, testModel.RootTest.Children, root, root);
                    break;
                
                case "Importance":
                    PopulateMetadataTree(MetadataKeys.Importance, testModel.RootTest.Children, root, root);
                    break;

                case "TestsOn":
                    PopulateMetadataTree(MetadataKeys.TestsOn, testModel.RootTest.Children, root, root);
                    break;
            }
            root.ExpandAll();
            return new TreeNode[] { root };
        }

        private void PopulateNamespaceTree(List<TestData> list, TestTreeNode parent)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TestData td = list[i];
                string componentKind = td.Metadata.GetValue(MetadataKeys.ComponentKind);
                if (componentKind != null)
                {
                    int imgIndex = GetImageIndex(componentKind);
                    TestTreeNode ttnode;
                    if (componentKind != ComponentKind.Fixture)
                    {
                        // create an appropriate node
                        ttnode = new TestTreeNode(td.Name, td.Id, imgIndex);
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
                        string nameSpace = td.CodeReference.NamespaceName;
                        // find the namespace node (or add if it doesn't exist)
                        TestTreeNode nsNode;
                        if (parent.Nodes.ContainsKey(nameSpace))
                        {
                            nsNode = parent.Nodes.Find(nameSpace, false)[0] as TestTreeNode;
                        }
                        else
                        {
                            nsNode = new TestTreeNode(nameSpace, nameSpace, 2);
                            parent.Nodes.Add(nsNode);
                        }
                        // add the fixture to the namespace
                        ttnode = new TestTreeNode(td.Name, td.Id, 3);
                        nsNode.Nodes.Add(ttnode);
                    }
                    // process child nodes
                    PopulateNamespaceTree(td.Children, ttnode);
                }
            }
        }

        private void PopulateMetadataTree(string key, List<TestData> list, TestTreeNode root, TestTreeNode parent)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TestData td = list[i];
                string componentKind = td.Metadata.GetValue(MetadataKeys.ComponentKind);
                if (componentKind != null)
                {
                    switch (componentKind)
                    {
                        case ComponentKind.Fixture:
                        case ComponentKind.Test:
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
                                    metadataNode = new TestTreeNode(m, m, 0);
                                    root.Nodes.Add(metadataNode);
                                }
                                // add node in the appropriate place
                                if (componentKind == ComponentKind.Fixture)
                                {
                                    TestTreeNode ttnode = new TestTreeNode(td.Name, td.Id, imgIndex);
                                    metadataNode.Nodes.Add(ttnode);
                                    PopulateMetadataTree(key, td.Children, root, ttnode);
                                }
                                else
                                {
                                    TestTreeNode ttnode = new TestTreeNode(td.Name, td.Id, imgIndex);
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
                    if (componentKind != ComponentKind.Fixture)
                    {
                        PopulateMetadataTree(key, td.Children, root, parent);
                    }
                }
            }
        }

        private int GetImageIndex(string componentKind)
        {
            switch (componentKind)
            {
                case ComponentKind.Root:
                case ComponentKind.Framework:
                    return 0;
                case ComponentKind.Assembly:
                    return 1;
                case ComponentKind.Fixture:
                    return 3;
                case ComponentKind.Test:
                    return 4;
                default:
                    return 0;
            }
        }

        public int CountTests(TestModel testModel)
        {
            return CountTests(testModel.RootTest);
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