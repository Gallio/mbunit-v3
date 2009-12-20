namespace Gallio.Icarus.Events
{
    /// <summary>
    /// An event aggregator.
    /// http://martinfowler.com/eaaDev/EventAggregator.html
    /// </summary>
    public interface IEventAggregator
    {
        void Send<T>(T message) where T : Event;
    }
}