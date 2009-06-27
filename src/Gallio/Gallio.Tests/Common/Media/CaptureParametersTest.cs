using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Gallio.Common.Markup;
using Gallio.Common.Media;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Media
{
    [TestsOn(typeof(CaptureParameters))]
    public class CaptureParametersTest
    {
        [Test]
        public void Zoom_CanGetAndSetValue()
        {
            var parameters = new CaptureParameters();

            Assert.AreEqual(1.0, parameters.Zoom);

            parameters.Zoom = 1.0 / 16;
            Assert.AreEqual(1.0 / 16, parameters.Zoom);

            parameters.Zoom = 16;
            Assert.AreEqual(16, parameters.Zoom);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Zoom = 1.0 / 17);
            Assert.Contains(ex.Message, "The zoom factor must be between 1/16 and 16.");

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Zoom = 17);
            Assert.Contains(ex.Message, "The zoom factor must be between 1/16 and 16.");
        }
    }
}
