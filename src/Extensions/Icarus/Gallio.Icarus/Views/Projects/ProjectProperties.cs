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
using System.IO;
using System.Windows.Forms;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runner;
using Gallio.Runner.Extensions;
using Gallio.UI.ErrorReporting;

namespace Gallio.Icarus.Views.Projects
{
    public partial class ProjectProperties : UserControl
    {
        private readonly IProjectController projectController;

        public ProjectProperties(IProjectController projectController)
        {
            this.projectController = projectController;
            
            InitializeComponent();

            hintDirectoriesListBox.DataSource = projectController.HintDirectories;
            applicationBaseDirectoryTextBox.DataBindings.Add("Text", projectController.TestPackage,
                "ApplicationBaseDirectory", false, DataSourceUpdateMode.OnPropertyChanged);
            workingDirectoryTextBox.DataBindings.Add("Text", projectController.TestPackage,
                "WorkingDirectory", false, DataSourceUpdateMode.OnPropertyChanged);
            shadowCopyCheckBox.DataBindings.Add("Checked", projectController.TestPackage,
                "ShadowCopy", false, DataSourceUpdateMode.OnPropertyChanged);
            testRunnerExtensionsListBox.DataSource = projectController.TestRunnerExtensions;
            reportDirectoryTextBox.Text = projectController.ReportDirectory;
            reportNameFormatTextBox.DataBindings.Add("Text", projectController, "ReportNameFormat", 
                false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void findHintDirectoryButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                newHintDirectoryTextBox.Text = folderBrowserDialog.SelectedPath;
        }

        private void addFolderButton_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(newHintDirectoryTextBox.Text))
                projectController.HintDirectories.Add(newHintDirectoryTextBox.Text);
            else
                DisplayInvalidFolderMessage();
        }

        private void hintDirectoriesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeHintDirectoryButton.Enabled = (hintDirectoriesListBox.SelectedItems.Count > 0);
        }

        private void removeHintDirectoryButton_Click(object sender, EventArgs e)
        {
            projectController.HintDirectories.Remove((string)hintDirectoriesListBox.SelectedItem);
        }

        private void findApplicationBaseDirectoryButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                applicationBaseDirectoryTextBox.Text = folderBrowserDialog.SelectedPath;
        }

        private void DisplayInvalidFolderMessage()
        {
            // TODO: move to resources for localisation
            string message = "Folder path does not exist." + Environment.NewLine + "Please select a valid folder path.";
            const string title = "Invalid Folder Path";
            ErrorDialog.Show(this, title, message, "");
        }

        private void findWorkingDirectoryButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                workingDirectoryTextBox.Text = folderBrowserDialog.SelectedPath;
        }

        private void testRunnerExtensionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeExtensionButton.Enabled = (testRunnerExtensionsListBox.SelectedItems.Count > 0);
        }
    }
}
