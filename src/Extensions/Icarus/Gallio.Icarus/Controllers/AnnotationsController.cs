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

using System.Collections.Generic;
using System.ComponentModel;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Model;
using Gallio.Model.Schema;
using Gallio.UI.Events;

namespace Gallio.Icarus.Controllers
{
    internal class AnnotationsController : NotifyController, IAnnotationsController, 
        Handles<ExploreFinished>
    {
        private readonly ITestController testController;
        private readonly IOptionsController optionsController;
        private List<AnnotationData> annotations = new List<AnnotationData>();
        private bool showErrors, showWarnings, showInfos;

        public IEnumerable<AnnotationData> Annotations
        {
            get
            {
                foreach (var annotation in annotations)
                {
                    if (!FilterAnnotation(annotation))
                        yield return annotation;
                }
            }
        }

        public string ErrorsText
        {
            get
            {
                var errors = Count(AnnotationType.Error);
                return (errors == 1) ? string.Format("{0} Error", errors) : string.Format("{0} Errors", errors);
            }
        }

        public string WarningsText
        {
            get
            {
                var warnings = Count(AnnotationType.Warning);
                return (warnings == 1) ? string.Format("{0} Warning", warnings) : string.Format("{0} Warnings", warnings);
            }
        }

        public string InfoText
        {
            get
            {
                var infos = Count(AnnotationType.Info);
                return (infos == 1) ? string.Format("{0} Info", infos) : string.Format("{0} Infos", infos);
            }
        }

        public AnnotationsController(ITestController testController, IOptionsController optionsController)
        {
            this.testController = testController;
            this.optionsController = optionsController;

            showErrors = optionsController.AnnotationsShowErrors;
            showWarnings = optionsController.AnnotationsShowWarnings;
            showInfos = optionsController.AnnotationsShowInfos;
        }

        public void ShowErrors(bool value)
        {
            showErrors = value;
            optionsController.AnnotationsShowErrors = value;
            UpdateList();
        }

        public void ShowWarnings(bool value)
        {
            showWarnings = value;
            optionsController.AnnotationsShowWarnings = value;
            UpdateList();
        }

        public void ShowInfos(bool value)
        {
            showInfos = value;
            optionsController.AnnotationsShowInfos = value;
            UpdateList();
        }

        private void UpdateList()
        {
            annotations.Clear();
            testController.ReadReport(r => annotations = r.TestModel.Annotations);
            OnPropertyChanged(new PropertyChangedEventArgs("Annotations"));
            OnPropertyChanged(new PropertyChangedEventArgs("ErrorsText"));
            OnPropertyChanged(new PropertyChangedEventArgs("WarningsText"));
            OnPropertyChanged(new PropertyChangedEventArgs("InfoText"));
        }

        private bool FilterAnnotation(AnnotationData annotationData)
        {
            switch (annotationData.Type)
            {
                case AnnotationType.Error:
                    return !showErrors;
                case AnnotationType.Warning:
                    return !showWarnings;
                case AnnotationType.Info:
                    return !showInfos;
            }
            return false;
        }

        private int Count(AnnotationType annotationType)
        {
            var count = 0;
            foreach (var annotation in annotations)
            {
                if (annotation.Type == annotationType)
                    count++;
            }
            return count;
        }

        public void Handle(ExploreFinished @event)
        {
            UpdateList();
        }
    }
}
