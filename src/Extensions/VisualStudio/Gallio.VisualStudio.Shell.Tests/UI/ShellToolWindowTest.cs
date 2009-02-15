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
using Gallio.VisualStudio.Shell.UI;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.VisualStudio.Shell.Tests.UI
{
    [TestFixture]
    [TestsOn(typeof(ShellToolWindow))]
    public class ShellToolWindowTest
    {
        private class MyConcreteShellToolWindowControl : ShellToolWindow
        {
        }

        [Test]
        public void ConstructsByDefault()
        {
            var control = new MyConcreteShellToolWindowControl();
            Assert.IsNull(control.Shell);
            Assert.IsNull(control.ToolWindowPane);
            Assert.IsNull(control.ToolWindowContainer);
        }

        [Test]
        public void ObtainsShellAndPaneFromContainer()
        {
            var pane = new ShellToolWindowPane(MockRepository.GenerateStub<IShell>());
            var container = new ShellToolWindowContainer() { ToolWindowPane = pane };
            var window = new MyConcreteShellToolWindowControl();

            container.ToolWindow = window;

            Assert.AreSame(pane.Shell, window.Shell);
            Assert.AreSame(container, window.ToolWindowContainer);
            Assert.AreSame(pane, window.ToolWindowPane);
        }
    }
}
