using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Runtime.Logging;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Logging
{
    [TestsOn(typeof(EventLogger))]
    public class EventLoggerTest
    {
        [Test]
        public void FiresEventWhenLogged()
        {
            LogMessageEventArgs receivedEvent = null;
            EventLogger logger = new EventLogger();
            logger.LogMessage += (sender, e) => receivedEvent = e;

            logger.Log(LogSeverity.Important, "Message", new Exception("foo"));

            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual(LogSeverity.Important, receivedEvent.Severity);
            Assert.AreEqual("Message", receivedEvent.Message);
            Assert.AreEqual("Foo", receivedEvent.Exception.Message);
        }
    }
}
