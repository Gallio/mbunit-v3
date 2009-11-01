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
using Gallio.Common.Reflection;
using Gallio.Model.Tree;
using Gallio.Model;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A pattern evaluator maintains state that is needed to interpret patterns.  
    /// </summary>
    /// <remarks>
    /// <para>
    /// It is used by the <see cref="PatternTestDriver" /> to populate a <see cref="TestModel" />
    /// during test enumeration.
    /// </para>
    /// </remarks>
    /// <seealso cref="PatternTestFramework"/>
    public interface IPatternEvaluator
    {
        /// <summary>
        /// Gets the builder for the test model under construction.
        /// </summary>
        ITestModelBuilder TestModelBuilder { get; }

        /// <summary>
        /// Gets the scope of the root test.
        /// </summary>
        /// <returns>The root scope.</returns>
        IPatternScope RootScope { get; }

        /// <summary>
        /// Registers the scope so that it can be resolved later by <see cref="IPatternEvaluator.GetScopes" />.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="scope"/> is null.</exception>
        void RegisterScope(IPatternScope scope);

        /// <summary>
        /// Finds scopes that are associated with the specified <see cref="ICodeElementInfo" />
        /// and returns an enumeration of their <see cref="IPatternScope"/> objects.
        /// </summary>
        /// <param name="codeElement">The code element.</param>
        /// <returns>The enumeration of scopes.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null.</exception>
        IEnumerable<IPatternScope> GetScopes(ICodeElementInfo codeElement);

        /// <summary>
        /// Finds tests that are declared by the specified <see cref="ICodeElementInfo" />.
        /// </summary>
        /// <param name="codeElement">The code element.</param>
        /// <returns>The enumeration of tests.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null.</exception>
        IEnumerable<PatternTest> GetDeclaredTests(ICodeElementInfo codeElement);

        /// <summary>
        /// Gets the test parts represented by a code element.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If an exception occurs while enumerating the pattern attributes,
        /// this method returns an empty array.
        /// </para>
        /// </remarks>
        /// <param name="codeElement">The code element.</param>
        /// <param name="defaultPrimaryPattern">The default primary pattern to use, if none can be resolved
        /// for the code element.  May be null if none.</param>
        /// <returns>The test parts, or an empty array if none.</returns>
        /// <seealso cref="IPattern.GetTestParts"/>
        IList<TestPart> GetTestParts(ICodeElementInfo codeElement, IPattern defaultPrimaryPattern);

        /// <summary>
        /// Consumes the specified code element.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Any exceptions thrown by a pattern are caught and manifested as annotations
        /// associated with the code element.  <seealso cref="PatternUsageErrorException"/>
        /// </para>
        /// </remarks>
        /// <param name="containingScope">The containing scope.</param>
        /// <param name="codeElement">The code element to consume.</param>
        /// <param name="skipChildren">If true, instructs the primary pattern to defer populating child tests.</param>
        /// <param name="defaultPrimaryPattern">The default primary pattern to use, if none can be resolved
        /// for the code element.  May be null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="containingScope"/>
        /// or <paramref name="codeElement"/> is null.</exception>
        /// <seealso cref="IPattern.Consume"/>
        void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren, IPattern defaultPrimaryPattern);

        /// <summary>
        /// Processes the specified code element.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Any exceptions thrown by a pattern are caught and manifested as annotations
        /// associated with the code element.  <seealso cref="PatternUsageErrorException"/>
        /// </para>
        /// </remarks>
        /// <param name="scope">The scope.</param>
        /// <param name="codeElement">The code element to process.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null.</exception>
        /// <seealso cref="IPattern.Process"/>
        void Process(IPatternScope scope, ICodeElementInfo codeElement);

        /// <summary>
        /// Returns true if a code element has one or more associated patterns.
        /// </summary>
        /// <param name="codeElement">The code element.</param>
        /// <returns>True if the code element has an associated pattern.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null.</exception>
        bool HasPatterns(ICodeElementInfo codeElement);

        /// <summary>
        /// Gets all patterns associated with a code element.
        /// </summary>
        /// <param name="codeElement">The code element.</param>
        /// <returns>The enumeration of patterns, or null if none.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null.</exception>
        IEnumerable<IPattern> GetPatterns(ICodeElementInfo codeElement);

        /// <summary>
        /// Gets the primary pattern associated with a code element, or null if none.
        /// </summary>
        /// <param name="codeElement">The code element.</param>
        /// <returns>The primary pattern, or null if none.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null.</exception>
        /// <exception cref="PatternUsageErrorException">Thrown if there are multiple primary patterns associated with the code element.</exception>
        IPattern GetPrimaryPattern(ICodeElementInfo codeElement);
    }
}
