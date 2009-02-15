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
using System.IO;
using System.Text;
using Gallio.Concurrency;
using Gallio.Runtime;
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Host
{
    [TestFixture]
    public class HostProgramIntegrationTest
    {
        [Test]
        public void HostProgramPrintsHelpUsageWhenHelpOptionSpecified()
        {
            ProcessTask task = StartHost("/help");
            task.Join(TimeSpan.FromSeconds(5));

            Assert.Contains(task.ConsoleOutput, "Available options");
            Assert.AreEqual(0, task.ExitCode);
        }

        [Test]
        public void HostProgramTerminatesAutomaticallyOnTimeoutIfNoConnection()
        {
            ProcessTask task = StartHost("/ipc-port:HostIntegrationTest /timeout:1");
            task.Join(TimeSpan.FromSeconds(5));

            Assert.Contains(task.ConsoleOutput, "* Watchdog timer expired!");
            Assert.AreEqual(0, task.ExitCode);
        }

        [Test]
        public void HostProgramTerminatesImmediatelyIfOwnerProcessDoesNotExist()
        {
            ProcessTask task = StartHost("/ipc-port:HostIntegrationTest /owner-process:100000"); // invalid process id
            task.Join(TimeSpan.FromSeconds(5));

            Assert.Contains(task.ConsoleOutput, "* The owner process with PID 100000 does not appear to be running!");
            Assert.AreEqual(0, task.ExitCode);
        }

        [Test]
        public void HostProgramTerminatesWhenItsOwnerProcessTerminates()
        {
            ProcessTask ownerProcess = StartHost("/ipc-port:HostIntegrationTest-Owner /timeout:30");
            int ownerProcessId = ownerProcess.Process.Id;

            ProcessTask task = StartHost("/ipc-port:HostIntegrationTest /owner-process:" + ownerProcessId);
            Assert.IsFalse(task.Join(TimeSpan.FromSeconds(2)), "The host should not have exited yet.");

            ownerProcess.Abort();
            Assert.IsTrue(task.Join(TimeSpan.FromSeconds(5)), "The host should terminate.");

            Assert.Contains(task.ConsoleOutput, "* Owner process terminated abruptly!");
            Assert.AreEqual(0, task.ExitCode);
        }

        private static ProcessTask StartHost(string arguments)
        {
            return Tasks.StartProcessTask(
                Path.Combine(RuntimeAccessor.RuntimePath, "Gallio.Host.exe"),
                arguments,
                RuntimeAccessor.RuntimePath);
        }
    }
}
