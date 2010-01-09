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
using System.Drawing;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Common.Diagnostics;
using MbUnit.Framework;
using Gallio.Icarus.Controllers;
using Gallio.Runtime.Logging;
using Gallio.Icarus.Controllers.EventArgs;
using System.Collections.Generic;
using Rhino.Mocks;
using Gallio.Icarus.Logging;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers"), TestsOn(typeof(RuntimeLogController))]
    internal class RuntimeLogControllerTest
    {
        [Test]
        public void MinLogSeverity_should_come_from_logger()
        {
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var runtimeLogController = new RuntimeLogController(optionsController);
            var runtimeLogger = MockRepository.GenerateStub<IRuntimeLogger>();
            runtimeLogController.SetLogger(runtimeLogger);
            runtimeLogger.MinLogSeverity = LogSeverity.Error;

            Assert.AreEqual(LogSeverity.Error, runtimeLogController.MinLogSeverity);
            runtimeLogController.MinLogSeverity = LogSeverity.Important;
            Assert.AreEqual(LogSeverity.Important, runtimeLogger.MinLogSeverity);
        }

        [Test]
        public void Setting_MinLogSeverity_should_save_options()
        {
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var runtimeLogController = new RuntimeLogController(optionsController);
            var runtimeLogger = MockRepository.GenerateStub<IRuntimeLogger>();
            runtimeLogController.SetLogger(runtimeLogger);

            runtimeLogController.MinLogSeverity = LogSeverity.Important;
            Assert.AreEqual(LogSeverity.Important, optionsController.MinLogSeverity);
            optionsController.AssertWasCalled(oc => oc.Save());
        }

        [Test]
        public void MinLogSeverity_should_be_restored_from_options()
        {
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            optionsController.MinLogSeverity = LogSeverity.Info;
            var runtimeLogController = new RuntimeLogController(optionsController);
            var runtimeLogger = MockRepository.GenerateStub<IRuntimeLogger>();
            runtimeLogController.SetLogger(runtimeLogger);

            Assert.AreEqual(LogSeverity.Info, runtimeLogController.MinLogSeverity);
        }

        [Test]
        public void LogMessage_should_bubble_up_from_logger()
        {
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var runtimeLogController = new RuntimeLogController(optionsController);
            var runtimeLogger = MockRepository.GenerateStub<IRuntimeLogger>();
            runtimeLogController.SetLogger(runtimeLogger);
            bool logMessageFlag = false;
            var eventArgs = new RuntimeLogEventArgs("message", Color.Red);
            runtimeLogController.LogMessage += (sender, e) =>
            {
                Assert.AreEqual(eventArgs, e);
                logMessageFlag = true;
            };

            runtimeLogger.Raise(rl => rl.LogMessage += null, runtimeLogger, eventArgs);
            Assert.AreEqual(true, logMessageFlag);
        }
    }
}
