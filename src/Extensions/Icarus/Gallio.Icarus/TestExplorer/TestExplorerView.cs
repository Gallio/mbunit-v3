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
using System.ComponentModel;
using System.Windows.Forms;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Icarus.Projects;
using Gallio.Icarus.Utilities;
using Gallio.Model;
using Gallio.UI.Common.Synchronization;
using SortOrder = Gallio.Icarus.Models.SortOrder;

namespace Gallio.Icarus.TestExplorer
{
    internal partial class TestExplorerView : UserControl
    {
        private readonly ITestExplorerController testExplorerController;
        private readonly ITestExplorerModel testExplorerModel;
        private bool updateFlag;

        public TestExplorerView(ITestExplorerController testExplorerController, ITestExplorerModel testExplorerModel)
        {
            this.testExplorerController = testExplorerController;
            this.testExplorerModel = testExplorerModel;

            InitializeComponent();
        }

    	protected override void OnLoad(EventArgs eventArgs)
		{
			SetupColors();
			SetupTreeViewCategories();

			testTree.Model = testExplorerModel.TreeModel;

    		EventHandler testExplorerControllerOnRestoreState = (s, e) => RestoreState();
    		testExplorerController.RestoreState += testExplorerControllerOnRestoreState;
			Disposed += (s, e) => { testExplorerController.RestoreState -= testExplorerControllerOnRestoreState; };

    		PropertyChangedEventHandler canEditTreeOnPropertyChanged = (s, e) => testTree.SetEditEnabled(testExplorerModel.CanEditTree);
    		testExplorerModel.CanEditTree.PropertyChanged += canEditTreeOnPropertyChanged;
			Disposed += (s, e) => { testExplorerModel.CanEditTree.PropertyChanged -= canEditTreeOnPropertyChanged; };

			SetupFilters();
			SetupSorting();

    		EventHandler testExplorerControllerOnSaveState = (s, e) => SaveState();
    		testExplorerController.SaveState += testExplorerControllerOnSaveState;
			Disposed += (s, e) => { testExplorerController.SaveState -= testExplorerControllerOnSaveState; };

			base.OnLoad(eventArgs);
		}

    	private void SetupColors()
    	{
    		testTree.SetPassedColor(testExplorerModel.PassedColor);
    		testTree.SetFailedColor(testExplorerModel.FailedColor);
    		testTree.SetInconclusiveColor(testExplorerModel.InconclusiveColor);
    		testTree.SetSkippedColor(testExplorerModel.SkippedColor);

    		PropertyChangedEventHandler passedColorOnPropertyChanged = (s, e) => testTree.SetPassedColor(testExplorerModel.PassedColor);
    		PropertyChangedEventHandler failedColorOnPropertyChanged = (s, e) => testTree.SetFailedColor(testExplorerModel.FailedColor);
    		PropertyChangedEventHandler inconclusiveColorOnPropertyChanged = (s, e) => testTree.SetInconclusiveColor(testExplorerModel.InconclusiveColor);
    		PropertyChangedEventHandler skippedColorOnPropertyChanged = (s, e) => testTree.SetSkippedColor(testExplorerModel.SkippedColor);

    		testExplorerModel.PassedColor.PropertyChanged += passedColorOnPropertyChanged;
    		testExplorerModel.FailedColor.PropertyChanged += failedColorOnPropertyChanged;
    		testExplorerModel.InconclusiveColor.PropertyChanged += inconclusiveColorOnPropertyChanged;
    		testExplorerModel.SkippedColor.PropertyChanged += skippedColorOnPropertyChanged;

    		Disposed += (s, e) =>
    		            {
    		            	testExplorerModel.PassedColor.PropertyChanged -= passedColorOnPropertyChanged;
    		            	testExplorerModel.FailedColor.PropertyChanged -= failedColorOnPropertyChanged;
    		            	testExplorerModel.InconclusiveColor.PropertyChanged -= inconclusiveColorOnPropertyChanged;
    		            	testExplorerModel.SkippedColor.PropertyChanged -= skippedColorOnPropertyChanged;
    		            };
    	}

