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
