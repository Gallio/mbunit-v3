using System;
using Gallio.Model.Reflection;
using MbUnit.Model.Builder;

namespace MbUnit.Model.Patterns
{
    /// <summary>
    /// <para>
    /// A dependency pattern attribute creates a dependency on the tests defined
    /// by some other code element.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method
        | AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public abstract class DependencyPatternAttribute : PatternAttribute
    {
        /// <inheritdoc />
        public override void ProcessTest(ITestBuilder testBuilder, ICodeElementInfo codeElement)
        {
            testBuilder.AddDecorator(int.MaxValue, delegate
            {
                ICodeElementInfo resolvedDependency = GetDependency(testBuilder, codeElement);

                foreach (ITestBuilder dependentTestBuilder in testBuilder.TestModelBuilder.GetTestBuilders(resolvedDependency))
                    testBuilder.AddDependency(dependentTestBuilder.Test);
            });
        }

        /// <summary>
        /// Gets the code element that declares the tests on which this test should depend.
        /// </summary>
        /// <param name="testBuilder">The test builder</param>
        /// <param name="codeElemeent">The code element</param>
        /// <returns>The code element representing the dependency</returns>
        protected abstract ICodeElementInfo GetDependency(ITestBuilder testBuilder, ICodeElementInfo codeElemeent);
    }
}