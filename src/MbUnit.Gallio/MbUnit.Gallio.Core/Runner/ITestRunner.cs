using System;
using MbUnit.Core.Serialization;
using MbUnit.Framework.Services.Runtime;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// A test runner provides operations for loading test projects, enumerating
    /// templates and tests, running tests, and generating reports.
    /// </summary>
    /// <remarks>
    /// This interface is primarily used to simplify test runner integration
    /// concerns by gathering the entire lifecycle in one place.
    /// </remarks>
    public interface ITestRunner : IDisposable
    {
        /// <summary>
        /// Loads a test project.
        /// </summary>
        /// <param name="project">The test project</param>
        void LoadProject(TestProject project);

        /// <summary>
        /// Gets the root of the template tree.
        /// Automatically builds the template tree if needed.
        /// </summary>
        TemplateInfo GetTemplateTreeRoot();

        /// <summary>
        /// Gets the root of the test tree.
        /// Automatically builds the test tree if needed.
        /// </summary>
        TestInfo GetTestTreeRoot();

        /// <summary>
        /// Runs the tests.
        /// </summary>
        void Run();

        /// <summary>
        /// Writes a test report.
        /// </summary>
        void WriteReport();
    }
}