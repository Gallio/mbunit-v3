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
using System.Linq;
using System.Text;
using System.Threading;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;

namespace Gallio.Tests.Framework
{
    [TestsOn(typeof(Tasks))]
    [RunSample(typeof(TasksSample))]
    public class TasksTest : BaseTestWithSampleRunner
    {
        [Test]
        public void WatchTaskThrowsIfTaskIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Tasks.WatchTask(null));
        }

        [Test]
        public void CreateThreadTaskThrowsIfActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Tasks.CreateThreadTask("", null));
        }

        [Test]
        public void StartThreadTaskThrowsIfActionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Tasks.StartThreadTask("", null));
        }

        [Test]
        public void CreateProcessTaskThrowsIfAnyArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Tasks.CreateProcessTask(null, "", Environment.CurrentDirectory));
            Assert.Throws<ArgumentNullException>(() => Tasks.CreateProcessTask("abc.exe", null, "abc"));
            Assert.Throws<ArgumentNullException>(() => Tasks.CreateProcessTask("abc.exe", "", null));
        }

        [Test]
        public void StartProcessTaskThrowsIfAnyArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Tasks.StartProcessTask(null, "", Environment.CurrentDirectory));
            Assert.Throws<ArgumentNullException>(() => Tasks.StartProcessTask("abc.exe", null, "abc"));
            Assert.Throws<ArgumentNullException>(() => Tasks.StartProcessTask("abc.exe", "", null));
        }

        [Test]
        public void NoThreadAbortsBubbledUpToTheFixture()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(TasksSample)));

            Assert.AreEqual(TestOutcome.Passed, run.Result.Outcome);
            Assert.AreEqual("", run.TestLog.ToString());
        }

        [Test]
        public void WhenTestFinishes_WaitsAShortTimeForPendingTasks()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(
                typeof(TasksSample).GetMethod("TaskThatTakesAShortAmountOfTimeToCompleteAfterTheTestFinishes")));

            Assert.AreEqual(TestOutcome.Passed, run.Result.Outcome);
            Assert.AreEqual("*** Log ***\n\nStarted.\nFinished.\n", run.TestLog.ToString());
        }

        [Test]
        public void WhenTestFinishes_AbortsPendingTasksEventually()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(
                typeof(TasksSample).GetMethod("TaskThatTakesAVeryLongAmountOfTimeToCompleteAfterTheTestFinishes")));

            Assert.AreEqual(TestOutcome.Passed, run.Result.Outcome);
            Assert.Contains(run.TestLog.ToString(), "Started");
            Assert.Contains(run.TestLog.ToString(), "Some tasks failed to complete within 3 seconds of test termination: Task");
        }

        [Test]
        public void WhenTestFinishes_FailedTaskIsReported()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(
                typeof(TasksSample).GetMethod("FailedTask")));

            Assert.AreEqual(TestOutcome.Passed, run.Result.Outcome);
            Assert.Contains(run.TestLog.ToString(), "Boom!");
        }

        [Explicit("Sample")]
        public class TasksSample
        {
            [Test]
            public void TaskThatTakesAShortAmountOfTimeToCompleteAfterTheTestFinishes()
            {
                Tasks.StartThreadTask("Task", () => {
                    TestLog.WriteLine("Started.");
                    Thread.Sleep(1000);
                    TestLog.WriteLine("Finished.");
                });
            }

            [Test]
            public void TaskThatTakesAVeryLongAmountOfTimeToCompleteAfterTheTestFinishes()
            {
                Tasks.StartThreadTask("Task", () =>
                {
                    TestLog.WriteLine("Started.");
                    Thread.Sleep(60000);
                    TestLog.WriteLine("Finished.");
                });
            }

            [Test]
            public void FailedTask()
            {
                Tasks.StartThreadTask("Task", delegate { throw new Exception("Boom!"); });
            }
        }
    }
}
