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
using System.Windows.Forms;
using Gallio.Common.Concurrency;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Icarus
{
    internal partial class AnnotationsWindow : DockWindow
    {
        private readonly IAnnotationsController annotationsController;

        public AnnotationsWindow(IAnnotationsController annotationsController)
        {
            this.annotationsController = annotationsController;
            
            InitializeComponent();

            annotationsController.Annotations.ListChanged += delegate
            {
                Sync.Invoke(this, PopulateListView);
            };

            showErrorsToolStripButton.DataBindings.Add("Checked", annotationsController, "ShowErrors", false, 
                DataSourceUpdateMode.OnPropertyChanged);
            showErrorsToolStripButton.DataBindings.Add("Text", annotationsController, "ErrorsText");

            showWarningsToolStripButton.DataBindings.Add("Checked", annotationsController, "ShowWarnings", false, 
                DataSourceUpdateMode.OnPropertyChanged);
            showWarningsToolStripButton.DataBindings.Add("Text", annotationsController, "WarningsText");

            showInfoToolStripButton.DataBindings.Add("Checked", annotationsController, "ShowInfos", false, 
                DataSourceUpdateMode.OnPropertyChanged);
            showInfoToolStripButton.DataBindings.Add("Text", annotationsController, "InfoText");
        }

        private void PopulateListView()
        {
            annotationsListView.Items.Clear();

            foreach (AnnotationData annotationData in annotationsController.Annotations)
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
            foreach (ListViewItem lvi in annotationsListView.SelectedItems)
                if (ParentForm != null)
                    ((Main)ParentForm).ShowSourceCode(((AnnotationData)lvi.Tag).CodeLocation);
        }

        private static string FilterText(string text)
        {
            return text != null ? text.Replace("\n", " ") : null;
        }
    }
}