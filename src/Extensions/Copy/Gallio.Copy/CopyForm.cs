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
using System.Reflection;
using System.Windows.Forms;
using Gallio.Common.Reflection;

namespace Gallio.Copy
{
    internal partial class CopyForm : Form
    {
        public CopyForm()
        {
            InitializeComponent();
           
            var pluginFolder = GetSourcePluginFolder();
            sourcePluginTreeView.Model = new PluginTreeModel(pluginFolder);
            sourcePluginFolderTextBox.Text = pluginFolder;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Please choose a destination folder for the copy.";

                if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
                    return;
            }
        }

        // modified version of debug method in DefaultRuntime.
        private static string GetSourcePluginFolder()
        {
            // Find the root "src" dir.
            string initPath = AssemblyUtils.GetAssemblyLocalPath(Assembly.GetExecutingAssembly());

            string srcDir = initPath;
            while (srcDir != null && Path.GetFileName(srcDir) != @"src")
                srcDir = Path.GetDirectoryName(srcDir);

            return srcDir;
       }
    }
}
