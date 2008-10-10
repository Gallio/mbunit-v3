using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using Gallio.VisualStudio.Shell.UI;

namespace Gallio.VisualStudio.Shell.Tests.UI
{
    [TestFixture]
    [TestsOn(typeof(GallioToolWindowControlContainer))]
    public class GallioToolWindowControlContainerTest
    {
        private class MyConcreteGallioToolWindowControl : GallioToolWindowControl
        {
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SetNullControl()
        {
            new GallioToolWindowControlContainer().SetControl(null);
        }

        [Test]
        public void SetControlOk()
        {
            var container = new GallioToolWindowControlContainer();
            var control = new MyConcreteGallioToolWindowControl();
            container.SetControl(control);
            Assert.IsTrue(container.Controls.Contains(control));
        }

    }
}
