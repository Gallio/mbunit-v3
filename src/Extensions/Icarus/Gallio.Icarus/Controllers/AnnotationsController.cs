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
    internal class AnnotationsController : NotifyController, IAnnotationsController
    {
        private readonly ITestController testController;
        private readonly IOptionsController optionsController;
        private readonly List<AnnotationData> annotationsList = new List<AnnotationData>();
        private readonly BindingList<AnnotationData> annotations;
        private bool showErrors = true, showWarnings = true, showInfos = true;
        private int errors, warnings, infos;

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
                optionsController.AnnotationsShowErrors = value;
                UpdateList();
            }
        }

        public bool ShowWarnings
        {
            get { return showWarnings; }
            set
            {
                showWarnings = value;
                optionsController.AnnotationsShowWarnings = value;
                UpdateList();
            }
        }

        public bool ShowInfos
        {
            get { return showInfos; }
            set
            {
                showInfos = value;
                optionsController.AnnotationsShowInfos = value;
                UpdateList();
            }
        }

        public string ErrorsText
        {
            get { return (errors == 1) ? string.Format("{0} Error", errors) : string.Format("{0} Errors", errors); }
        }

        public string WarningsText
        {
            get { return (warnings == 1) ? string.Format("{0} Warning", warnings) : string.Format("{0} Warnings", warnings); }
        }

        public string InfoText
        {
            get { return (infos == 1) ? string.Format("{0} Info", infos) : string.Format("{0} Infos", infos); }
        }

        public AnnotationsController(ITestController testController, IOptionsController optionsController)
        {
            this.testController = testController;
            this.optionsController = optionsController;

            showErrors = optionsController.AnnotationsShowErrors;
            showWarnings = optionsController.AnnotationsShowWarnings;
            showInfos = optionsController.AnnotationsShowInfos;

            annotations = new BindingList<AnnotationData>(annotationsList);
            testController.ExploreFinished += delegate { UpdateList(); };
        }

        private void UpdateList()
        {
            annotations.Clear();
            errors = warnings = infos = 0;
            testController.ReadReport(report =>
            {
                foreach (AnnotationData annotationData in report.TestModel.Annotations)
                {
                    switch (annotationData.Type)
                    {
                        case AnnotationType.Error:
                            if (showErrors)
                                annotations.Add(annotationData);
                            errors++;
                            break;
                        case AnnotationType.Warning:
                            if (showWarnings)
                                annotations.Add(annotationData);
                            warnings++;
                            break;
                        case AnnotationType.Info:
                            if (showInfos)
                                annotations.Add(annotationData);
                            infos++;
                            break;
                    }
                }
                OnPropertyChanged(new PropertyChangedEventArgs("ErrorsText"));
                OnPropertyChanged(new PropertyChangedEventArgs("WarningsText"));
                OnPropertyChanged(new PropertyChangedEventArgs("InfoText"));
            });
        }
    }
}
