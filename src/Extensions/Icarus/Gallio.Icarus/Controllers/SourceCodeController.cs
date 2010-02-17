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
using Gallio.Common.Policies;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Common.Reflection;

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
                var codeLocation = CodeLocation.Unknown;
                testController.ReadReport(report =>
                {
                    if (report.TestModel == null) 
                        return;

                    var testData = report.TestModel.GetTestById(testId);
                    if (testData != null)
                        codeLocation = testData.CodeLocation;
                });

                if (CanDisplay(codeLocation))
                    ViewSourceCode(codeLocation);
            }
        }

        private static bool CanDisplay(CodeLocation codeLocation) {
            if (codeLocation == CodeLocation.Unknown)
                return false;
            
            var path = codeLocation.Path.ToLower();
            if (path.EndsWith(".dll") || path.EndsWith(".exe"))
                return false;

            return true;
        }

        public void ViewSourceCode(CodeLocation codeLocation)
        {
            // fire event for view
            EventHandlerPolicy.SafeInvoke(ShowSourceCode, this,
                new ShowSourceCodeEventArgs(codeLocation));
        }
    }
}
