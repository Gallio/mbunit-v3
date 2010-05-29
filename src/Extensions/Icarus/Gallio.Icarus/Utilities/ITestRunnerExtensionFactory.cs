using Gallio.Runner.Extensions;

namespace Gallio.Icarus.Utilities
{
    public interface ITestRunnerExtensionFactory
    {
        ///<summary>
        /// Create a <see cref="ITestRunnerExtension" /> from a specification.
        ///</summary>
        ///<param name="specification">A test runner extension specification.</param>
        ///<returns>A <see cref="ITestRunnerExtension" />.</returns>
        ITestRunnerExtension CreateFromSpecification(string specification);
    }
}