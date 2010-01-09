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
using Gallio.AutoCAD.Isolation;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Model.Isolation;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;
using Gallio.Tests;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.AutoCAD.Tests.Integration
{
    [Category("Integration")]
    [RunSample(typeof(AcadSampleTest))]
    [Ignore("This fixture appears to be causing the build server to hang!")]
    public class RunAcadSampleTest : BaseTestWithSampleRunner
    {
        [Row(false)]
        [Row(true)]
        public bool AttachDebugger;

        [Test]
        public void RunsTestWithinAutoCAD()
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(AcadSampleTest).GetMethod("Test")));
            Assert.IsNotNull(run, "Test should have run.");
            Assert.AreEqual(TestOutcome.Passed, run.Result.Outcome);
            Assert.Contains(run.TestLog.ToString(), "Running with AutoCAD");
            Assert.Contains(run.TestLog.ToString(), "Process name acad");
        }

        protected override void ConfigureRunner()
        {
            base.ConfigureRunner();

            Runner.TestRunnerFactoryName = "AutoCAD";

            // Suppress the potential attach-to-existing option coming out
            // of user persistent preferences (IAcadPreferenceManager).
            Runner.TestRunnerOptions.AddProperty("AcadAttachToExisting", "false");

            if (AttachDebugger)
            {
                Runner.TestPackage.DebuggerSetup = new DebuggerSetup();
            }
        }
    }
}