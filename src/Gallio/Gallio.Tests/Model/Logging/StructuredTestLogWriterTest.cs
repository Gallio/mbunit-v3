using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Model.Logging;
using MbUnit.Framework;

namespace Gallio.Tests.Model.Logging
{
    [TestsOn(typeof(StructuredTestLogWriter))]
    public class StructuredTestLogWriterTest
    {
        [Test]
        public void ToStringReturnsTheFormattedLog()
        {
            StructuredTestLogWriter writer = new StructuredTestLogWriter();
            writer.Default.WriteLine("text");

            Assert.AreEqual("*** Log ***\n\ntext\n", writer.ToString());
        }
    }
}
