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

using System.Collections.Generic;
using Gallio.Icarus.Events;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Events
{
    [TestsOn(typeof(EventAggregator))]
    public class EventAggregatorTest
    {
        [Test]
        public void Send_should_call_handle_on_all_interested_parties()
        {
            var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
            var handler = new TestEventHandler();
            serviceLocator.Stub(sl => sl.ResolveAll<Handles<TestEvent>>())
                .Return(new List<Handles<TestEvent>> { handler });
            var eventAggregator = new EventAggregator(serviceLocator);

            eventAggregator.Send(new TestEvent());

            Assert.IsTrue(handler.Handled);
        }

        private class TestEvent : Event
        { }

        private class TestEventHandler : Handles<TestEvent>
        {
            public bool Handled { get; private set; }

            public void Handle(TestEvent message)
            {
                Handled = true;
            }
        }
    }
}
