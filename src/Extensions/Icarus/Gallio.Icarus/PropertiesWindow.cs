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
using System.IO;
using System.Windows.Forms;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runner;
using Gallio.Runner.Extensions;

namespace Gallio.Icarus
{
    public partial class PropertiesWindow : DockWindow
    {
        private readonly IProjectController projectController;

        public PropertiesWindow(IProjectController projectController)
        {
            this.projectController = projectController;
            InitializeComponent();
            hintDirectoriesListBox.DataSource = projectController.HintDirectories;
            applicationBaseDirectoryTextBox.DataBindings.Add("Text", projectController,
                "TestPackageConfig.HostSetup.ApplicationBaseDirectory", false, DataSourceUpdateMode.OnPropertyChanged);
            workingDirectoryTextBox.DataBindings.Add("Text", projectController,
                "TestPackageConfig.HostSetup.WorkingDirectory", false, DataSourceUpdateMode.OnPropertyChanged);
            shadowCopyCheckBox.DataBindings.Add("Checked", projectController, 
                "TestPackageConfig.HostSetup.ShadowCopy", false, DataSourceUpdateMode.OnPropertyChanged);
            testRunnerExtensionsListBox.DataSource = projectController.TestRunnerExtensions;
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

        private static void DisplayInvalidFolderMessage()
        {
            // TODO: move to resources for localisation
            string message = "Folder path does not exist." + Environment.NewLine + "Please select a valid folder path.";
            const string title = "Invalid folder path";
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void findWorkingDirectoryButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                workingDirectoryTextBox.Text = folderBrowserDialog.SelectedPath;
        }

        private void addExtensionButton_Click(object sender, EventArgs e)
        {
            try
            {
                TestRunnerExtensionUtils.CreateExtensionFromSpecification(newExtensionTextBox.Text);
                projectController.TestRunnerExtensions.Add(newExtensionTextBox.Text);
                newExtensionTextBox.Text = string.Empty;
            }
            catch (RunnerException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void removeExtensionButton_Click(object sender, EventArgs e)
        {
            projectController.TestRunnerExtensions.Remove((string)testRunnerExtensionsListBox.SelectedItem);
        }

        private void testRunnerExtensionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeExtensionButton.Enabled = (testRunnerExtensionsListBox.SelectedItems.Count > 0);
        }
    }
}
