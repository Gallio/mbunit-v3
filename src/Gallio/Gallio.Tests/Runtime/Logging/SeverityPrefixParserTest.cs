using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Runtime.Logging;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Logging
{
    [TestsOn(typeof(SeverityPrefixParser))]
    public class SeverityPrefixParserTest
    {
        [Test]
        [Row("Default severity for first message.", new object[] {
            new object[] { LogSeverity.Info, "Default severity for first message." }
        })]
        [Row("[Important] Message 1\nMessage 1 Part 2\n[Error] Message 2\n[Debug] Message 3\n[Info] Message 4\n[Warning] Message 5\nMessage 5 Part 2\nMessage 5 Part 3", new object[] {
            new object[] { LogSeverity.Important, "Message 1" },
            new object[] { LogSeverity.Important, "Message 1 Part 2" },
            new object[] { LogSeverity.Error, "Message 2" },
            new object[] { LogSeverity.Debug, "Message 3" },
            new object[] { LogSeverity.Info, "Message 4" },
            new object[] { LogSeverity.Warning, "Message 5" },
            new object[] { LogSeverity.Warning, "Message 5 Part 2" },
            new object[] { LogSeverity.Warning, "Message 5 Part 3" },
        })]
        public void WritesFormattedMessageToTextWriter(string text, object[] expectedSeverityAndMessages)
        {
            var parser = new SeverityPrefixParser();

            List<object> actualSeverityAndMessages = new List<object>();
            foreach (string line in text.Split('\n'))
            {
                LogSeverity severity;
                string message;
                parser.ParseLine(line, out severity, out message);

                actualSeverityAndMessages.Add(new object[] { severity, message });
            }

            Assert.AreElementsEqual(expectedSeverityAndMessages, actualSeverityAndMessages);
        }
    }
}
