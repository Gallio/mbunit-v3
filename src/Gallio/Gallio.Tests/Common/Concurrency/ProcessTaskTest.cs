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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Gallio.Common.Concurrency;
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Concurrency
{
    [TestsOn(typeof(ProcessTask))]
    [TestFixture]
    public class ProcessTaskTest
    {
        public class WhenProcessCompletes
        {
            private const int COR_E_STACKOVERFLOW = unchecked((int)0x800703E9L);

            private ProcessTask processTask;
            private ManualResetEvent terminatedSuccessfully;

            [FixtureSetUp]
            public void SetUp()
            {
                terminatedSuccessfully = new ManualResetEvent(false);

                processTask = Tasks.StartProcessTask(Path.Combine(Environment.SystemDirectory, "cmd.exe"),
                    "/C exit " + COR_E_STACKOVERFLOW, Environment.CurrentDirectory);
                processTask.Terminated += processTask_Terminated;
                processTask.Start();
                Assert.IsTrue(processTask.Join(TimeSpan.FromSeconds(10)), "Wait for exit");
            }

            private void processTask_Terminated(object sender, TaskEventArgs e)
            {
                // Verify that everything is correctly set in the callback.
                TaskFlagsAreCorrect();
                ExitCodeIsSet();
                ExitCodeDescriptionIsSet();

                terminatedSuccessfully.Set();
            }

            [Test]
            public void TaskFlagsAreCorrect()
            {
                Assert.IsTrue(processTask.IsTerminated, "Terminated");
                Assert.IsFalse(processTask.IsRunning, "Not Running");
                Assert.IsFalse(processTask.IsPending, "Not Pending");
                Assert.IsFalse(processTask.IsAborted, "Not Aborted");
            }

            [Test]
            public void ExitCodeIsSet()
            {
                Assert.AreEqual(COR_E_STACKOVERFLOW, processTask.ExitCode);
            }

            [Test]
            public void ExitCodeDescriptionIsSet()
            {
                Assert.Contains(processTask.ExitCodeDescription, "stack overflow");
            }

            [Test]
            public void ResultObjectIsSet()
            {
                Assert.AreEqual(COR_E_STACKOVERFLOW, processTask.Result.Value);
            }

            [Test]
            public void TerminatedEventIsFired()
            {
                Assert.IsTrue(terminatedSuccessfully.WaitOne(1000), "Terminated successfully.");
            }
        }
    }
}
