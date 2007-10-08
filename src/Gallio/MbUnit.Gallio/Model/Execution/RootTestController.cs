// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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

using System;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Model;

namespace MbUnit.Model.Execution
{
    /// <summary>
    /// A test controller for a root test.  Recursively enters contexts
    /// until a master test is found then delegates control over to the
    /// <see cref="ITestController" /> produced by the master test.
    /// </summary>
    public class RootTestController : ITestController
    {
        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public void RunTests(IProgressMonitor progressMonitor, ITestMonitor rootTestMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");
            if (rootTestMonitor == null)
                throw new ArgumentNullException(@"rootTestMonitor");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Running tests.", rootTestMonitor.TestCount);

                RunTestMonitor(progressMonitor, rootTestMonitor);
            }
        }

        private static void RunTestMonitor(IProgressMonitor progressMonitor, ITestMonitor testMonitor)
        {
            using (ITestController controller = testMonitor.Test.CreateTestController())
            {
                if (controller != null)
                {
                    // Delegate to the associated controller, if present.
                    controller.RunTests(new SubProgressMonitor(progressMonitor, testMonitor.TestCount), testMonitor);
                }
                else
                {
                    // Enter the scope of the test and recursve until we find a controller.
                    progressMonitor.SetStatus(String.Format("Run test: {0}.", testMonitor.Test.Name));

                    IStepMonitor stepMonitor = testMonitor.StartRootStep();
                    try
                    {
                        foreach (ITestMonitor monitor in testMonitor.Children)
                            RunTestMonitor(progressMonitor, monitor);
                    }
                    finally
                    {
                        stepMonitor.FinishStep(TestStatus.Executed, TestOutcome.Passed, null);

                        progressMonitor.Worked(1);
                    }
                }
            }
        }
    }
}
