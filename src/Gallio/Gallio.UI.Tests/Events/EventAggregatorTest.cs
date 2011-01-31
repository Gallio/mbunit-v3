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

using System.Collections.Generic;
using Gallio.Runtime.Extensibility;
using Gallio.UI.Events;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.UI.Tests.Events
{
    [TestsOn(typeof(EventAggregator))]
    public class EventAggregatorTest
    {
        private EventAggregator eventAggregator;
        private IServiceLocator serviceLocator;

        [SetUp]
        public void SetUp()
        {
            serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
            eventAggregator = new EventAggregator(serviceLocator);
        }

        [Test]
        public void Send_should_call_handle_on_all_interested_parties()
        {
            var handler = new TestEventHandler();
            serviceLocator.Stub(sl => sl.ResolveAll<Handles<TestEvent>>())
                .Return(new List<Handles<TestEvent>> { handler });

            eventAggregator.Send(this, new TestEvent());

            Assert.IsTrue(handler.Handled, "Expected Handle to be called on the handler.");
        }

        [Test]
        public void Send_should_not_call_handle_on_the_sender()
        {
            var handler = new TestEventHandler();
            serviceLocator.Stub(sl => sl.ResolveAll<Handles<TestEvent>>())
                .Return(new List<Handles<TestEvent>> { handler });

            eventAggregator.Send(handler, new TestEvent());

            Assert.IsFalse(handler.Handled, "Expected Handle not to be called on the sender.");
        }

        private class TestEvent : Event
        { }

        private class TestEventHandler : Handles<TestEvent>
        {
            public bool Handled { get; private set; }

            public void Handle(TestEvent @event)
            {
                Handled = true;
            }
        }
    }
}
