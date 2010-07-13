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
using System.Diagnostics;
using Gallio.AutoCAD.Commands;
using Gallio.AutoCAD.ProcessManagement;
using Gallio.Common.Concurrency;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.AutoCAD.Tests.ProcessManagement
{
    [TestsOn(typeof(CreatedAcadProcess))]
    public class CreatedAcadProcessTest
    {
        [Test]
        public void Constructor_SetsFileName()
        {
            var logger = MockRepository.GenerateStub<ILogger>();
            var commandRunner = MockRepository.GenerateStub<IAcadCommandRunner>();
            var processCreator = MockRepository.GenerateStub<IProcessCreator>();
            var debuggerManager = MockRepository.GenerateStub<IDebuggerManager>();

            var acadProcess = new CreatedAcadProcess(logger, commandRunner, @"c:\path\to\acad.exe", processCreator, debuggerManager);
            Assert.AreEqual(@"c:\path\to\acad.exe", acadProcess.FileName);
        }

        public class Start
        {
            private IProcess actualProcess;
            private IProcessCreator processCreator;
            private CreatedAcadProcess acadProcess;

            [SetUp]
            public void SetUp()
            {
                var logger = MockRepository.GenerateStub<ILogger>();
                var commandRunner = MockRepository.GenerateStub<IAcadCommandRunner>();
                var debuggerManager = MockRepository.GenerateStub<IDebuggerManager>();
                actualProcess = MockRepository.GenerateStub<IProcess>();
                processCreator = MockRepository.GenerateStub<IProcessCreator>();
                processCreator.Stub(x => x.Start(Arg<ProcessStartInfo>.Is.Anything)).Return(actualProcess);

                acadProcess = new CreatedAcadProcess(logger, commandRunner, @"c:\path\to\acad.exe", processCreator, debuggerManager);
            }

            [Test]
            public void WhenActualProcessHasExited_ThrowsInvalidOperationException()
            {
                actualProcess.Stub(x => x.HasExited).Return(true);

                Assert.Throws<InvalidOperationException>(() => acadProcess.Start("IPC port name", Guid.Empty, null));
            }

            [Test]
            public void WhenUnableToLoad_ThrowsTimeoutException()
            {
                acadProcess.ReadyPollInterval = TimeSpan.Zero;
                acadProcess.ReadyTimeout = TimeSpan.FromTicks(1);
                actualProcess.Stub(x => x.IsModuleLoaded(Arg<string>.Is.Anything)).Return(false);
                
                Assert.Throws<TimeoutException>(() => acadProcess.Start("IPC port name", Guid.Empty, null));
            }

            public class PassesToProcessCreator
            {
                private IProcessCreator processCreator;
                private IProcess actualProcess;
                private CreatedAcadProcess acadProcess;

                [SetUp]
                public void SetUp()
                {
                    var logger = MockRepository.GenerateStub<ILogger>();
                    var commandRunner = MockRepository.GenerateStub<IAcadCommandRunner>();
                    var debuggerManager = MockRepository.GenerateStub<IDebuggerManager>();
                    processCreator = MockRepository.GenerateMock<IProcessCreator>();
                    actualProcess = MockRepository.GenerateStub<IProcess>();
                    actualProcess.Stub(x => x.IsModuleLoaded(Arg<string>.Is.Anything)).Return(true);

                    acadProcess = new CreatedAcadProcess(logger, commandRunner, @"c:\path\to\acad.exe", processCreator, debuggerManager);
                }

                [Test]
                public void CommandLineArguments()
                {
                    acadProcess.Arguments = "command line args";
                    CallStartAndAssert(x => x.Arguments, "command line args");
                }

                [Test]
                public void FileName()
                {
                    CallStartAndAssert(x => x.FileName, @"c:\path\to\acad.exe");
                }

                [Test]
                public void WorkingDirectory()
                {
                    acadProcess.WorkingDirectory = @"c:\working\dir\";
                    CallStartAndAssert(x => x.WorkingDirectory, @"c:\working\dir\");
                }

                private void CallStartAndAssert<T>(Converter<ProcessStartInfo, T> convert, T expected)
                {
                    processCreator
                        .Expect(x => x.Start(Arg<ProcessStartInfo>.Is.Anything))
                        .Return(actualProcess)
                        .Callback((ProcessStartInfo p) =>
                        {
                            Assert.AreEqual(convert(p), expected);
                            return true;
                        });

                    acadProcess.Start("IPC port name", Guid.Empty, null);

                    processCreator.VerifyAllExpectations();
                }
            }
        }

        public class Dispose
        {
            private IProcess actualProcess;
            private IProcessCreator processCreator;
            private CreatedAcadProcess acadProcess;

            [SetUp]
            public void SetUp()
            {
                var logger = MockRepository.GenerateStub<ILogger>();
                var commandRunner = MockRepository.GenerateStub<IAcadCommandRunner>();
                var debuggerManager = MockRepository.GenerateStub<IDebuggerManager>();

                actualProcess = MockRepository.GenerateMock<IProcess>();
                actualProcess.Stub(x => x.IsModuleLoaded(Arg<string>.Is.Anything)).Return(true);

                processCreator = MockRepository.GenerateStub<IProcessCreator>();
                processCreator.Stub(x => x.Start(Arg<ProcessStartInfo>.Is.Anything)).Return(actualProcess);

                acadProcess = new CreatedAcadProcess(logger, commandRunner, @"c:\path\to\acad.exe", processCreator, debuggerManager);
            }

            [Test]
            public void WhenCalledAfterStart_CallsKillAndDisposeOnActualProcess()
            {
                acadProcess.Start("IPC port name", Guid.Empty, null);

                acadProcess.Dispose();

                actualProcess.AssertWasCalled(x => x.Kill());
                actualProcess.AssertWasCalled(x => x.Dispose());
            }

            [Test]
            public void WhenCalledBeforeStart_DoesNotThrow()
            {
                Assert.DoesNotThrow(acadProcess.Dispose);
            }

            [Test]
            public void WhenCalledAndActualProcessHasAlreadyExited_CallsDisposeButNotKill()
            {
                acadProcess.Start("IPC port name", Guid.Empty, null);
                actualProcess.Stub(x => x.HasExited).Return(true);

                acadProcess.Dispose();

                actualProcess.AssertWasNotCalled(x => x.Kill());
                actualProcess.AssertWasCalled(x => x.Dispose());
            }
        }
    }
}
