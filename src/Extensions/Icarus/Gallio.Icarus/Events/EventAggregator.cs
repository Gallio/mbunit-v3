using Gallio.Runtime.Extensibility;

namespace Gallio.Icarus.Events
{
    /// <inheritdoc />
    public class EventAggregator : IEventAggregator
    {
        private readonly IServiceLocator serviceLocator;

        public EventAggregator(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        public void Send<T>(T message) where T : Event
        {
            foreach (var handler in serviceLocator.ResolveAll<Handles<T>>())
            {
                handler.Handle(message);
            }
        }
    }
}