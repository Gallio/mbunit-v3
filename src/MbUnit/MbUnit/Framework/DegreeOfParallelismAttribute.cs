// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;

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
        /// <param name="degreeOfParallelism">The degree of parallelism.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="degreeOfParallelism"/> is less than 1.</exception>
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
