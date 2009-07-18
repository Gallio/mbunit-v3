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
using Gallio.Icarus.Models;
using Gallio.Model;
using System.Windows.Forms;
using System.Drawing;

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

        public Color PassedColor { get; set; }
        
        public Color FailedColor { get; set; }
        
        public Color SkippedColor { get; set; }

        public Color InconclusiveColor { get; set; }

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

        private void nodeTextBox_DrawText(object sender, DrawEventArgs e)
        {
            var node = (TestTreeNode)e.Node.Tag;
            switch (node.TestStatus)
            {
                case TestStatus.Passed:
                    e.TextColor = PassedColor;
                    break;
                case TestStatus.Failed:
                    e.TextColor = FailedColor;
                    break;
                case TestStatus.Skipped:
                    e.TextColor = SkippedColor;
                    break;
                case TestStatus.Inconclusive:
                    e.TextColor = InconclusiveColor;
                    break;
            }
        }

        public void Expand(TestStatus state)
        {
            BeginUpdate();
            
            CollapseAll();
            
            foreach (TreeNodeAdv node in AllNodes)
                ExpandNode(node, state);
            
            EndUpdate();
        }

        private void ExpandNode(TreeNodeAdv node, TestStatus state)
        {
            if (((TestTreeNode)node.Tag).TestStatus == state)
                Expand(node);

            foreach (TreeNodeAdv tNode in node.Children)
                ExpandNode(tNode, state);
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

        public void Select(TestStatus testStatus)
        {
            BeginUpdate();

            CollapseAll();

            foreach (TreeNodeAdv node in AllNodes)
                SelectNode(node, testStatus);

            EndUpdate();
        }

        private void SelectNode(TreeNodeAdv node, TestStatus testStatus)
        {
            if (((TestTreeNode)node.Tag).IsTest)
            {
                if (((TestTreeNode)node.Tag).TestStatus == testStatus)
                {
                    ((TestTreeNode)node.Tag).CheckState = CheckState.Checked;
                    Expand(node);
                }
                else
                {
                    ((TestTreeNode)node.Tag).CheckState = CheckState.Unchecked;
                }
            }
            else
                ((TestTreeNode)node.Tag).CheckState = CheckState.Unchecked;

            foreach (TreeNodeAdv tNode in node.Children)
                SelectNode(tNode, testStatus);
        }
    }
}
