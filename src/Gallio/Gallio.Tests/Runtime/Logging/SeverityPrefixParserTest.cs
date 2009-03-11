// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
