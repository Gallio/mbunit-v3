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
using System.Windows.Forms;

using MbUnit.Icarus.Controls;
using MbUnit.Icarus.Controls.Enums;
using MbUnit.Icarus.Interfaces;
using MbUnit.Model;
using MbUnit.Model.Filters;
using MbUnit.Model.Serialization;

namespace MbUnit.Icarus.AdapterModel
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
        public TreeNode[] BuildTestTree(TestModel testModel)
        {
            TreeNode[] testTree = new TreeNode[1];
            TestTreeNode root = new TestTreeNode(testModel.RootTest.Name, 0, 0, WalkTestTree(testModel.RootTest.Children));
            root.Name = testModel.RootTest.Id;
            root.ExpandAll();
            root.Checked = true;
            root.CheckState = CheckBoxStates.Checked;
            testTree[0] = root;
            return testTree;
        }

        private TestTreeNode[] WalkTestTree(List<TestData> list)
        {
            TestTreeNode[] nodes = new TestTreeNode[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                TestData td = list[i];
                int imgIndex = 0;
                string codeBase = null;
                string componentKind = td.Metadata.GetValue("ComponentKind");
                if (componentKind != null)
                {
                    switch (componentKind)
                    {
                        case "Framework":
                            imgIndex = 1;
                            break;
                        case "Assembly":
                            imgIndex = 2;
                            codeBase = td.Metadata.GetValue("CodeBase");
                            break;
                        case "Fixture":
                            imgIndex = 3;
                            break;
                        case "Test":
                            imgIndex = 4;
                            break;
                    }
                    TestTreeNode ttnode = new TestTreeNode(td.Name, imgIndex, imgIndex, WalkTestTree(td.Children));
                    ttnode.Name = td.Id;
                    ttnode.Checked = true;
                    ttnode.CheckState = CheckBoxStates.Checked;
                    if (codeBase != null)
                    {
                        ttnode.CodeBase = codeBase;
                    }
                    nodes[i] = ttnode;
                }
            }
            return nodes;
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
                string assemblyName = System.IO.Path.GetFileName(assemblyPath);
                string assemblyVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyPath).FileVersion;
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
                    case CheckBoxStates.Unchecked:
                        {
                            if (node.SelectedImageIndex == 0)
                            {
                                filters.Add(new NoneFilter<ITest>());
                            }
                            break;
                        }
                    case CheckBoxStates.Checked:
                        {
                            switch (node.SelectedImageIndex)
                            {
                                case 0:
                                    {
                                        filters.Add(new AnyFilter<ITest>());
                                        break;
                                    }
                                default:
                                    {
                                        filters.Add(new IdFilter<ITest>(node.Name));
                                        break;
                                    }
                            }
                            break;
                        }
                    case CheckBoxStates.Indeterminate:
                        {
                            filters.Add(GetFilter(node.Nodes));
                            break;
                        }
                }
            }
            return new AndFilter<ITest>(filters.ToArray());
        }
    }
}