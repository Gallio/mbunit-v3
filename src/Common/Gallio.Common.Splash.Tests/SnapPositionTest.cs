using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace Gallio.Common.Splash.Tests
{
    public class SnapPositionTest
    {
        [Test]
        public void Constructor_SetsProperties()
        {
            var snapPosition = new SnapPosition(SnapKind.Leading, 10);
            Assert.AreEqual(SnapKind.Leading, snapPosition.Kind);
            Assert.AreEqual(10, snapPosition.CharIndex);
        }
    }
}
