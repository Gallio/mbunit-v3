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

using Gallio.Common.Reflection;
using System.Collections.Generic;
using Gallio.Model;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A <see cref="IPattern" /> defines a composable rule for building up a test model
    /// using reflection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The general idea is that a pattern applies contributions to a <see cref="IPatternScope" />
    /// that represents the state of the pattern interpretation process.  A primary pattern
    /// adds contributions to its containing scope with the <see cref="Consume"/> method.  All
    /// patterns (primary and non-primary) add further contributions to the pattern's own
    /// scope with the <see cref="Process" /> method.
    /// </para>
    /// <para>
    /// A pattern can also defer some of its processing by registering a decorator on the scope.
    /// Once all of the decorators have been gathered, they can be applied in sorted order
    /// as required.
    /// </para>
    /// <para>
    /// Pattern processing is performed recursively.  First the primary pattern for the assembly
    /// is found.  If none is registered then the default assembly pattern is used instead.
    /// The assembly pattern then performs reflection over the types within the assembly and hands
    /// off control (via <see cref="Consume"/>) to any primary patterns it finds each type.  Likewise the
    /// types scan their members and call into their primary patterns.  And so on.  Each primary pattern
    /// is responsible for invoking (via <see cref="Process" />) any additional non-primary patterns
    /// associated with their code element after they have established the appropriate evaluation scope.
    /// </para>
    /// <para>
    /// Typically a pattern is associated with a code element by means of a <see cref="PatternAttribute" />
    /// but other associations are possible.  Some patterns might define default rules for recursively
    /// processing code elements that do not have primary patterns of their own.  Others might use
    /// means other than standard reflection to discover the patterns to be applied.  The process is
    /// intended to be open and extensible.
    /// </para>
    /// </remarks>
    /// <seealso cref="PatternAttribute"/>
    /// <seealso cref="PatternTestFramework"/>
    public interface IPattern
    {
        /// <summary>
        /// Returns true if this is a primary pattern.
        /// </summary>
        /// <remarks>
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
        /// of the test model.  Non-primary patterns may implement the <see cref="Process" />
        /// method to decorate tests and test parameters declared by the primary pattern.
        /// </para>
        /// </remarks>
        bool IsPrimary { get; }

        /// <summary>
        /// Gets the test parts represented by a code element.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is only called for primary patterns.
        /// </para>
        /// </remarks>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="codeElement">The code element.</param>
        /// <returns>The test parts, or an empty array if none.</returns>
        /// <exception cref="PatternUsageErrorException">May be thrown to halt processing of the pattern
        /// and report an error message to the user as an annotation that describes how the
        /// pattern was misapplied.</exception>
        IList<TestPart> GetTestParts(IPatternEvaluator evaluator, ICodeElementInfo codeElement);

        /// <summary>
        /// Consumes a code element and applies its contributions to the scope
        /// provided by a containing pattern.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is used to declare new tests, test parameters and other components
        /// and add them to a containing test that was defined in some other scope.
        /// </para>
        /// <para>
        /// For example, when enumerating test fixtures, the assembly-level pattern will
        /// call the <see cref="Consume" /> method of the primary patterns associated
        /// with each type in an assembly.  Some of these patterns will create new test fixture
        /// objects and add them as children of the containing assembly-level test.  They will then
        /// call the <see cref="Process" /> method of each non-primary pattern associated with
        /// the type within the scope of the test fixture.  Then they will typically 
        /// recurse into the fixture to apply contributions defined by patterns associated
        /// with methods, fields, properties, events, constructors and generic type parameters.
        /// </para>
        /// <para>
        /// This method is only called for primary patterns.
        /// </para>
        /// </remarks>
        /// <param name="containingScope">The containing scope.</param>
        /// <param name="codeElement">The code element to process.</param>
        /// <param name="skipChildren">If true, skips generating child test components.
        /// Instead the children may be populated on demand using
        /// <see cref="IPatternScope.AddDeferredComponentPopulator" />.  The implementation
        /// may safely ignore the value of this flag so long as subsequent attempts to populate children on
        /// demand are idempotent (do nothing or have no adverse side-effects).</param>
        /// <seealso cref="IsPrimary"/>
        /// <exception cref="PatternUsageErrorException">May be thrown to halt processing of the pattern
        /// and report an error message to the user as an annotation that describes how the
        /// pattern was misapplied.</exception>
        void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren);

        /// <summary>
        /// Processes a code element and applies its contributions to the scope of this pattern.
        /// The scope will typically have been introduced by the <see cref="Consume" /> method
        /// of a primary pattern applied to this code element.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is used by patterns to decorate tests and test parameters that
        /// have been declared by primary patterns.
        /// </para>
        /// <para>
        /// For example, the <see cref="Process" /> method will typically be called by another
        /// pattern that has just created a new test based on the associated code element,
        /// such as a test method.  The method then has the opportunity to modify the test
        /// to add metadata, change its name, add new behaviors, and so on.
        /// </para>
        /// </remarks>
        /// <param name="scope">The scope.</param>
        /// <param name="codeElement">The code element to process.</param>
        /// <exception cref="PatternUsageErrorException">May be thrown to halt processing of the pattern
        /// and report an error message to the user as an annotation that describes how the
        /// pattern was misapplied.</exception>
        void Process(IPatternScope scope, ICodeElementInfo codeElement);
    }
}