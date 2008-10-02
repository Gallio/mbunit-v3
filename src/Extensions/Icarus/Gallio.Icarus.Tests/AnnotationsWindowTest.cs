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

using System.Collections.Generic;
using System.ComponentModel;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests
{
    [MbUnit.Framework.Category("Views")]
    class AnnotationsWindowTest : MockTest
    {
        [Test]
        public void PopulateListView_Test()
        {
            IAnnotationsController annotationsController = mocks.CreateMock<IAnnotationsController>();
            BindingList<AnnotationData> annotations = new BindingList<AnnotationData>(new List<AnnotationData>(new[]
            {
                new AnnotationData(AnnotationType.Warning, CodeLocation.Unknown, new CodeReference(), "message", "details"), 
                new AnnotationData(AnnotationType.Info, CodeLocation.Unknown, new CodeReference(), "message", "details")
            }));
            Expect.Call(annotationsController.Annotations).Return(annotations).Repeat.Twice();
            Expect.Call(annotationsController.ShowErrors).Return(true);
            Expect.Call(annotationsController.ErrorsText).Return("ErrorsText");
            Expect.Call(annotationsController.ShowWarnings).Return(true);
            Expect.Call(annotationsController.WarningsText).Return("WarningsText");
            Expect.Call(annotationsController.ShowInfo).Return(true);
            Expect.Call(annotationsController.InfoText).Return("InfoText");
            mocks.ReplayAll();
            AnnotationsWindow annotationsWindow = new AnnotationsWindow(annotationsController);
            annotations.Add(new AnnotationData(AnnotationType.Error, CodeLocation.Unknown, new CodeReference(), "message", "details"));
        }
    }
}
