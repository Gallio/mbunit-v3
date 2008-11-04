// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using System.Drawing;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Gallio.Icarus.Tests
{
    [Category("Views")]
    class RuntimeLogWindowTest : MockTest
    {
        [Test]
        public void LogMessage_Test()
        {
            IRuntimeLogController runtimeLogController = mocks.StrictMock<IRuntimeLogController>();
            runtimeLogController.LogMessage += null;
            IEventRaiser logMessage = LastCall.IgnoreArguments().GetEventRaiser();
            mocks.ReplayAll();
            RuntimeLogWindow runtimeLogWindow = new RuntimeLogWindow(runtimeLogController);
            logMessage.Raise(runtimeLogController, new RuntimeLogEventArgs("message", Color.Black));
        }
    }
}
