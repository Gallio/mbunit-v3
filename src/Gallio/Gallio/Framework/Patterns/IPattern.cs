// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

using Gallio.Reflection;
using Gallio.Framework.Explorer;
using Gallio.Framework.Patterns;

namespace Gallio.Framework.Patterns
{
    /// <summary>
    /// <para>
    /// An <see cref="IPattern" /> defines a composable rule for building
    /// <see cref="PatternTest" /> and <see cref="PatternTestParameter" /> objects
    /// using reflection.
    /// </para>
    /// <para>
    /// The general idea is that a pattern can apply any number of contributions to the
    /// <see cref="IPatternTestBuilder" /> or <see cref="IPatternTestParameterBuilder" />
    /// that represents the current scope of the process of constructing a
    /// test object model.  A pattern can register a decorator for the current object
    /// being built.  Once all decorators have been gathered, they are applied in
    /// sorted order.  Likewise a pattern can create new builders of its own
    /// then recurse back into the reflection layer to give a chance for other patterns
    /// to apply their own contributions.
    /// </para>
    /// <para>
    /// The entire pattern test model construction process is built up in this way.
    /// None of the rules are hardcoded except for bootstrapping via reflection.
    /// Typically a pattern will be associated with a code element by a
    /// <see cref="PatternAttribute" /> but new patterns can be created that look for other
    /// kinds of attributes or do other things.
    /// </para>
    /// </summary>
    /// <seealso cref="PatternAttribute"/>
    /// <seealso cref="PatternTestFramework"/>
    public interface IPattern
    {
        /// <summary>
        /// <para>
        /// Consumes the <paramref name="codeElement" /> and applies its contributions to
        /// the <paramref name="containingTestBuilder"/>.
        /// </para>
        /// <para>
        /// This method is used to declare new tests, test parameters and other components
        /// and add them to a containing test that was defined by some other <paramref name="codeElement" />.
        /// </para>
        /// <para>
        /// For example, when enumerating tests, the <see cref="Consume" />
        /// will call the <see cref="ProcessTest" /> method of all patterns associated
        /// with the public types in an assembly.  Some of these patterns will create new test fixture
        /// objects and add them as children of the containing assembly-level test.  They will then
        /// call <see cref="BootstrapAssemblyPattern" /> for each of the other patterns defined
        /// by this <paramref name="codeElement" />.  A test fixture pattern will then typically
        /// recurse into the fixture to apply contributions defined by patterns associated
        /// with methods, fields, properties, events, constructors and generic type parameters.
        /// </para>
        /// </summary>
        /// <param name="containingTestBuilder">The containing test builder</param>
        /// <param name="codeElement">The code element to process</param>
        /// <returns>True if the pattern has consumed the code element and
        /// will generate new test components from it.  False if the containing
        /// test should apply whatever default processing it may have to consume
        /// code element.  For example, the containing test may give an opportunity to
        /// patterns associated with the code element to consume a test method parameter
        /// but will revert to default behavior if no patterns claim it.</returns>
        bool Consume(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement);

        /// <summary>
        /// <para>
        /// Processes a test that was declared by a pattern associated with this
        /// <paramref name="codeElement" /> and applies contributions to its builder.
        /// </para>
        /// <para>
        /// This method is used to decorate tests defined by <paramref name="codeElement" /> itself.
        /// </para>
        /// <para>
        /// For example, <see cref="ProcessTest" /> will typically be called by another pattern that has
        /// just created a new test based on declarative information about the <paramref name="codeElement" />.
        /// The callee then has the opportunity to add decorators to the new test and to
        /// apply other contributions of its choosing.
        /// </para>
        /// </summary>
        /// <param name="testBuilder">The test builder</param>
        /// <param name="codeElement">The code element to process</param>
        void ProcessTest(IPatternTestBuilder testBuilder, ICodeElementInfo codeElement);

        /// <summary>
        /// <para>
        /// Processes a test parameter that was declared by a pattern associated with this
        /// <paramref name="codeElement" /> and applies contributions to its builder.
        /// </para>
        /// <para>
        /// This method is used to decorate test parameters defined by <paramref name="codeElement" /> itself.
        /// </para>
        /// <para>
        /// For example, <see cref="ProcessTestParameter" /> will typically be called by another
        /// pattern that has just created a new test parameter based on declarative information
        /// about the <paramref name="codeElement" />.  The callee then has the opportunity to add decorators to
        /// the new test parameter and to apply other contributions of its choosing.
        /// </para>
        /// </summary>
        /// <param name="testParameterBuilder">The test parameter builder</param>
        /// <param name="codeElement">The code element to process</param>
        void ProcessTestParameter(IPatternTestParameterBuilder testParameterBuilder, ICodeElementInfo codeElement);
    }
}