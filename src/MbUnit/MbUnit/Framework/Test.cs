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
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Common.Diagnostics;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Describes a test generated either at test exploration time or at test
    /// execution time by a test factory.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Tests cam be nested to form test suites and other aggregates.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// Produces a suite of static tests as part of a containing test fixture.
    /// The suite includes some custom set-up and tear-down behavior, metadata,
    /// a timeout, an order relative to the other tests in the fixture, and a list
    /// of children.  There are two test cases defined in line, and one reference
    /// to another statically defined test elsewhere.
    /// <code><![CDATA[
    /// [StaticTestFactory]
    /// public static IEnumerable<Test> TestSuite()
    /// {
    ///     yield return new TestSuite("My Suite")
    ///     {
    ///         Description = "An example test suite.",
    ///         Metadata =
    ///         {
    ///             { MetadataKeys.AuthorName, "Me" },
    ///             { MetadataKeys.AuthorEmail, "me@mycompany.com" }
    ///         },
    ///         SuiteSetUp = () => DatabaseUtils.SetUpDatabase(),
    ///         SuiteTearDown = () => DatabaseUtils.TearDownDatabase(),
    ///         Timeout = TimeSpan.FromMinutes(2),
    ///         Children =
    ///         {
    ///             new TestCase("Test 1", () => {
    ///                 // first test in suite
    ///             }),
    ///             new TestCase("Test 2", () => {
    ///                // second test in suite
    ///             }),
    ///             new TestFixtureReference(typeof(OtherFixtureToIncludeInSuite))
    ///         }
    ///     };
    /// }
    /// ]]></code>
    /// </para>
    /// </example>
    /// <seealso cref="StaticTestFactoryAttribute"/>
    /// <seealso cref="DynamicTestFactoryAttribute"/>
    /// <seealso cref="TestCase"/>
    /// <seealso cref="TestSuite"/>
    /// <seealso cref="TestFixtureReference"/>
    public abstract class Test
    {
        /// <summary>
        /// Builds a collection of static tests during test exploration.
        /// </summary>
        /// <param name="tests">The enumeration of tests to build as children of the containing scope.</param>
        /// <param name="containingScope">The containing pattern evaluation scope.</param>
        /// <param name="declaringCodeElement">The code element that represents the scope in which the test was defined.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="containingScope"/>, <paramref name="declaringCodeElement"/>
        /// or <paramref name="tests"/> is null or contains a null.</exception>
        /// <seealso cref="StaticTestFactoryAttribute" />
        public static void BuildStaticTests(IEnumerable<Test> tests, IPatternScope containingScope, ICodeElementInfo declaringCodeElement)
        {
            if (containingScope == null)
                throw new ArgumentNullException("containingScope");
            if (declaringCodeElement == null)
                throw new ArgumentNullException("declaringCodeElement");
            if (tests == null)
                throw new ArgumentNullException("tests");
            if (GenericCollectionUtils.Exists(tests, test => test == null))
                throw new ArgumentNullException("tests", "Test enumeration should not contain null.");

            // HACK: Preserve exact test ordering.  No easy way to decorate all newly created tests at this time
            //       so we assume newly added ones must be at the end.
            var originalChildren = containingScope.TestBuilder.ToTest().Children;
            int originalCount = originalChildren.Count;

            foreach (Test test in tests)
                test.BuildStaticTest(containingScope, declaringCodeElement);

            var children = containingScope.TestBuilder.ToTest().Children;
            for (int i = originalCount; i < children.Count; i++)
                children[i].Order = i;
        }

        /// <summary>
        /// Runs a collection of dynamic tests during test execution.
        /// </summary>
        /// <param name="tests">The enumeration of tests to run.</param>
        /// <param name="declaringCodeElement">The code element that represents the scope in which the test was defined.</param>
        /// <param name="setUp">Optional set-up code to run before the test, or null if none.</param>
        /// <param name="tearDown">Optional tear-down code to run after the test, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="declaringCodeElement"/> or
        /// <paramref name="tests"/> is null or contains a null.</exception>
        /// <seealso cref="DynamicTestFactoryAttribute" />
        [SystemInternal]
        public static TestOutcome RunDynamicTests(IEnumerable<Test> tests, ICodeElementInfo declaringCodeElement, Action setUp, Action tearDown)
        {
            if (declaringCodeElement == null)
                throw new ArgumentNullException("declaringCodeElement");
            if (tests == null)
                throw new ArgumentNullException("tests");
            if (GenericCollectionUtils.Exists(tests, test => test == null))
                throw new ArgumentNullException("tests", "Test enumeration should not contain null.");

            TestOutcome combinedOutcome = TestOutcome.Passed;
            foreach (Test test in tests)
                combinedOutcome = combinedOutcome.CombineWith(test.RunDynamicTest(declaringCodeElement, setUp, tearDown));

            return combinedOutcome;
        }

        /// <summary>
        /// Builds a static test during test exploration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Subclasses may override this behavior to change how the static test
        /// gets added to the test model.
        /// </para>
        /// </remarks>
        /// <param name="containingScope">The containing pattern evaluation scope.</param>
        /// <param name="declaringCodeElement">The code element that represents the scope in which the test was defined.</param>
        /// <seealso cref="StaticTestFactoryAttribute" />
        protected abstract void BuildStaticTest(IPatternScope containingScope, ICodeElementInfo declaringCodeElement);

        /// <summary>
        /// Runs a dynamic test during test execution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Subclasses may override this behavior to change how the dynamic test
        /// is executed as a test step.
        /// </para>
        /// </remarks>
        /// <param name="declaringCodeElement">The code element that represents the scope in which the test was defined.</param>
        /// <param name="setUp">Optional set-up code to run before the test, or null if none.</param>
        /// <param name="tearDown">Optional tear-down code to run after the test, or null if none.</param>
        /// <seealso cref="DynamicTestFactoryAttribute" />
        [SystemInternal]
        protected abstract TestOutcome RunDynamicTest(ICodeElementInfo declaringCodeElement, Action setUp, Action tearDown);
    }
}
