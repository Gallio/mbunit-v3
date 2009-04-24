using System;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Utilities;
using Gallio.Reflection;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Controllers
{
    internal class SourceCodeController : ISourceCodeController
    {
        private readonly ITestController testController;

        public event EventHandler<ShowSourceCodeEventArgs> ShowSourceCode;

        public SourceCodeController(ITestController testController)
        {
            this.testController = testController;
        }

        public void ViewSourceCode(string testId, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("View source code", 100))
            {
                CodeLocation codeLocation = CodeLocation.Unknown;
                testController.ReadReport(report =>
                {
                    if (report.TestModel != null)
                    {
                        TestData testData = report.TestModel.GetTestById(testId);
                        if (testData != null)
                            codeLocation = testData.CodeLocation;
                    }
                });

                if (codeLocation == CodeLocation.Unknown || codeLocation.Path.EndsWith(".dll")
                    || codeLocation.Path.EndsWith(".exe"))
                    return;

                // fire event for view
                EventHandlerUtils.SafeInvoke(ShowSourceCode, this, 
                    new ShowSourceCodeEventArgs(codeLocation));
            }
        }
    }
}