    	private void SetupTreeViewCategories()
    	{
    		updateFlag = true;
    		treeViewComboBox.ComboBox.DataSource = testExplorerModel.TreeViewCategories.Value;
    		updateFlag = false;

    		PropertyChangedEventHandler currentTreeViewCategoryOnPropertyChanged = (s, e) => treeViewComboBox.ComboBox.SelectedItem = testExplorerModel.CurrentTreeViewCategory.Value;
    		testExplorerModel.CurrentTreeViewCategory.PropertyChanged += currentTreeViewCategoryOnPropertyChanged;
    		treeViewComboBox.ComboBox.SelectedItem = testExplorerModel.CurrentTreeViewCategory.Value ?? UserOptions.DefaultTreeViewCategory;

    		PropertyChangedEventHandler treeViewCategoriesOnPropertyChanged = (s, e) =>
			{
				updateFlag = true;
				treeViewComboBox.ComboBox.DataSource = testExplorerModel.TreeViewCategories.Value;
				updateFlag = false;
			};
    		testExplorerModel.TreeViewCategories.PropertyChanged += treeViewCategoriesOnPropertyChanged;

    		Disposed += (s, e) =>
			{
				testExplorerModel.CurrentTreeViewCategory.PropertyChanged -= currentTreeViewCategoryOnPropertyChanged;
				testExplorerModel.TreeViewCategories.PropertyChanged -= treeViewCategoriesOnPropertyChanged;
			};
    	}

    	private void SetupSorting()
        {
    		EventHandler sortAscToolStripButtonOnClick = (s, e) => SortTree(SortOrder.Ascending);
    		EventHandler sortDescToolStripButtonOnClick = (s, e) => SortTree(SortOrder.Descending);

    		sortAscToolStripButton.Click += sortAscToolStripButtonOnClick;
    		sortDescToolStripButton.Click += sortDescToolStripButtonOnClick;

    		Disposed += (s, e) =>
			{
				sortAscToolStripButton.Click -= sortAscToolStripButtonOnClick;
				sortDescToolStripButton.Click -= sortDescToolStripButtonOnClick;
			};
        }

    	private void SetupFilters()
        {
            SetupPassedFilters();
            SetupFailedFilters();
            SetupInconclusiveFilters();
        }

    	private void SetupPassedFilters()
    	{
    		EventHandler filterPassedTestsToolStripMenuItemOnClick = (s, e) => FilterStatus(TestStatus.Passed);
    		filterPassedTestsToolStripMenuItem.Click += filterPassedTestsToolStripMenuItemOnClick;
    		filterPassedTestsToolStripButton.Click += filterPassedTestsToolStripMenuItemOnClick;
    		
    		PropertyChangedEventHandler filterPassedOnPropertyChanged = (s, e) =>
			{
				filterPassedTestsToolStripMenuItem.Checked = filterPassedTestsToolStripButton.Checked = testExplorerModel.FilterPassed.Value;
			};
    		testExplorerModel.FilterPassed.PropertyChanged += filterPassedOnPropertyChanged;

    		Disposed += (s, e) =>
			{
				filterPassedTestsToolStripMenuItem.Click -= filterPassedTestsToolStripMenuItemOnClick;
				filterPassedTestsToolStripButton.Click -= filterPassedTestsToolStripMenuItemOnClick;
				testExplorerModel.FilterPassed.PropertyChanged -= filterPassedOnPropertyChanged;
			};
    	}

    	private void SetupFailedFilters()
    	{
    		EventHandler filterFailedTestsToolStripMenuItemOnClick = (s, e) => FilterStatus(TestStatus.Failed);
    		filterFailedTestsToolStripMenuItem.Click += filterFailedTestsToolStripMenuItemOnClick;
    		filterFailedTestsToolStripButton.Click += filterFailedTestsToolStripMenuItemOnClick;

    		PropertyChangedEventHandler filterFailedOnPropertyChanged = (s, e) =>
			{
				filterFailedTestsToolStripMenuItem.Checked = filterFailedTestsToolStripButton.Checked = testExplorerModel.FilterFailed.Value;
			};
    		testExplorerModel.FilterFailed.PropertyChanged += filterFailedOnPropertyChanged;

    		Disposed += (s, e) =>
			{
				filterFailedTestsToolStripMenuItem.Click -= filterFailedTestsToolStripMenuItemOnClick;
				filterFailedTestsToolStripButton.Click -= filterFailedTestsToolStripMenuItemOnClick;
				testExplorerModel.FilterFailed.PropertyChanged -= filterFailedOnPropertyChanged;
			};
    	}

    	private void SetupInconclusiveFilters()
        {
    		EventHandler filterInconclusiveTestsToolStripMenuItemOnClick = (s, e) => FilterStatus(TestStatus.Inconclusive);
    		filterInconclusiveTestsToolStripMenuItem.Click += filterInconclusiveTestsToolStripMenuItemOnClick;
            filterInconclusiveTestsToolStripButton.Click += filterInconclusiveTestsToolStripMenuItemOnClick;

    		PropertyChangedEventHandler filterInconclusiveOnPropertyChanged = (s, e) =>
			{
				filterInconclusiveTestsToolStripMenuItem.Checked = filterInconclusiveTestsToolStripButton.Checked = testExplorerModel.FilterInconclusive.Value;
			};
    		testExplorerModel.FilterInconclusive.PropertyChanged += filterInconclusiveOnPropertyChanged;

    		Disposed += (s, e) =>
			{
				filterInconclusiveTestsToolStripMenuItem.Click -= filterInconclusiveTestsToolStripMenuItemOnClick;
				filterInconclusiveTestsToolStripButton.Click -= filterInconclusiveTestsToolStripMenuItemOnClick;
				testExplorerModel.FilterInconclusive.PropertyChanged -= filterInconclusiveOnPropertyChanged;
			};
        }

