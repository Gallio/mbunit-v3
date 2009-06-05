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
using System.Windows.Forms;
using Gallio.Common.Policies;

namespace Gallio.Copy
{
    internal partial class CopyForm : Form
    {
        private readonly ICopyController copyController;

        public CopyForm(ICopyController copyController)
        {
            this.copyController = copyController;

            InitializeComponent();

            foreach (var plugin in copyController.Plugins)
                pluginsListView.Items.Add(plugin);

            UnhandledExceptionPolicy.ReportUnhandledException += (sender, e) => MessageBox.Show(this, 
                e.GetDescription(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void closeButton_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void copyButton_Click(object sender, System.EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
                    return;

                var list = new List<string>();
                foreach (ListViewItem item in pluginsListView.CheckedItems)
                    list.Add(item.Text);

                copyController.CopyTo(folderBrowserDialog.SelectedPath, list);
            }
        }
    }
}
