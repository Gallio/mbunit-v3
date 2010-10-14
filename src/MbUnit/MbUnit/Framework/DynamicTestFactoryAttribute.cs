// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Diagnostics;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// Specifies a method that is used to generate tests dynamically at runtime.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The tests created by the dynamic test factory are considered to be children
    /// of the factory method that generated them.  Because the tests are created
    /// dynamically, they will not appear in the test tree (so most test runners will
    /// not find them) and they are not directly affected by test filters.  However,
    /// the dynamic test factory is represented as a test so it will be visible in
    /// the test tree.
    /// </para>
    /// <para>
    /// Contrast with <see cref="StaticTestFactoryAttribute" />.
    /// </para>
    /// <para>
    /// The method to which this attribute is applied must be declared by the
    /// fixture class.  It may have parameters and be data-driven just like an ordinary
    /// test method!  The method may be static.  It must return an enumeration of
    /// values of type <see cref="Test" />.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// A simple dynamic test factory that reads some data from a file and generates
    /// a number of dynamic tests.
    /// <code><![CDATA[
    /// [DynamicTestFactory]
    /// public IEnumerable<Test> CreateDynamicTests()
    /// {
    ///     foreach (string searchTerm in File.ReadAllLines("SearchTerms.txt"))
    ///     {
    ///         yield return new TestCase("Search Term: " + searchTerm, () => {
    ///             var searchEngine = new SearchEngine();
    ///             Assert.IsNotEmpty(searchEngine.GetSearchResults(searchTerm));
    ///         });
    ///     }
    /// }
    /// ]]></code>
    /// </para>
    /// <para>
    /// A data-driven test factory that reads some data from a file and generates
    /// a number of dynamic tests repeatedly in multiple configurations.
    /// <code><![CDATA[
    /// [DynamicTestFactory]
    /// [Row("Google")]
    /// [Row("Yahoo")]
    /// public IEnumerable<Test> CreateDynamicTests(string searchProvider)
    /// {
    ///     foreach (string searchTerm in File.ReadAllLines("SearchTerms.txt"))
    ///     {
    ///         yield return new TestCase("Search Term: " + searchTerm, () => {
    ///             var searchEngine = new SearchEngine(searchProvider);
    ///             Assert.IsNotEmpty(searchEngine.GetSearchResults(searchTerm));
    ///         });
    ///     }
    /// }
    /// ]]></code>
    /// </para>
    /// </example>
    /// <seealso cref="StaticTestFactoryAttribute"/>
    /// <seealso cref="TestCase"/>
    [AttributeUsage(PatternAttributeTargets.TestMethod, AllowMultiple = false, Inherited = true)]
    public class DynamicTestFactoryAttribute : TestMethodPatternAttribute
    {
        /// <inheritdoc />
        [DebuggerNonUserCode]
        protected override void Execute(PatternTestInstanceState state)
        {
            var tests = state.InvokeTestMethod() as IEnumerable<Test>;
            if (tests == null)
                throw new TestFailedException("Expected the dynamic test factory method to "
                    + "return a value that is assignable to type IEnumerable<Test>.");

            TestOutcome outcome = Test.RunDynamicTests(tests, state.Test.CodeElement, null, null);
            if (outcome != TestOutcome.Passed)
                throw new SilentTestException(outcome);
        }
    }
}
