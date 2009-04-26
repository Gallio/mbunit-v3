using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Reflection;
using Gallio.Runtime.Extensibility;

namespace Gallio.Model
{
    /// <summary>
    /// Provides services based on the installed set of test frameworks.
    /// </summary>
    public interface ITestFrameworkManager
    {
        /// <summary>
        /// Gets handles for all registered test frameworks.
        /// </summary>
        IList<ComponentHandle<ITestFramework, TestFrameworkTraits>> FrameworkHandles { get; }

        /// <summary>
        /// Gets an aggregate test explorer for selected frameworks.
        /// </summary>
        /// <param name="frameworkFilter">A predicate to select which frameworks should
        /// be consulted, or null to include all frameworks</param>
        /// <returns>The test explorer</returns>
        ITestExplorer GetTestExplorer(Predicate<TestFrameworkTraits> frameworkFilter);
    }
}
