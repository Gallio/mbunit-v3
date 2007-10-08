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

using System.Windows.Forms;
using MbUnit.Icarus.Controls;
using MbUnit.Icarus.Interfaces;
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
            TreeNode[] tnode1 = new TreeNode[1];
            TestData tr = testModel.RootTest;
            TestTreeNode ttnode1 = new TestTreeNode(tr.Name, 0, 0);
            ttnode1.ExpandAll();
            ttnode1.Name = tr.Id;
            tnode1[0] = ttnode1;
                
            int kids = tr.Children.Count;
            TreeNode[] tnode2 = new TreeNode[kids];
            for (int ia = 0; ia < kids; ia++)
            {
                TestTreeNode ttnode2 = new TestTreeNode(tr.Children[ia].Name, 1, 1);
                ttnode2.ExpandAll();
                ttnode2.Name = tr.Children[ia].Id;
                tnode2[0] = ttnode2;

                int kids2 = tr.Children[ia].Children.Count;
                TreeNode[] tnode3 = new TreeNode[kids2];
                for (int i = 0; i < kids2; i++)
                {
                    TestTreeNode ttnode3 = new TestTreeNode(tr.Children[ia].Children[i].Name, 2, 2);
                    ttnode3.ExpandAll();
                    ttnode3.Name = tr.Children[ia].Children[i].Id;
                    tnode3[i] = ttnode3;

                    int kids3 = tr.Children[ia].Children[i].Children.Count;
                    TreeNode[] tnode4 = new TreeNode[kids3];
                    
                    for (int i2 = 0; i2 < kids3; i2++)
                    {
                        //string componentKind =
                        //    tr.Children[ia].Children[i].Children[i2].Metadata["ComponentKind"][0];

                        //if (componentKind == "Fixture")
                        //{
                            //MessageBox.Show("ok");
                            TestTreeNode ttnode4 =
                                new TestTreeNode(tr.Children[ia].Children[i].Children[i2].Name, 3, 3);

                            int kids4 = tr.Children[ia].Children[i].Children[i2].Children.Count;

                            ttnode4.Name = tr.Children[ia].Children[i].Children[i2].Id;
                            tnode4[i2] = ttnode4;

                            TreeNode[] tnode5 = new TreeNode[kids4];
                            for (int i3 = 0; i3 < kids4; i3++)
                            {
                                TestTreeNode ttnode5 =
                                    new TestTreeNode(tr.Children[ia].Children[i].Children[i2].Children[i3].Name, 4, 4);
                                ttnode5.Name = tr.Children[ia].Children[i].Children[i2].Children[i3].Id;
                                tnode5[i3] = ttnode5;

                                int kids5 = tr.Children[ia].Children[i].Children[i2].Children[i3].Children.Count;

                                if (kids5 > 0)
                                {
                                    TreeNode[] tnode6 = new TreeNode[kids5];
                                    for (int i4 = 0; i4 < kids5; i4++)
                                    {
                                        TestTreeNode ttnode6 =
                                            new TestTreeNode(
                                                tr.Children[ia].Children[i].Children[i2].Children[i3].Children[i4].Name, 4,
                                                4);
                                        ttnode6.Name = tr.Children[ia].Children[i].Children[i2].Children[i3].Children[i4].Id;
                                        tnode6[i4] = ttnode6;

                                        int kids6 = tr.Children[ia].Children[i].Children[i2].Children[i3].Children[i4].Children.Count;

                                        if (kids6 > 0)
                                        {
                                            //start

                                            TreeNode[] tnode7 = new TreeNode[kids6];
                                            for (int i5 = 0; i5 < kids6; i5++)
                                            {
                                                TestTreeNode ttnode7 =
                                                    new TestTreeNode(
                                                        tr.Children[ia].Children[i].Children[i2].Children[i3].Children[i4].Children[i5].Name, 4,
                                                        4);
                                                ttnode7.Name = tr.Children[ia].Children[i].Children[i2].Children[i3].Children[i4].Children[i5].Id;
                                                tnode7[i5] = ttnode7;

                                                int kids7 = tr.Children[ia].Children[i].Children[i2].Children[i3].Children[i4].Children[i5].Children.Count;

                                                if (kids7 > 0)
                                                {
                                                    //start
                                                    TreeNode[] tnode8 = new TreeNode[kids7];
                                                    for (int i6 = 0; i6 < kids7; i6++)
                                                    {
                                                        TestTreeNode ttnode8 =
                                                            new TestTreeNode(
                                                                tr.Children[ia].Children[i].Children[i2].Children[i3].Children[i4].Children[i5].Children[i6].Name, 4,
                                                                4);
                                                        ttnode8.Name = tr.Children[ia].Children[i].Children[i2].Children[i3].Children[i4].Children[i5].Children[i6].Id;
                                                        tnode8[i6] = ttnode8;
                                                    }

                                                    tnode7[i5].Nodes.AddRange(tnode8);
                                                    //end

                                                }
                                            }

                                            tnode6[i4].Nodes.AddRange(tnode7);
                                            //end
                                        }
                                    }

                                    tnode5[i3].Nodes.AddRange(tnode6);
                                }
                            }

                            tnode4[i2].Nodes.AddRange(tnode5);
                        //}
                    }

                    if (tnode4[0] != null)
                    {
                        tnode3[i].Nodes.AddRange(tnode4);
                    }
                }
                tnode2[0].Nodes.AddRange(tnode3);
            }

            tnode1[0].Nodes.AddRange(tnode2);

            return tnode1;

        }
    }
}