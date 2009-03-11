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
using System.ComponentModel;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Controllers
{
    class AnnotationsController : IAnnotationsController
    {
        private readonly ITestController testController;
        private readonly List<AnnotationData> annotationsList = new List<AnnotationData>();
        private readonly BindingList<AnnotationData> annotations;
        private bool showErrors = true, showWarnings = true, showInfo = true;

        public BindingList<AnnotationData> Annotations
        {
            get { return annotations; }
        }

        public bool ShowErrors
        {
            get { return showErrors; }
            set
            {
                showErrors = value;
                UpdateList();
            }
        }

        public bool ShowWarnings
        {
            get { return showWarnings; }
            set
            {
                showWarnings = value;
                UpdateList();
            }
        }

        public bool ShowInfo
        {
            get { return showInfo; }
            set
            {
                showInfo = value;
                UpdateList();
            }
        }

        public string ErrorsText { get; private set; }

        public string WarningsText { get; private set; }

        public string InfoText { get; private set; }

        public AnnotationsController(ITestController testController)
        {
            this.testController = testController;
            annotations = new BindingList<AnnotationData>(annotationsList);
            testController.ExploreFinished += delegate { UpdateList(); };
        }

        private void UpdateList()
        {
            annotations.Clear();
            int error = 0, warning = 0, info = 0;
            testController.ReadReport(report =>
            {
                foreach (AnnotationData annotationData in report.TestModel.Annotations)
                {
                    switch (annotationData.Type)
                    {
                        case AnnotationType.Error:
                            if (showErrors)
                                annotations.Add(annotationData);
                            error++;
                            break;
                        case AnnotationType.Warning:
                            if (showWarnings)
                                annotations.Add(annotationData);
                            warning++;
                            break;
                        case AnnotationType.Info:
                            if (showInfo)
                                annotations.Add(annotationData);
                            info++;
                            break;
                    }
                }
                ErrorsText = (error == 1) ? string.Format("{0} Error", error) : string.Format("{0} Errors", error);
                WarningsText = (warning == 1) ? string.Format("{0} Warning", warning): string.Format("{0} Warnings", warning);
                InfoText = (info == 1) ? string.Format("{0} Info", info) : string.Format("{0} Infos", info);
            });
        }
    }
}
