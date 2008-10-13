using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MbUnit.Framework;
using Gallio.VisualStudio.Shell.UI;
using Rhino.Mocks;

namespace Gallio.VisualStudio.Shell.Tests.UI
{
    [TestFixture]
    [TestsOn(typeof(ShellToolWindowContainer))]
    public class ShellToolWindowContainerTest
    {
        private class MyConcreteToolWindow : ShellToolWindow
        {
        }

        [Test]
        public void SetNullControl()
        {
            var container = new ShellToolWindowContainer();
            container.ToolWindow = null;

            Assert.IsNull(container.ToolWindow);
            Assert.AreEqual(0, container.Controls.Count);
        }

        [Test]
        public void SetControlOk()
        {
            var pane = new ShellToolWindowPane(MockRepository.GenerateStub<IShell>());
            var container = new ShellToolWindowContainer() { ToolWindowPane = pane };
            var control = new MyConcreteToolWindow();
            container.ToolWindow = control;

            Assert.AreSame(control, container.ToolWindow);
            Assert.IsTrue(container.Controls.Contains(control));
            Assert.AreEqual(DockStyle.Fill, control.Dock);
        }
    }
}
