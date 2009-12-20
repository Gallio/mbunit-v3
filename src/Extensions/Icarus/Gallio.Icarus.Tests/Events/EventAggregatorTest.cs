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
