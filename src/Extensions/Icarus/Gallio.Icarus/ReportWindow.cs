// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Gallio.Icarus.Interfaces;

namespace Gallio.Icarus
{
    public partial class ReportWindow : DockWindow
    {
        private IProjectAdapterView projectAdapterView;

        public ReportWindow(IProjectAdapterView projectAdapterView)
        {
            this.projectAdapterView = projectAdapterView;
            InitializeComponent();
        }

        public string ReportPath
        {
            set { reportViewer.Url = new Uri(value); }
        }

        public IList<string> ReportTypes
        {
            set
            {
                reportTypes.Items.Clear();
                foreach (string reportType in value)
                    reportTypes.Items.Add(reportType);
                if (value.Count > 0)
                    reportTypes.SelectedIndex = 0;
            }
        }

        private void btnSaveReportAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.OverwritePrompt = true;
            saveFile.AddExtension = true;
            string ext = "All files (*.*)|*.*";
            switch ((string)reportTypes.SelectedItem)
            {
                case "Xml":
                case "Xml-Inline":
                    ext = "XML files (*.xml)|*.xml";
                    break;
                case "Html":
                case "Html-Inline":
                    ext = "HTML files (*.html)|*.html";
                    break;
                case "Xhtml":
                case "Xhtml-Inline":
                    ext = "XHTML files (*.xhtml)|*.xhtml";
                    break;
                case "Text":
                    ext = "Text files (*.txt)|*.txt";
                    break;
            }
            saveFile.DefaultExt = ext;
            saveFile.Filter = ext;
            if (saveFile.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void reportTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSaveReportAs.Enabled = ((string)reportTypes.SelectedItem != "");
        }

        private void reloadReport_Click(object sender, EventArgs e)
        {
            reportViewer.Url = new Uri("about:blank");
            projectAdapterView.CreateReport();
        }
    }
}