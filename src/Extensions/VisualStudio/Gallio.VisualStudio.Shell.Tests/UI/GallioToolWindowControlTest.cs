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
    [TestsOn(typeof(GallioToolWindowControl))]
    public class GallioToolWindowControlTest
    {
        private class MyConcreteGallioToolWindowControl : GallioToolWindowControl
        {
            public MyConcreteGallioToolWindowControl()
            {
            }

            public MyConcreteGallioToolWindowControl(IShell shell)
                : base(shell)
            {
            }
        }

        [Test]
        public void ConstructsByDefault()
        {
            var control = new MyConcreteGallioToolWindowControl();
            Assert.IsNull(control.Shell);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullShell()
        {
            new MyConcreteGallioToolWindowControl(null);
        }

        [Test]
        public void ConstructsWithShellOk()
        {
            var mockShell = MockRepository.GenerateStub<IShell>();
            var control = new MyConcreteGallioToolWindowControl(mockShell);
            Assert.AreSame(mockShell, control.Shell);
        }
    }
}
