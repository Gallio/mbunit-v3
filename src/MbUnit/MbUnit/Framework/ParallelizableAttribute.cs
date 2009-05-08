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
using System.Threading;
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Specifies that a test can be run in parallel with other parallelizable tests.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this attribute is not specified, then the test will be run with Process-level
    /// isolation.  That is, it will not run concurrently with any other test within the
    /// given process (notwithstanding any use of <see cref="ThreadedRepeatAttribute" />
    /// or similar decorators).
    /// </para>
    /// <para>
    /// Parallelizable tests are run in batches subject to the hierarchical structure of
    /// the tests, any explicit ordering specifications, and test dependencies.
    /// </para>
    /// <para>
    /// If two tests must be run exclusively of one another but are otherwise parallelizable,
    /// then a simple expedient is to assign a different test order to each one.  Later editions
    /// of the framework may introduce additional controls over test isolation, mutual exclusion,
    /// access to shared resources, and degree of paralleism.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public class ParallelizableAttribute : TestDecoratorPatternAttribute
    {
        /// <inheritdoc />
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            scope.TestBuilder.IsParallelizable = true;
        }
    }
}
