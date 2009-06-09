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
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Specifies that a test can be run in parallel with other parallelizable tests.
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
    /// <para>
    /// To make all tests within a fixture parallelizable, apply this attribute to
    /// the fixture class with a scope of <see cref="TestScope.Descendants" /> to
    /// only make the tests parallelizable or <see cref="TestScope.All"/> to also make
    /// the fixture itself parallelizable.   Likewise to make all tests in a test assembly
    /// parallelizable, apply this attribute to the test assembly with the scope set similarly.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// This C# example shows a fixture with a few tests, two of which are parallelizable.
    /// <code><![CDATA[
    /// public class Fixture
    /// {
    ///     // may run in parallel with test 2
    ///     [Parallelizable]
    ///     public void Test1() { ... }
    ///     
    ///     // may run in parallel with test 1
    ///     [Parallelizable]
    ///     public void Test2() { ... }
    ///     
    ///     // will not run in parallel because it is not parallelizable
    ///     public void Test3() { ... }
    /// }
    /// ]]></code>
    /// </para>
    /// <para>
    /// This C# example shows a fixture all of whose tests are parallelizable.
    /// <code><![CDATA[
    /// [Parallizable(TestScope.Descendants)]
    /// public class Fixture
    /// {
    ///     public void Test1() { ... }
    ///     
    ///     public void Test2() { ... }
    ///     
    ///     public void Test3() { ... }
    /// }
    /// ]]></code>
    /// </para>
    /// <para>
    /// This C# example shows a test assembly all of whose tests are parallelizable.
    /// <code><![CDATA[
    /// [assembly: Parallizable(TestScope.All)]
    /// ]]></code>
    /// </para>
    /// </example>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public class ParallelizableAttribute : TestDecoratorPatternAttribute
    {
        private readonly TestScope scope;

        /// <summary>
        /// Specifies that this test is parallelizable.
        /// </summary>
        public ParallelizableAttribute()
            : this(TestScope.Self)
        {
        }

        /// <summary>
        /// Specifies that the tests in the specified scope are parallelizable.
        /// </summary>
        /// <param name="scope">The test scope.</param>
        public ParallelizableAttribute(TestScope scope)
        {
            this.scope = scope;
        }

        /// <summary>
        /// Gets the scope to which the parallelizable attribute applies.
        /// </summary>
        public TestScope Scope
        {
            get { return scope; }
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            if (this.scope == TestScope.Self || this.scope == TestScope.All)
            {
                scope.TestBuilder.IsParallelizable = true;
            }

            if (this.scope == TestScope.Descendants || this.scope == TestScope.All)
            {
                SetParallelizableRecursively(scope.TestBuilder.ToTest());
            }
        }

        private static void SetParallelizableRecursively(PatternTest parent)
        {
            foreach (PatternTest child in parent.Children)
            {
                child.IsParallelizable = true;

                SetParallelizableRecursively(child);
            }
        }
    }
}
