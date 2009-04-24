using System.Threading;
using Gallio.Icarus.Utilities;

namespace Gallio.Icarus.Tests.Utilities
{
    internal class TestSynchronizationContext : ISynchronizationContext
    {
        public void Post(SendOrPostCallback sendOrPostCallback, object state)
        {
            sendOrPostCallback(state);
        }

        public void Send(SendOrPostCallback sendOrPostCallback, object state)
        {
            sendOrPostCallback(state);
        }
    }
}
