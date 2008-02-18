using Gallio.Hosting;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// Static service locator class for <see cref="ITestContextTracker" />.
    /// Handles the case where no <see cref="ITestContextTracker" /> is registered
    /// with the <see cref="Runtime" /> by returning a <see cref="StubTestContextTracker" />.
    /// </summary>
    public static class TestContextTrackerAccessor
    {
        private static ITestContextTracker cachedContextTracker;

        static TestContextTrackerAccessor()
        {
            Runtime.InstanceChanged += delegate { cachedContextTracker = null; };
        }

        /// <summary>
        /// Gets the context tracker instance.
        /// </summary>
        public static ITestContextTracker GetInstance()
        {
            if (cachedContextTracker == null)
            {
                if (Runtime.IsInitialized)
                    cachedContextTracker = Runtime.Instance.Resolve<ITestContextTracker>();
                else
                    cachedContextTracker = new StubTestContextTracker();
            }

            return cachedContextTracker;
        }
    }
}
