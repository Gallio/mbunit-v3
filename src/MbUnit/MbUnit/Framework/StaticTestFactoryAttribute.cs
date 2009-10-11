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
using System.Reflection;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Model.Tree;

namespace MbUnit.Framework
{
    /// <summary>
    /// Specifies a method that is used to generate tests statically at
    /// test exploration time.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The tests created by the static test factory are considered to be children
    /// of the fixture that contains the factory method that generated them.
    /// Because the tests are created statically, they will appear in the test
    /// runner and they can be filtered as usual.  However, this means that the
    /// tests will be generated only when the fixture is being explored rather
    /// than each time the fixture is executed.  It also means that the factory
    /// method must be static and cannot be parameterized.
    /// </para>
    /// <para>
    /// Contrast with <see cref="DynamicTestFactoryAttribute" />.
    /// </para>
    /// <para>
    /// The method to which this attribute is applied must be declared by the
    /// fixture class.  It must be static and must not have any parameters.
    /// It must return an enumeration of values of type <see cref="Test" />.
    /// </para>
    /// <para>
    /// Tests built by a <see cref="StaticTestFactoryAttribute" /> will not be visible in ReSharper
    /// and other test runners that rely only on source code and metadata exclusively.
    /// This is because the test factory must be executed in order to populate the list of tests
    /// but the code might not be available in a compiled form during test exploration.  The
    /// tests should still run as part of the containing fixture although the results may
    /// only be visible in the full test report.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// A simple static test factory that reads some data from a file and generates
    /// a number of static tests.  The file is read at test exploration time,
    /// so if the file changes, the test package must be reloaded to obtain the new
    /// contents.
    /// <code><![CDATA[
    /// [StaticTestFactory]
    /// public static IEnumerable<Test> CreateStaticTests()
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
    /// </example>
    /// <seealso cref="DynamicTestFactoryAttribute"/>
    /// <seealso cref="TestCase"/>
    [AttributeUsage(PatternAttributeTargets.ContributionMethod, AllowMultiple = false, Inherited = true)]
    public class StaticTestFactoryAttribute : ContributionMethodPatternAttribute
    {
        /// <inheritdoc />
        protected override void Validate(IPatternScope containingScope, IMethodInfo method)
        {
            base.Validate(containingScope, method);

            if (!method.IsStatic)
                ThrowUsageErrorException("A static test factory method must be static.");
            if (method.Parameters.Count != 0)
                ThrowUsageErrorException("A static test factory method must not have any parameters.");
            if (method.ContainsGenericParameters)
                ThrowUsageErrorException("A static test factory method must not be generic.");
            if (method.DeclaringType.ContainsGenericParameters)
                ThrowUsageErrorException("A static test factory method must not be declared on a generic type.");
        }

        /// <inheritdoc />
        protected override void DecorateContainingScope(IPatternScope containingScope, IMethodInfo method)
        {
            MethodInfo factoryMethod = method.Resolve(false);
            if (factoryMethod == null || Reflector.IsUnresolved(factoryMethod))
            {
                containingScope.TestModelBuilder.AddAnnotation(new Annotation(AnnotationType.Info, method,
                    "This test runner does not fully support static test factory methods "
                    + "because the code that defines the factory cannot be executed "
                    + "at test exploration time.  Consider using dynamic test factory "
                    + "methods instead."));
                return;
            }

            var tests = factoryMethod.Invoke(null, null) as IEnumerable<Test>;
            if (tests == null)
            {
                containingScope.TestModelBuilder.AddAnnotation(new Annotation(AnnotationType.Error, method,
                    "Expected the static test factory method to return a value that is assignable "
                    + "to type IEnumerable<Test>."));
                return;
            }

            Test.BuildStaticTests(tests, containingScope, method);
        }
    }
}
