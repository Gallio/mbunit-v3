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
using System.Windows.Forms;
using Gallio.Icarus.Properties;

namespace Gallio.Icarus.ProjectProperties
{
    public partial class View : UserControl, IView
    {
        private readonly IController controller;

        public View(IController controller, IModel model)
        {
            this.controller = controller;

            InitializeComponent();

            model.ApplicationBaseDirectory.PropertyChanged += (s, e) =>
                applicationBaseDirectoryTextBox.Text = model.ApplicationBaseDirectory;

            model.HintDirectories.PropertyChanged += (s, e) =>
            {
                hintDirectoriesListBox.Items.Clear();
                hintDirectoriesListBox.Items.AddRange(model.HintDirectories.ToArray());
            };

            model.ReportDirectory.PropertyChanged += (s, e) => reportDirectoryTextBox.Text = model.ReportDirectory;

            model.ReportNameFormat.PropertyChanged += (s, e) => reportNameFormatTextBox.Text = model.ReportNameFormat;

            model.ShadowCopy.PropertyChanged += (s, e) => shadowCopyCheckBox.Checked = model.ShadowCopy;

            model.TestRunnerExtensionSpecifications.PropertyChanged += (s, e) =>
            {
                testRunnerExtensionsListBox.Items.Clear();
                testRunnerExtensionsListBox.Items.AddRange(model.TestRunnerExtensionSpecifications.ToArray());
            };

            model.WorkingDirectory.PropertyChanged += (s, e) => workingDirectoryTextBox.Text = model.WorkingDirectory;
        }

        private void findHintDirectoryButton_Click(object sender, EventArgs e)
        {
            var folder = GetFolder();

            if (false == string.IsNullOrEmpty(folder))
                newHintDirectoryTextBox.Text = folder;
        }

        private static string GetFolder()
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                return folderBrowserDialog.ShowDialog() == DialogResult.OK 
                    ? folderBrowserDialog.SelectedPath : null;
            }
        }

        private void addFolderButton_Click(object sender, EventArgs e)
        {
            try
            {
                controller.AddHintDirectory(newHintDirectoryTextBox.Text);
                newHintDirectoryTextBox.Clear();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                newHintDirectoryTextBox.Focus();
            }
        }

        private void hintDirectoriesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeHintDirectoryButton.Enabled = (hintDirectoriesListBox.SelectedItems.Count > 0);
        }

        private void removeHintDirectoryButton_Click(object sender, EventArgs e)
        {
            var selectedItem = (string)hintDirectoriesListBox.SelectedItem;
            controller.RemoveHintDirectory(selectedItem);
        }

        private void findApplicationBaseDirectoryButton_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    applicationBaseDirectoryTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void findWorkingDirectoryButton_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    workingDirectoryTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void testRunnerExtensionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeExtensionButton.Enabled = (testRunnerExtensionsListBox.SelectedItems.Count > 0);
        }

        private void shadowCopyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            controller.SetShadowCopy(shadowCopyCheckBox.Checked);
        }

        private void addExtensionButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(newExtensionTextBox.Text))
                return;

            try
            {
                controller.AddTestRunnerExtensionSpecification(newExtensionTextBox.Text);
                newExtensionTextBox.Clear();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                newExtensionTextBox.Focus();
            }
        }

        private void removeExtensionButton_Click(object sender, EventArgs e)
        {
            var testRunnerExtension = (string)testRunnerExtensionsListBox.SelectedItem;
            controller.RemoveTestRunnerExtensionSpecification(testRunnerExtension);
        }

        protected override void OnLoad(EventArgs e)
        {
            controller.Load();
        }

        private void applicationBaseDirectoryTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                controller.SetApplicationBaseDirectory(applicationBaseDirectoryTextBox.Text);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                applicationBaseDirectoryTextBox.Focus();
            }
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, Resources.View_Invalid_input, 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void workingDirectoryTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                controller.SetWorkingDirectory(workingDirectoryTextBox.Text);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                workingDirectoryTextBox.Focus();
            }
        }

        private void reportNameFormatTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                controller.SetReportNameFormat(reportNameFormatTextBox.Text);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                reportNameFormatTextBox.Focus();
            }
        }
    }
}
