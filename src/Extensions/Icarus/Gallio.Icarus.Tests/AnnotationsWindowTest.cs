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
