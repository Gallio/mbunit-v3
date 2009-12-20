namespace Gallio.Icarus.Events
{
    /// <summary>
    /// Marker interface used by the EventAggregator to
    /// locate classes interested in an event.
    /// </summary>
    /// <typeparam name="T">The type of event.</typeparam>
    public interface Handles<T> where T: Event
    {
        void Handle(T message);
    }
}