    	private void FilterStatus(TestStatus testStatus)
        {
            testExplorerController.FilterStatus(testStatus);
        }

        private void SortTree(SortOrder sortOrder)
        {
            sortAscToolStripButton.Checked = (sortOrder == SortOrder.Ascending);
            sortDescToolStripButton.Checked = (sortOrder == SortOrder.Descending);
            testExplorerController.SortTree(sortOrder);
        }

        private void removeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (testTree.SelectedNode == null || !(testTree.SelectedNode.Tag is TestDataNode))
                return;

            var node = (TestDataNode)testTree.SelectedNode.Tag;

            if (node.FileName != null)
            {
                testExplorerController.RemoveFile(node.FileName);
            }
        }

        private void treeViewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var newValue = (string)treeViewComboBox.SelectedItem;

            if (testExplorerModel.CurrentTreeViewCategory == newValue)
                return;

            // if updateFlag is set, then the index has changed because
            // we are populating the list, so no need to refresh!
            if (updateFlag)
                return;

            SaveState();
            testExplorerController.ChangeTreeCategory(newValue, pm => RestoreState());
        }

        private void resetTestsMenuItem_Click(object sender, EventArgs e)
        {
            testExplorerController.ResetTests();
        }

        private void expandAllMenuItem_Click(object sender, EventArgs e)
        {
            testTree.ExpandAll();
        }

        private void collapseAllMenuItem_Click(object sender, EventArgs e)
        {
            testTree.CollapseAll();
        }

        private void viewSourceCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewSourceCode();
        }

        private void ViewSourceCode()
        {
            if (testTree.SelectedNode == null || !(testTree.SelectedNode.Tag is TestTreeNode))
                return;

            var node = (TestTreeNode)testTree.SelectedNode.Tag;

            testExplorerController.ShowSourceCode(node.Id);
        }

        private void expandPassedTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testTree.Expand(TestStatus.Passed);
        }

        private void expandFailedTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testTree.Expand(TestStatus.Failed);
        }

        private void expandInconclusiveTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testTree.Expand(TestStatus.Inconclusive);
        }

        private void addFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = Dialogs.CreateAddFilesDialog())
            {
                if (openFileDialog.ShowDialog(this) != DialogResult.OK)
                    return;

                testExplorerController.AddFiles(openFileDialog.FileNames);
            }
        }

        private void removeAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testExplorerController.RemoveAllFiles();
        }

        private void testTree_SelectionChanged(object sender, EventArgs e)
        {
            SyncContext.Post(cb => TreeSelectionChanged(), null);
        }

        private void TreeSelectionChanged()
        {
            var nodes = new List<TestTreeNode>();

            if (testTree.SelectedNode != null && testTree.SelectedNode.Tag is TestDataNode)
            {
                var testTreeNode = (TestDataNode)testTree.SelectedNode.Tag;

                removeFileToolStripMenuItem.Enabled = testTreeNode.FileName != null;
                viewSourceCodeToolStripMenuItem.Enabled = testTreeNode.SourceCodeAvailable;

                nodes.AddRange(GetSelectedNodes(testTreeNode));
            }
            else
            {
                removeFileToolStripMenuItem.Enabled = false;
                viewSourceCodeToolStripMenuItem.Enabled = false;
            }

            testExplorerController.SetTreeSelection(nodes);
        }

        private static IEnumerable<TestTreeNode> GetSelectedNodes(TestTreeNode testTreeNode)
        {
            var nodes = new List<TestTreeNode>();

            if (testTreeNode is NamespaceNode)
            {
                nodes.AddRange(((NamespaceNode)testTreeNode).GetChildren());
            }
            else
            {
                nodes.Add(testTreeNode);
            }
            return nodes;
        }

        private void SaveState()
        {
            var collapsedNodes = testTree.GetCollapsedNodes();
            testExplorerController.SetCollapsedNodes(collapsedNodes);
        }

        private void RestoreState()
        {
            testTree.CollapseNodes(testExplorerModel.CollapsedNodes.Value);
        }

        private void testTree_DoubleClick(object sender, EventArgs e)
        {
            ViewSourceCode();
        }

        private void selectFailedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testTree.Select(TestStatus.Failed);
        }

        private void selectPassedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testTree.Select(TestStatus.Passed);
        }

        private void selectInconclusiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testTree.Select(TestStatus.Inconclusive);
        }
    }
}
