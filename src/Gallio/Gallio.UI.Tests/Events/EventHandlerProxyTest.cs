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

using Gallio.UI.Events;
using MbUnit.Framework;

namespace Gallio.UI.Tests.Events
{
    [Category("Events"), TestsOn(typeof(EventHandlerProxy<>))]
    public class EventHandlerProxyTest
    {
        [Test]
        public void Handle_should_pass_event_through()
        {
            var @event = new TestEvent();
            var handler = new TestHandler(@event);
            var proxy = new EventHandlerProxy<TestEvent>(handler);

            proxy.Handle(@event);

            Assert.IsTrue(handler.Handled);
        }

        private class TestEvent : Event { }

        private class TestHandler : Handles<TestEvent>
        {
            private readonly TestEvent @event;

            public TestHandler(TestEvent @event)
            {
                this.@event = @event;
            }

            public void Handle(TestEvent testEvent)
            {
                if (testEvent == @event)
                    Handled = true;
            }

            public bool Handled { get; private set; }
        }
    }
}
