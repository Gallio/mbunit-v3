using Gallio.Icarus.Events;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Events
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
