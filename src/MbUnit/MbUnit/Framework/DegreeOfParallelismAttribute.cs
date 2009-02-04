using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Specifies the maximum number of concurrent threads to use when tests are run in parallel
    /// for all tests in the test assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Defaults to <see cref="Environment.ProcessorCount" /> or 2, whichever is greater.
    /// </para>
    /// </remarks>
    /// <seealso cref="TestAssemblyExecutionParameters.DegreeOfParallelism"/>
    [AttributeUsage(PatternAttributeTargets.TestAssembly, AllowMultiple = true, Inherited = true)]
    public class DegreeOfParallelismAttribute : TestAssemblyDecoratorPatternAttribute
    {
        private readonly int degreeOfParallelism;

        /// <summary>
        /// Specifies the maximum number of concurrent threads to use when tests are run in parallel.
        /// </summary>
        /// <param name="degreeOfParallelism">The degree of parallelism</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="degreeOfParallelism"/> is less than 1</exception>
        public DegreeOfParallelismAttribute(int degreeOfParallelism)
        {
            if (degreeOfParallelism < 1)
                throw new ArgumentOutOfRangeException("degreeOfParallelism", "Degree of parallelism must be at least 1.");

            this.degreeOfParallelism = degreeOfParallelism;
        }

        /// <summary>
        /// Gets the degree of parallelism.
        /// </summary>
        public int DegreeOfParallelism
        {
            get { return degreeOfParallelism; }
        }

        /// <inheritdoc />
        protected override void DecorateAssemblyTest(IPatternScope assemblyScope, IAssemblyInfo assembly)
        {
            assemblyScope.TestBuilder.TestActions.InitializeTestChain.After(state =>
            {
                TestAssemblyExecutionParameters.DegreeOfParallelism = degreeOfParallelism;
            });
        }
    }
}
