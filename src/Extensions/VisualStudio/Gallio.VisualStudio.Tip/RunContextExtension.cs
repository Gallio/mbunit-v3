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
using System.Collections.Generic;
using Gallio.Model.Schema;
using Gallio.Runner.Events;
using Gallio.Runner.Extensions;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Execution;

namespace Gallio.VisualStudio.Tip
{
    internal class RunContextExtension : TestRunnerExtension
    {
        private readonly IRunContext runContext;
        private readonly Dictionary<string, GallioTestElement> testElementsById;

        public RunContextExtension(IRunContext runContext)
        {
            this.runContext = runContext;

            testElementsById = new Dictionary<string, GallioTestElement>();
        }

        protected override void Initialize()
        {
            foreach (ITestElement testElement in runContext.RunConfig.TestElements)
            {
                GallioTestElement gallioTestElement = testElement as GallioTestElement;
                if (gallioTestElement != null)
                    testElementsById.Add(gallioTestElement.GallioTestId, gallioTestElement);
            }

            Events.RunStarted += delegate(object sender, RunStartedEventArgs e)
            {
                // Change the status of all tests to Started so that all Gallio tests look "In Progress".
                // If we didn't do this, then there would be one "In Progress" test (the first one started)
                // and a whole bunch of "Pending" tests.  Visual Studio assumes that it controls the order
                // of execution of all tests but it cannot.  Behind the scenes we hijack the order of execution
                // when Visual Studio starts the first test.  Of course that test might not actually run
                // first but it will seem to be "In Progress" just the same.  Instead of misleading the user
                // as to which test is currently running, we just make them all look "In Progress" at once.  Ugh.
                foreach (GallioTestElement gallioTestElement in testElementsById.Values)
                {
                    TestStateEvent ev = new TestStateEvent(runContext.RunConfig.TestRun.Id, gallioTestElement.ExecutionId.Id, TestState.Started);
                    runContext.ResultSink.AddResult(ev);
                }
            };

            Events.TestStepFinished += delegate(object sender, TestStepFinishedEventArgs e)
            {
                // Submit a GallioTestResult for each primary run of a test case.
                // In the case of data-driven tests, we may submit multiple results that will later be merged.
                if (e.TestStepRun.Step.IsPrimary)
                {
                    GallioTestElement gallioTestElement = GetTestElement(e.Test);
                    if (gallioTestElement != null)
                    {
                        GallioTestResult result = GallioTestResultFactory.CreateTestResult(e.TestStepRun, runContext.RunConfig.TestRun.Id, gallioTestElement);
                        runContext.ResultSink.AddResult(result);
                    }
                }
            };
        }

        private GallioTestElement GetTestElement(TestData test)
        {
            GallioTestElement result;
            testElementsById.TryGetValue(test.Id, out result);
            return result;
        }
    }
}
