using System;
using System.Collections.Generic;
using Gallio.Model;
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A pattern evaluator maintains state that is needed to interpret patterns.  It is
    /// used by the <see cref="PatternTestExplorer" /> to populate a <see cref="TestModel" />
    /// during test enumeration.
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    public interface IPatternEvaluator
    {
        /// <summary>
        /// Gets the builder for the test model under construction.
        /// </summary>
        ITestModelBuilder TestModelBuilder { get; }

        /// <summary>
        /// Creates a top-level test as a child of the root test.
        /// Returns the scope of the newly created top-level test.
        /// </summary>
        /// <param name="name">The test name</param>
        /// <param name="codeElement">The associated code element, or null if none</param>
        /// <returns>The builder for the top-level test</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        IPatternScope CreateTopLevelTestScope(string name, ICodeElementInfo codeElement);

        /// <summary>
        /// Registers the scope so that it can be resolved later by <see cref="IPatternEvaluator.GetScopes" />.
        /// </summary>
        /// <param name="scope">The scope</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="scope"/> is null</exception>
        void RegisterScope(IPatternScope scope);

        /// <summary>
        /// Finds scopes that are associated with the specified <see cref="ICodeElementInfo" />
        /// and returns an enumeration of their <see cref="IPatternScope"/> objects.
        /// </summary>
        /// <param name="codeElement">The code element</param>
        /// <returns>The enumeration of scopes</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        IEnumerable<IPatternScope> GetScopes(ICodeElementInfo codeElement);

        /// <summary>
        /// Finds tests that are declared by the specified <see cref="ICodeElementInfo" />.
        /// </summary>
        /// <param name="codeElement">The code element</param>
        /// <returns>The enumeration of tests</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        IEnumerable<PatternTest> GetDeclaredTests(ICodeElementInfo codeElement);

        /// <summary>
        /// Returns true if the code element represents a test.
        /// </summary>
        /// <remarks>
        /// Any exceptions thrown by a pattern are caught and manifested as annotations
        /// associated with the code element.  <seealso cref="PatternUsageErrorException"/>
        /// </remarks>
        /// <param name="codeElement">The code element</param>
        /// <param name="defaultPrimaryPattern">The default primary pattern to use, if none can be resolved
        /// for the code element.  May be null if none.</param>
        /// <returns>True if the code element represents a test</returns>
        /// <seealso cref="IPattern.IsTest"/>
        bool IsTest(ICodeElementInfo codeElement, IPattern defaultPrimaryPattern);

        /// <summary>
        /// Consumes the specified code element.
        /// </summary>
        /// <remarks>
        /// Any exceptions thrown by a pattern are caught and manifested as annotations
        /// associated with the code element.  <seealso cref="PatternUsageErrorException"/>
        /// </remarks>
        /// <param name="containingScope">The containing scope</param>
        /// <param name="codeElement">The code element to consume</param>
        /// <param name="skipChildren">If true, instructs the primary pattern to defer populating child tests</param>
        /// <param name="defaultPrimaryPattern">The default primary pattern to use, if none can be resolved
        /// for the code element.  May be null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="containingScope"/>
        /// or <paramref name="codeElement"/> is null</exception>
        /// <seealso cref="IPattern.Consume"/>
        void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren, IPattern defaultPrimaryPattern);

        /// <summary>
        /// Processes the specified code element.
        /// </summary>
        /// <remarks>
        /// Any exceptions thrown by a pattern are caught and manifested as annotations
        /// associated with the code element.  <seealso cref="PatternUsageErrorException"/>
        /// </remarks>
        /// <param name="scope">The scope</param>
        /// <param name="codeElement">The code element to process</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        /// <seealso cref="IPattern.Process"/>
        void Process(IPatternScope scope, ICodeElementInfo codeElement);

        /// <summary>
        /// Returns true if a code element has one or more associated patterns.
        /// </summary>
        /// <param name="codeElement">The code element</param>
        /// <returns>True if the code element has an associated pattern</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        bool HasPatterns(ICodeElementInfo codeElement);

        /// <summary>
        /// Gets all patterns associated with a code element.
        /// </summary>
        /// <param name="codeElement">The code element</param>
        /// <returns>The enumeration of patterns, or null if none</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        IEnumerable<IPattern> GetPatterns(ICodeElementInfo codeElement);

        /// <summary>
        /// Gets the primary pattern associated with a code element, or null if none.
        /// </summary>
        /// <param name="codeElement">The code element</param>
        /// <returns>The primary pattern, or null if none</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        /// <exception cref="PatternUsageErrorException">Thrown if there are multiple primary patterns associated with the code element</exception>
        IPattern GetPrimaryPattern(ICodeElementInfo codeElement);
    }
}