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
using System.IO;
using Gallio.Runtime;
using Gallio.Common.Reflection;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Debugging
{
    [TestFixture]
    [TestsOn(typeof(DefaultDebuggerManager))]
    public class DefaultDebuggerManagerTest
    {
        [Test]
        public void GetDebugger_WhenDebuggerSetupIsNull_Throws()
        {
            var manager = new DefaultDebuggerManager();

            Assert.Throws<ArgumentNullException>(() => manager.GetDebugger(null, MockRepository.GenerateStub<ILogger>()));
        }

        [Test]
        public void GetDebugger_WhenLoggerIsNull_Throws()
        {
            var manager = new DefaultDebuggerManager();

            Assert.Throws<ArgumentNullException>(() => manager.GetDebugger(new DebuggerSetup(), null));
        }

        [Test]
        public void GetDebugger_WhenArgumentsValid_ReturnsADebugger()
        {
            var manager = new DefaultDebuggerManager();

            Assert.IsNotNull(manager.GetDebugger(new DebuggerSetup(), MockRepository.GenerateStub<ILogger>()));
        }
    }
}
