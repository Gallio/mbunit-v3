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
using System.Linq;
using System.Text;
using Gallio.Runtime.Logging;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Logging
{
    [TestsOn(typeof(BufferedLogWriter))]
    public class BufferedLogWriterTest
    {
        [Test]
        public void Constructor_WhenLoggerIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new BufferedLogWriter(null, LogSeverity.Error));
        }

        [Test]
        public void Encoding_ReturnsUnicode()
        {
            var logger = MockRepository.GenerateStub<ILogger>();
            var writer = new BufferedLogWriter(logger, LogSeverity.Important);

            Assert.AreEqual(Encoding.Unicode, writer.Encoding);
        }

        [Test]
        public void NewLine_ReturnsLinefeed()
        {
            var logger = MockRepository.GenerateStub<ILogger>();
            var writer = new BufferedLogWriter(logger, LogSeverity.Important);

            Assert.AreEqual("\n", writer.NewLine);
        }

        [Test]
        public void Write_BuffersLogMessagesUpToNewLineAndTrimsBlankLinesAtEnds()
        {
            var logger = MockRepository.GenerateMock<ILogger>();
            logger.Expect(x => x.Log(LogSeverity.Important, "First message"));
            logger.Expect(x => x.Log(LogSeverity.Important, "Second message"));
            logger.Expect(x => x.Log(LogSeverity.Important, "Third message\nWith extra line"));
            logger.Expect(x => x.Log(LogSeverity.Important, "Fourth message"));

            var writer = new BufferedLogWriter(logger, LogSeverity.Important);

            writer.Write("First");
            writer.WriteLine(" message");
            writer.WriteLine("\n\nSecond message\n\n");
            writer.WriteLine("Third message\nWith extra line");
            writer.WriteLine();
            writer.Write("Fourth message".ToCharArray());
            writer.Dispose();

            logger.VerifyAllExpectations();
        }
    }
}
