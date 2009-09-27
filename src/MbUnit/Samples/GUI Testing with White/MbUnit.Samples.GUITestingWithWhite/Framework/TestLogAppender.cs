using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using log4net.Appender;
using log4net.Core;

namespace MbUnit.Samples.GUITestingWithWhite.Framework
{
    /// <summary>
    /// A log4net appender that writes log output to the TestLog.
    /// White uses log4net to write progress information during a test run.
    /// </summary>
    public class TestLogAppender : IAppender
    {
        public string Name { get; set; }

        public void DoAppend(LoggingEvent loggingEvent)
        {
            string message = loggingEvent.RenderedMessage;
            TestLog.Writer["White"].WriteLine(message);
        }

        public void Close()
        {
        }
    }
}
