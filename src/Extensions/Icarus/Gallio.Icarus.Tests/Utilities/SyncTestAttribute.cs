using MbUnit.Framework;
using Gallio.Framework.Pattern;
using Gallio.Icarus.Utilities;

namespace Gallio.Icarus.Tests.Utilities
{
    internal class SyncTestAttribute : TestAttribute
    {
        protected override object Execute(PatternTestInstanceState state)
        {
            SynchronizationContext.Instance = new TestSynchronizationContext();
            var o = base.Execute(state);
            SynchronizationContext.Instance = null;
            return o;
        }
    }
}
