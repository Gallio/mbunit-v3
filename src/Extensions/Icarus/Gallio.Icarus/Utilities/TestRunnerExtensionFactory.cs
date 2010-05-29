using Gallio.Runner.Extensions;

namespace Gallio.Icarus.Utilities
{
    /// <summary>
    /// Factory for <see cref="ITestRunnerExtension" />s.
    /// </summary>
    public class TestRunnerExtensionFactory : ITestRunnerExtensionFactory
    {
        ///<summary>
        /// Create a <see cref="ITestRunnerExtension" /> from a specification.
        ///</summary>
        ///<param name="specification">A test runner extension specification.</param>
        ///<returns>A <see cref="ITestRunnerExtension" />.</returns>
        public ITestRunnerExtension CreateFromSpecification(string specification)
        {
            return TestRunnerExtensionUtils.CreateExtensionFromSpecification(specification);
        }
    }
}
