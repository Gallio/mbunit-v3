using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Runtime.Logging;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Logging
{
    [TestsOn(typeof(SeverityPrefixLogger))]
    public class SeverityPrefixLoggerTest
    {
        [Test]
        public void ConstructorThrowsWhenTextWriterIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SeverityPrefixLogger(null));
        }

        [Test]
        [Row(LogSeverity.Debug, "A debug message.", false,
            "[Debug] A debug message.\n")]
        [Row(LogSeverity.Error, "An error message.", false,
            "[Error] An error message.\n")]
        [Row(LogSeverity.Important, "An important message.", false,
            "[Important] An important message.\n")]
        [Row(LogSeverity.Info, "An info message.", false,
            "[Info] An info message.\n")]
        [Row(LogSeverity.Warning, "A warning message.", false,
            "[Warning] A warning message.\n")]
        [Row(LogSeverity.Error, "An error message.", true,
            "[Error] An error message.\n  System.Exception: Foo\n")]
        public void WritesFormattedMessageToTextWriter(LogSeverity severity, string message, bool includeException, string expectedText)
        {
            StringWriter writer = new StringWriter();
            writer.NewLine = "\n";
            var logger = new SeverityPrefixLogger(new TextLogger(writer));

            logger.Log(severity, message, includeException ? new Exception("Foo") : null);

            Assert.AreEqual(expectedText, writer.ToString());
        }

        [Test]
        [Row(LogSeverity.Debug)]
        [Row(LogSeverity.Info)]
        [Row(LogSeverity.Important)]
        [Row(LogSeverity.Error)]
        [Row(LogSeverity.Warning)]
        public void LoggerAndParserAreSymmetrical(LogSeverity severity)
        {
            const string message = "Message";

            StringWriter writer = new StringWriter();
            writer.NewLine = "\n";
            var logger = new SeverityPrefixLogger(new TextLogger(writer));
            logger.Log(severity, message);

            var parser = new SeverityPrefixParser();

            LogSeverity parsedSeverity;
            string parsedMessage;
            parser.ParseLine(writer.ToString().Trim(), out parsedSeverity, out parsedMessage);

            Assert.AreEqual(severity, parsedSeverity);
            Assert.AreEqual(message, parsedMessage);
        }
    }
}
