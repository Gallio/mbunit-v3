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
