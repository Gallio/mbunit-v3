using System;
using System.Collections.Generic;
using System.Reflection;

namespace MbUnit.Core.Model
{
    /// <summary>
    /// The test framework service provides support for enumerating and executing
    /// tests that belong to some test framework.  A new third party test framework
    /// may be supported by defining and registering a suitable implementation
    /// of this interface.
    /// </summary>
    public interface ITestFramework
    {
        /// <summary>
        /// Gets the name of the test framework.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Populates the test template tree with this framework's contributions.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="parent">The parent template</param>
        void BuildTemplates(TestTemplateTreeBuilder builder, ITestTemplate parent);

        /// <summary>
        /// Provides the test framework with an opportunity to perform processing
        /// just after a test assembly is loaded.  For example, it might quickly
        /// scan the assembly to configure assembly resolution strategies or
        /// to configure the behavior of built-in services in other ways.
        /// </summary>
        /// <param name="assembly">The loaded test assembly</param>
        void InitializeTestAssembly(Assembly assembly);
    }
}
