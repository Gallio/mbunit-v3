namespace Gallio.Icarus.Events
{
    public class EventHandlerProxy<T> : Handles<T> where T : Event
    {
        private readonly Handles<T> target;

        public EventHandlerProxy(Handles<T> target)
        {
            this.target = target;
        }

        public void Handle(T message)
        {
            target.Handle(message);
        }
    }
}
