// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Drawing;
using System.Windows.Forms;

using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Icarus.Interfaces;

namespace Gallio.Icarus
{
    public partial class AnnotationsWindow : DockWindow
    {
        private readonly IProjectAdapterView projectAdapterView;
        private List<AnnotationData> annotations;
        
        public List<AnnotationData> Annotations
        {
            set
            {
                annotations = value;
                PopulateListView();
            }
        }

        public AnnotationsWindow(IProjectAdapterView projectAdapterView)
        {
            this.projectAdapterView = projectAdapterView;
            InitializeComponent();

            Annotations = new List<AnnotationData>();
        }

        private void PopulateListView()
        {
            annotationsListView.Items.Clear();
            int error = 0, warning = 0, info = 0;
            foreach (AnnotationData annotationData in annotations)
            {
                switch (annotationData.Type)
                {
                    case AnnotationType.Error:
                        if (showErrorsToolStripButton.Checked)
                            AddListViewItem(annotationData, 0);
                        error++;
                        break;
                    case AnnotationType.Warning:
                        if (showWarningsToolStripButton.Checked)
                            AddListViewItem(annotationData, 1);
                        warning++;
                        break;
                    case AnnotationType.Info:
                        if (showInfoToolStripButton.Checked)
                            AddListViewItem(annotationData, 2);
                        info++;
                        break;
                }
            }
            showErrorsToolStripButton.Text = string.Format("{0} Errors", error);
            showWarningsToolStripButton.Text = string.Format("{0} Warnings", warning);
            showInfoToolStripButton.Text = string.Format("{0} Info", info);
        }

        private void AddListViewItem(AnnotationData annotationData, int imgIndex)
        {
            ListViewItem lvi = new ListViewItem(annotationData.Message, imgIndex);
            lvi.SubItems.AddRange(new string[] { annotationData.Details, annotationData.CodeLocation.ToString(), 
                        annotationData.CodeReference.ToString() });
            lvi.Tag = annotationData;
            annotationsListView.Items.Add(lvi);
        }

        private void annotationsToolStripButton_Click(object sender, EventArgs e)
        {
            PopulateListView();
        }

        private void annotationsListView_DoubleClick(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in annotationsListView.SelectedItems)
                projectAdapterView.SourceCodeLocation = ((AnnotationData)lvi.Tag).CodeLocation;
        }
    }
}