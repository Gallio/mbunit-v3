using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework.Pattern;
using MbUnit.Framework;

// Note: Used by DefaultTestCaseTimeout test.
[assembly: DefaultTestCaseTimeout(9 * 60)]

namespace MbUnit.Tests.Framework
{
    public class DefaultTestCaseTimeoutAttributeTest
    {
        [Test]
        public void AttributeShouldSetExecutionParameterAtRuntime()
        {
            Assert.AreEqual(TimeSpan.FromMinutes(9), TestAssemblyExecutionParameters.DefaultTestCaseTimeout);
        }

        [Test]
        public void ZeroTimeoutMeansInfinite()
        {
            var attrib = new DefaultTestCaseTimeoutAttribute(0);
            Assert.AreEqual(0, attrib.TimeoutSeconds);
            Assert.IsNull(attrib.Timeout);
        }

        [Test]
        public void DisallowsNegativeTimeout()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DefaultTestCaseTimeoutAttribute(-1));
        }

        [Test]
        public void TimeoutIsEvaluatedInSeconds()
        {
            var attrib = new DefaultTestCaseTimeoutAttribute(120);
            Assert.AreEqual(120, attrib.TimeoutSeconds);
            Assert.AreEqual(TimeSpan.FromSeconds(120), attrib.Timeout);
        }
    }
}
