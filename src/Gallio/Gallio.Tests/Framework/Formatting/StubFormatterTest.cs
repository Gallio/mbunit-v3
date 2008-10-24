using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework.Formatting;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Formatting
{
    [TestsOn(typeof(StubFormatter))]
    public class StubFormatterTest
    {
        [Test]
        public void StubFormatterCanUseBuiltInRules()
        {
            StubFormatter formatter = new StubFormatter();

            // Just try a couple of basic types.
            Assert.AreEqual("\"abc\"", formatter.Format("abc"));
            Assert.AreEqual("1.2m", formatter.Format(1.2m));
            Assert.AreEqual("'\\n'", formatter.Format('\n'));
        }
    }
}
