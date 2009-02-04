using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework.Pattern;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    public class TimeoutAttributeTest
    {
        [Test]
        public void ZeroTimeoutMeansInfinite()
        {
            var attrib = new TimeoutAttribute(0);
            Assert.AreEqual(0, attrib.TimeoutSeconds);
            Assert.IsNull(attrib.Timeout);
        }

        [Test]
        public void DisallowsNegativeTimeout()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeoutAttribute(-1));
        }

        [Test]
        public void TimeoutIsEvaluatedInSeconds()
        {
            var attrib = new TimeoutAttribute(120);
            Assert.AreEqual(120, attrib.TimeoutSeconds);
            Assert.AreEqual(TimeSpan.FromSeconds(120), attrib.Timeout);
        }
    }
}
