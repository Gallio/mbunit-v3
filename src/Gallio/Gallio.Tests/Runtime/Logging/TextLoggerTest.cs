// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
    [TestsOn(typeof(TextLogger))]
    public class TextLoggerTest
    {
        [Test]
        public void ConstructorThrowsWhenTextWriterIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TextLogger(null));
        }

        [Test]
        [Row(LogSeverity.Debug, "A debug message.", false,
            "A debug message.\n")]
        [Row(LogSeverity.Error, "An error message.", false,
            "An error message.\n")]
        [Row(LogSeverity.Important, "An important message.", false,
            "An important message.\n")]
        [Row(LogSeverity.Info, "An info message.", false,
            "An info message.\n")]
        [Row(LogSeverity.Warning, "A warning message.", false,
            "A warning message.\n")]
        [Row(LogSeverity.Error, "An error message.", true,
            "An error message.\n  System.Exception: Foo\n")]
        public void WritesFormattedMessageToTextWriter(LogSeverity severity, string message, bool includeException, string expectedText)
        {
            StringWriter writer = new StringWriter();
            writer.NewLine = "\n";
            TextLogger logger = new TextLogger(writer);

            logger.Log(severity, message, includeException ? new Exception("Foo") : null);

            Assert.AreEqual(expectedText, writer.ToString());
        }
    }
}
