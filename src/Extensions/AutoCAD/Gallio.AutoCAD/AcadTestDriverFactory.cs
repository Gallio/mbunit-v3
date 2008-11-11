using System;
using Gallio.Runner;
using Gallio.Runner.Drivers;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// Provides support for creating <see cref="ITestDriver"/> instances inside an AutoCAD process.
    /// </summary>
    public class AcadTestDriverFactory : ITestDriverFactory
    {
        private IAcadProcessFactory processFactory;

        /// <summary>
        /// Intializes a new AutoCAD test driver factory.
        /// </summary>
        /// <param name="processFactory">A <see cref="IAcadProcessFactory"/> instance.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="processFactory"/> is null.</exception>
        public AcadTestDriverFactory(IAcadProcessFactory processFactory)
        {
            if (processFactory == null)
                throw new ArgumentNullException("processFactory");
            this.processFactory = processFactory;
        }

        /// <inheritdoc/>
        public ITestDriver CreateTestDriver()
        {
            IAcadProcess process = processFactory.CreateProcess();
            if (process == null)
                throw new RunnerException("Unable to create ITestDriver because IAcadProcessFactory returned a null IAcadProcess.");

            IRemoteTestDriver remoteDriver = process.GetRemoteTestDriver();
            return new AcadTestDriver(process, remoteDriver);
        }
    }
}
