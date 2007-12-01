using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Model;
using Gallio.Model.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// <para>
    /// A test explorer scans a volume of code using reflection to build a
    /// partial test tree.  The tests constructed in this manner may not be
    /// complete or executable but they provide useful insight into the
    /// layout of the test suite that can subsequently be used to drive the
    /// test runner.
    /// </para>
    /// <para>
    /// A test explorer may assume that the reflection data will remain unchanged
    /// and cache information until <see cref="Reset" /> is called.
    /// </para>
    /// </summary>
    public interface ITestExplorer
    {
        /// <summary>
        /// Notifies the test explorer that reflection data may have changed.
        /// The test explorer can take advantage of this opportunity to flush
        /// its caches.
        /// </summary>
        void Reset();

        /// <summary>
        /// Returns true if the code element represents a test.
        /// </summary>
        /// <param name="element">The element</param>
        /// <returns>True if the element represents a test</returns>
        bool IsTest(ICodeElementInfo element);

        /// <summary>
        /// Explores the tests defined by a type.
        /// </summary>
        /// <remarks>
        /// This method should not recurse into nested types, if any.
        /// </remarks>
        /// <param name="type">The type</param>
        /// <returns>An enumeration of the top-level tests defined
        /// by the type.  For example, if the type is a test fixture, this
        /// method might return a single <see cref="ITest" /> representing
        /// the fixture with the individual test cases represented as children of the fixture.</returns>
        IEnumerable<ITest> ExploreType(ITypeInfo type);

        /// <summary>
        /// Explores the tests defined by an assembly.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <returns>An enumeration of the top-level tests defined by the assembly.</returns>
        IEnumerable<ITest> ExploreAssembly(IAssemblyInfo assembly);
    }
}
