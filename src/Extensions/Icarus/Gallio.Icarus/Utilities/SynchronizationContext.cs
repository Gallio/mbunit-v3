using System.Threading;

namespace Gallio.Icarus.Utilities
{
    internal class SynchronizationContext : ISynchronizationContext
    {
        private readonly System.Threading.SynchronizationContext synchronizationContext;

        internal SynchronizationContext(System.Threading.SynchronizationContext synchronizationContext)
        {
            this.synchronizationContext = synchronizationContext;
        }

        public void Post(SendOrPostCallback sendOrPostCallback, object sender)
        {
            synchronizationContext.Post(sendOrPostCallback, sender);
        }
    }
}
