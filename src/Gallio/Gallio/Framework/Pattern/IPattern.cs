// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// A <see cref="IPattern" /> defines a composable rule for building
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
        /// Returns true if this is a primary pattern.
        /// </para>
        /// <para>
        /// A primary pattern is a pattern that defines the ultimate purpose of a code
        /// element and the nature of the tests or test parameters that are produced from it.
        /// Consequently the primary pattern is the only one on which the <see cref="Consume" />
        /// method will be called.
        /// </para>
        /// <para>
        /// Each code element may have at most one primary pattern.  It is an error
        /// for a code element to have more than one associated primary pattern.
        /// </para>
        /// <para>
        /// If a code element does not have an associated primary pattern, its containing
        /// test may choose to apply default processing to it instead.  For example, the
        /// containing test may give an opportunity to patterns associated with the code element
        /// to consume a test method parameter but will revert to default behavior if no
        /// primary patterns explicitly claim it.
        /// </para>
        /// <para>
        /// Non-primary patterns still play a very important part in the construction
        /// of the test model.  Non-primary patterns may implement <see cref="ProcessTest" /> and
        /// <see cref="ProcessTestParameter" /> to decorate tests and test parameters
        /// created by the primary pattern.
        /// </para>
        /// </summary>
        bool IsPrimary { get; }

        /// <summary>
        /// <para>
        /// Returns true if the code element associated with the pattern represents a test.
        /// </para>
        /// </summary>
        /// <param name="patternResolver">The pattern resolver</param>
        /// <param name="codeElement">The code element</param>
        /// <returns>True if the code element represents a test</returns>
        bool IsTest(IPatternResolver patternResolver, ICodeElementInfo codeElement);

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
        /// <remarks>
        /// <para>
        /// This method is only called for primary patterns.
        /// </para>
        /// </remarks>
        /// <param name="containingTestBuilder">The containing test builder</param>
        /// <param name="codeElement">The code element to process</param>
        /// <param name="skipChildren">If true, skips generating child tests.  Instead the children may
        /// be populated on demand using <see cref="IPatternTestBuilder.PopulateChildrenChain" />.  The implementation
        /// may safely ignore the value of this flag so long as subsequent attempts to populate children on
        /// demand have no adverse side-effects.</param>
        /// <seealso cref="IsPrimary"/>
        void Consume(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement, bool skipChildren);

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