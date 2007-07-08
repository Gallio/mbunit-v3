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
        /// Populates the test template tree using a test project.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="parent">The parent template</param>
        /// <param name="project">The test project</param>
        void PopulateTestTemplateTree(TestTemplateTreeBuilder builder, ITestTemplate parent, TestProject project);
    }
}
