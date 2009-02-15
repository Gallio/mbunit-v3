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
using Aga.Controls.Tree.NodeControls;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Model;

namespace Gallio.Icarus.Controls
{
    public class TestTreeView : TreeViewAdv
    {
        private readonly NodeCheckBox nodeCheckBox;

        public bool EditEnabled
        {
            get { return nodeCheckBox.EditEnabled; }
            set { nodeCheckBox.EditEnabled = value; }
        }

        public List<string> CollapsedNodes
        {
            get
            {
                List<string> collapsedNodes = new List<string>();
                foreach (TreeNodeAdv treeNode in AllNodes)
                {
                    if (!treeNode.IsExpanded)
                        collapsedNodes.Add(((TestTreeNode)treeNode.Tag).Name);
                }
                return collapsedNodes;
            }
        }

        public TestTreeView()
        {
            nodeCheckBox = new NodeCheckBox
                               {
                                   DataPropertyName = "CheckState",
                                   LeftMargin = 0,
                                   ParentColumn = null,
                                   ThreeState = true
                               };
            NodeControls.Add(nodeCheckBox);

            NodeIcon nodeTypeIcon = new NodeIcon
                                        {
                                            DataPropertyName = "NodeTypeIcon",
                                            LeftMargin = 1,
                                            ParentColumn = null
                                        };
            NodeControls.Add(nodeTypeIcon);

            NodeIcon testStateIcon = new NodeIcon
                                         {
                                             DataPropertyName = "TestStatusIcon",
                                             LeftMargin = 1,
                                             ParentColumn = null
                                         };
            NodeControls.Add(testStateIcon);

            NodeTextBox nodeTextBox = new NodeTextBox
                                          {
                                              DataPropertyName = "Text",
                                              IncrementalSearchEnabled = true,
                                              EditEnabled = false,
                                              LeftMargin = 3,
                                              ParentColumn = null
                                          };
            nodeTextBox.DrawText += nodeTextBox_DrawText;
            NodeControls.Add(nodeTextBox);
        }

        public IOptionsController OptionsController { get; set; }

        private void nodeTextBox_DrawText(object sender, DrawEventArgs e)
        {
            if (OptionsController == null)
                return;

            TestTreeNode node = (TestTreeNode)e.Node.Tag;
            switch (node.TestStatus)
            {
                case TestStatus.Passed:
                    e.TextColor = OptionsController.PassedColor;
                    break;
                case TestStatus.Failed:
                    e.TextColor = OptionsController.FailedColor;
                    break;
                case TestStatus.Skipped:
                    e.TextColor = OptionsController.SkippedColor;
                    break;
                case TestStatus.Inconclusive:
                    e.TextColor = OptionsController.InconclusiveColor;
                    break;
            }
        }

        public void Expand(TestStatus state)
        {
            BeginUpdate();
            CollapseAll();
            foreach (TreeNodeAdv node in AllNodes)
                TestNodes(node, state);
            EndUpdate();
        }

        private void TestNodes(TreeNodeAdv node, TestStatus state)
        {
            if (node.Tag is TestTreeNode)
                if (((TestTreeNode)node.Tag).TestStatus == state)
                    Expand(node);

            // Loop though all the child nodes and expand them if they
            // meet the test state.
            foreach (TreeNodeAdv tNode in node.Children)
                TestNodes(tNode, state);
        }

        public void Expand(TreeNodeAdv node)
        {
            // Loop through all parent nodes that are not already
            // expanded and expand them.
            if (node.Parent != null && !node.Parent.IsExpanded)
                Expand(node.Parent);

            node.Expand();
        }

        public void CollapseNodes(IList<string> nodes)
        {
            ExpandAll();
            foreach (TreeNodeAdv treeNode in AllNodes)
            {
                if (treeNode.IsExpanded && nodes.Contains(((TestTreeNode)treeNode.Tag).Name))
                    treeNode.Collapse();
            }
        }
    }
}
