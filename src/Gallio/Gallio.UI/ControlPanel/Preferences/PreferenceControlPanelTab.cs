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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Gallio.Common;

namespace Gallio.UI.ControlPanel.Preferences
{
    internal partial class PreferenceControlPanelTab : ControlPanelTab
    {
        public PreferenceControlPanelTab()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds a preference pane.
        /// </summary>
        /// <param name="path">The preference pane path consisting of slash-delimited name segments
        /// specifying tree nodes</param>
        /// <param name="icon">The preference pane icon, or null if none</param>
        /// <param name="paneFactory">The preference pane factory</param>
        public void AddPane(string path, Icon icon, Func<PreferencePane> paneFactory)
        {
            string[] pathSegments = path.Split('/');
            if (pathSegments.Length == 0)
                throw new ArgumentException("Preference pane path must not be empty.", "path");

            TreeNode treeNode = null;
            foreach (string pathSegment in pathSegments)
            {
                TreeNodeCollection treeNodeCollection = treeNode != null
                    ? treeNode.Nodes
                    : preferencePaneTree.Nodes;

                TreeNode childTreeNode = FindTreeNodeByName(treeNodeCollection, pathSegment);
                if (childTreeNode == null)
                {
                    childTreeNode = new TreeNode(pathSegment);
                    childTreeNode.Tag = new Func<PreferencePane>(CreatePlaceholderPreferencePane);
                    treeNodeCollection.Add(childTreeNode);
                }

                treeNode = childTreeNode;
            }

            treeNode.Tag = paneFactory;

            if (icon != null)
            {
                int imageIndex = preferencePaneIconImageList.Images.Count;
                preferencePaneIconImageList.Images.Add(icon);

                treeNode.ImageIndex = imageIndex;
                treeNode.SelectedImageIndex = imageIndex;
            }
        }

        /// <inheritdoc />
        public override void ApplySettingsChanges()
        {
            foreach (PreferencePane preferencePane in splitContainer1.Panel2.Controls)
                preferencePane.ApplySettingsChanges();
        }

        private static PreferencePane CreatePlaceholderPreferencePane()
        {
            return new PlaceholderPreferencePane();
        }

        private static TreeNode FindTreeNodeByName(TreeNodeCollection collection, string name)
        {
            foreach (TreeNode node in collection)
                if (node.Text == name)
                    return node;
            return null;
        }

        private void EnsurePaneVisible()
        {
            TreeNode treeNode = preferencePaneTree.SelectedNode;
            if (treeNode != null)
            {
                bool found = false;
                foreach (PreferencePane candidate in splitContainer1.Panel2.Controls)
                {
                    if (candidate.Tag == treeNode)
                    {
                        candidate.Visible = true;
                        found = true;
                    }
                    else
                    {
                        candidate.Visible = false;
                    }
                }

                if (!found)
                {
                    Func<PreferencePane> paneFactory = (Func<PreferencePane>) treeNode.Tag;
                    PreferencePane preferencePane = paneFactory();
                    preferencePane.Dock = DockStyle.Fill;
                    preferencePane.Margin = new Padding(0, 0, 0, 0);
                    preferencePane.AutoSize = true;
                    preferencePane.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    preferencePane.Tag = treeNode;
                    preferencePane.SettingsChanged += preferencePane_SettingsChanged;
                    splitContainer1.Panel2.Controls.Add(preferencePane);
                }
            }
        }

        private void preferencePaneTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            EnsurePaneVisible();
        }

        private void PreferenceControlPanelTab_Load(object sender, EventArgs e)
        {
            if (preferencePaneTree.SelectedNode == null
                && preferencePaneTree.Nodes.Count != 0)
            {
                preferencePaneTree.SelectedNode = preferencePaneTree.Nodes[0];
            }
        }

        private void preferencePane_SettingsChanged(object sender, EventArgs e)
        {
            OnSettingsChanged(e);
        }
    }
}
