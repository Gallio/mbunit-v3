using System.Threading;

namespace Gallio.Icarus.Utilities
{
    public interface ISynchronizationContext
    {
        void Post(SendOrPostCallback sendOrPostCallback, object sender);
    }
}
