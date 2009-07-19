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
using System.Linq;
using System.Text;
using Gallio.Common.Diagnostics;
using Gallio.Runtime.Logging;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Logging
{
    [TestsOn(typeof(DispatchLogger))]
    public class DispatchLoggerTest
    {
        [Test]
        public void AddLogListener_WhenLoggerIsNull_Throws()
        {
            DispatchLogger logger = new DispatchLogger();

            Assert.Throws<ArgumentNullException>(() => logger.AddLogListener(null));
        }

        [Test]
        public void RemoveLogListener_WhenLoggerIsNull_Throws()
        {
            DispatchLogger logger = new DispatchLogger();

            Assert.Throws<ArgumentNullException>(() => logger.RemoveLogListener(null));
        }

        [Test]
        public void Log_WhenEventHandlerRegistered_FiresEvent()
        {
            LogEntrySubmittedEventArgs receivedEvent = null;
            DispatchLogger logger = new DispatchLogger();
            logger.LogMessage += (sender, e) => receivedEvent = e;

            logger.Log(LogSeverity.Important, "Message", new Exception("foo"));

            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual(LogSeverity.Important, receivedEvent.Severity);
            Assert.AreEqual("Message", receivedEvent.Message);
            Assert.AreEqual("foo", receivedEvent.ExceptionData.Message);
        }

        [Test]
        public void Log_WhenListenerRegistered_CallsListener()
        {
            DispatchLogger logger = new DispatchLogger();
            ILogger listener = MockRepository.GenerateMock<ILogger>();
            var ex = new ExceptionData(new Exception("foo"));
            listener.Expect(x => listener.Log(LogSeverity.Important, "Message", ex));
            logger.AddLogListener(listener);

            logger.Log(LogSeverity.Important, "Message", ex);

            listener.VerifyAllExpectations();
        }

        [Test]
        public void Log_WhenListenerRegisteredThenUnregistered_DoesNotCallListener()
        {
            DispatchLogger logger = new DispatchLogger();
            ILogger listener = MockRepository.GenerateMock<ILogger>();
            var ex = new ExceptionData(new Exception("foo"));
            logger.AddLogListener(listener);
            logger.RemoveLogListener(listener);

            logger.Log(LogSeverity.Important, "Message", ex);

            listener.VerifyAllExpectations();
        }
    }
}
