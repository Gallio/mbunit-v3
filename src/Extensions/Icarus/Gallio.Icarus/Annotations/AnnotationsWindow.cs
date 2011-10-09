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
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Model;
using Gallio.Model.Schema;

namespace Gallio.Icarus.Annotations
{
    internal partial class AnnotationsWindow : UserControl
    {
        private readonly IAnnotationsController annotationsController;
        private readonly ISourceCodeController sourceCodeController;

        public AnnotationsWindow(IAnnotationsController annotationsController, 
            ISourceCodeController sourceCodeController)
        {
            this.annotationsController = annotationsController;
            this.sourceCodeController = sourceCodeController;
            
            InitializeComponent();

            annotationsController.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Annotations")
                    PopulateListView();
            };

            showErrorsToolStripButton.Click += (s, e) => annotationsController.ShowErrors(showErrorsToolStripButton.Checked);
            showErrorsToolStripButton.DataBindings.Add("Text", annotationsController, "ErrorsText");

            showWarningsToolStripButton.Click += (s, e) => annotationsController.ShowWarnings(showWarningsToolStripButton.Checked);
            showWarningsToolStripButton.DataBindings.Add("Text", annotationsController, "WarningsText");

            showInfoToolStripButton.Click += (s, e) => annotationsController.ShowInfos(showInfoToolStripButton.Checked);
            showInfoToolStripButton.DataBindings.Add("Text", annotationsController, "InfoText");
        }

        private void PopulateListView()
        {
            annotationsListView.Items.Clear();

            foreach (var annotationData in annotationsController.Annotations)
            {
                switch (annotationData.Type)
                {
                    case AnnotationType.Error:
                        AddListViewItem(annotationData, 0);
                        break;
                    case AnnotationType.Warning:
                        AddListViewItem(annotationData, 1);
                        break;
                    case AnnotationType.Info:
                        AddListViewItem(annotationData, 2);
                        break;
                }
            }

            if (annotationsListView.Items.Count > 0)
                Show();
        }

        private void AddListViewItem(AnnotationData annotationData, int imgIndex)
        {
            var lvi = new ListViewItem(FilterText(annotationData.Message), imgIndex);
            
            lvi.SubItems.AddRange(new[] 
            { 
                FilterText(annotationData.Details), 
                annotationData.CodeLocation.ToString(), 
                annotationData.CodeReference.ToString() 
            });

            lvi.Tag = annotationData;
            annotationsListView.Items.Add(lvi);
        }

        private void annotationsListView_DoubleClick(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in annotationsListView.SelectedItems)
            {
                var codeLocation = ((AnnotationData)listViewItem.Tag).CodeLocation;
                sourceCodeController.ViewSourceCode(codeLocation);
            }
        }

        private static string FilterText(string text)
        {
            return text != null ? text.Replace("\n", " ") : null;
        }
    }
}