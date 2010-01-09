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
using System.Linq;
using System.Text;
using Gallio.VisualStudio.Shell.Core;
using Gallio.VisualStudio.Shell.UI.ToolWindows;
using MbUnit.Framework;
using Microsoft.VisualStudio.Shell;
using Rhino.Mocks;

namespace Gallio.VisualStudio.Shell.Tests.UI
{
    [TestFixture]
    [TestsOn(typeof(ToolWindow))]
    public class ToolWindowTest
    {
        private class MyConcreteShellToolWindowControl : ToolWindow
        {
        }

        [Test]
        public void ConstructsByDefault()
        {
            var control = new MyConcreteShellToolWindowControl();

            Assert.IsNull(control.ToolWindowPane);
        }

        [Test]
        public void ObtainsPaneFromContainer()
        {
            var pane = new ShellToolWindowPane(MockRepository.GenerateStub<IServiceProvider>());
            var container = new ToolWindowContainer(pane);
            var window = new MyConcreteShellToolWindowControl();

            container.ToolWindow = window;

            Assert.AreSame(pane, window.ToolWindowPane);
        }
    }
}
