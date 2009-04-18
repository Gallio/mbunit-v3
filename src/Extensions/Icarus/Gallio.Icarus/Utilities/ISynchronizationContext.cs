using System.Threading;

namespace Gallio.Icarus.Utilities
{
    public interface ISynchronizationContext
    {
        void Post(SendOrPostCallback sendOrPostCallback, object state);
        void Send(SendOrPostCallback sendOrPostCallback, object state);
    }
}
