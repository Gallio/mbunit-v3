// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

using Gallio.Core.ProgressMonitoring;
using Gallio.Logging;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// A test controller that emits the description of the test as a runtime error.
    /// </summary>
    public class ErrorTestController : BaseTestController
    {
        /// <inheritdoc />
        public override void RunTests(IProgressMonitor progressMonitor, ITestMonitor rootTestMonitor)
        {
            ITestStepMonitor stepMonitor = rootTestMonitor.StartTestInstance();

            stepMonitor.LogWriter[LogStreamNames.Failures].WriteLine("An error occurred during test enumeration.  Some tests may have been skipped.\n\nReason: {0}",
                rootTestMonitor.Test.Metadata[MetadataKeys.Description]);

            stepMonitor.FinishStep(TestStatus.Error, TestOutcome.Failed, null);
        }
    }
}
