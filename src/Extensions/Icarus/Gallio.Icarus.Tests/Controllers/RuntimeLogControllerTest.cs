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
using System.Drawing;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Common.Diagnostics;
using MbUnit.Framework;
using Gallio.Icarus.Controllers;
using Gallio.Runtime.Logging;
using Gallio.Icarus.Controllers.EventArgs;
using System.Collections.Generic;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    public class RuntimeLogControllerTest : RuntimeLogController
    {
        public RuntimeLogControllerTest() : base(MockRepository.GenerateMock<IOptionsController>())
        { }

        // can't use Color.SomeColor in a RowTest :(
        private readonly Dictionary<LogSeverity, Color> colors = new Dictionary<LogSeverity, Color>();

        [FixtureSetUp]
        public void FixtureSetUp()
        {
            colors.Add(LogSeverity.Error, Color.Red);
            colors.Add(LogSeverity.Warning, Color.Gold);
            colors.Add(LogSeverity.Important, Color.Black);
            colors.Add(LogSeverity.Info, Color.Gray);
            colors.Add(LogSeverity.Debug, Color.DarkGray);
        }

        [Test]
        [Row(LogSeverity.Error)]
        [Row(LogSeverity.Warning)]
        [Row(LogSeverity.Important)]
        [Row(LogSeverity.Info)]
        [Row(LogSeverity.Debug)]
        public void LogImpl_Test(LogSeverity logSeverity)
        {
            const string message = "message";
            var flag = false;
            EventHandler<RuntimeLogEventArgs> eh = delegate(object sender, RuntimeLogEventArgs e)
            {
                Assert.AreEqual(message, e.Message);
                Assert.AreEqual(colors[logSeverity], e.Color);
                flag = true;
            };
            LogMessage += eh;
            MinLogSeverity = LogSeverity.Debug;
            LogImpl(logSeverity, message, null);
            Assert.AreEqual(true, flag);
            LogMessage -= eh;
        }

        [Test]
        public void LogImpl_Exception_Test()
        {
            LogSeverity logSeverity = LogSeverity.Error;
            Exception ex = new Exception();
            string message = "message";
            string exceptionMessage = ExceptionUtils.SafeToString(ex);
            bool firstPass = true;
            EventHandler<RuntimeLogEventArgs> eh = delegate(object sender, RuntimeLogEventArgs e)
            {
                if (firstPass)
                {
                    Assert.AreEqual(message, e.Message);
                    firstPass = false;
                }
                else
                {
                    Assert.AreEqual(exceptionMessage, e.Message);
                }
                Assert.AreEqual(colors[logSeverity], e.Color);
            };
            LogMessage += eh;
            LogImpl(logSeverity, message, new ExceptionData(ex));
            LogMessage -= eh;
        }

        [Test]
        public void LogImpl_should_filter_()
        {
            EventHandler<RuntimeLogEventArgs> eh = (sender, e) => Assert.Fail();
            LogMessage += eh;

            MinLogSeverity = LogSeverity.Error;
            LogImpl(LogSeverity.Warning, "message", null);
            
            LogMessage -= eh;
        }
    }
}
