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

using System;
using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Framework.Pattern;
using Gallio.Utilities;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A pattern evaluator maintains state that is needed to interpret patterns.  It is
    /// used by the <see cref="PatternTestExplorer" /> to populate a <see cref="TestModel" />
    /// during test enumeration.
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    public class PatternEvaluator
    {
        private readonly TestModel testModel;
        private readonly IPatternResolver patternResolver;

        private readonly MultiMap<ICodeElementInfo, PatternEvaluationScope> registeredScopes;
        private event Action finishModel;

        /// <summary>
        /// Creates a pattern evaluator.
        /// </summary>
        /// <param name="testModel">The test model to build on</param>
        /// <param name="patternResolver">The pattern resolver</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModel"/> or
        /// <paramref name="patternResolver"/> is null</exception>
        public PatternEvaluator(TestModel testModel, IPatternResolver patternResolver)
        {
            if (testModel == null)
                throw new ArgumentNullException("testModel");
            if (patternResolver == null)
                throw new ArgumentNullException("patternResolver");

            this.testModel = testModel;
            this.patternResolver = patternResolver;

            registeredScopes = new MultiMap<ICodeElementInfo, PatternEvaluationScope>();
        }

        /// <summary>
        /// Gets the test model being built.
        /// </summary>
        public TestModel TestModel
        {
            get { return testModel; }
        }

        /// <summary>
        /// Gets the reflection policy for the model.
        /// </summary>
        public IReflectionPolicy ReflectionPolicy
        {
            get { return testModel.TestPackage.ReflectionPolicy; }
        }

        /// <summary>
        /// Adds a test as a child of the root test.
        /// </summary>
        /// <param name="test">The test to add</param>
        /// <returns>The test's scope</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null</exception>
        public PatternEvaluationScope AddTest(PatternTest test)
        {
            if (test == null)
                throw new ArgumentNullException("test");

            PatternEvaluationScope scope = new PatternEvaluationScope(this, null, test.CodeElement, test, null, test.DataContext, true);
            RegisterScope(scope);
            testModel.RootTest.AddChild(test);
            return scope;
        }

        /// <summary>
        /// Registers the scope so that it can be resolved later by <see cref="GetScopes" />.
        /// </summary>
        /// <param name="scope">The scope</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="scope"/> is null</exception>
        public void RegisterScope(PatternEvaluationScope scope)
        {
            if (scope == null)
                throw new ArgumentNullException("scope");

            ICodeElementInfo codeElement = scope.CodeElement;
            if (codeElement != null)
                registeredScopes.Add(codeElement, scope);
        }

        /// <summary>
        /// Finds scopes that are associated with the specified <see cref="ICodeElementInfo" />
        /// and returns an enumeration of their <see cref="PatternEvaluationScope"/> objects.
        /// </summary>
        /// <param name="codeElement">The code element</param>
        /// <returns>The enumeration of scopes</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        public IEnumerable<PatternEvaluationScope> GetScopes(ICodeElementInfo codeElement)
        {
            if (codeElement == null)
                return EmptyArray<PatternEvaluationScope>.Instance;
            return registeredScopes[codeElement];
        }

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
        public bool IsTest(ICodeElementInfo codeElement, IPattern defaultPrimaryPattern)
        {
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");

            try
            {
                IPattern primaryPattern = GetPrimaryPattern(codeElement) ?? defaultPrimaryPattern;
                if (primaryPattern != null)
                    return primaryPattern.IsTest(this, codeElement);
            }
            catch (Exception ex)
            {
                PublishExceptionAsAnnotation(codeElement, ex);
            }

            return false;
        }

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
        public void Consume(PatternEvaluationScope containingScope, ICodeElementInfo codeElement, bool skipChildren, IPattern defaultPrimaryPattern)
        {
            if (containingScope == null)
                throw new ArgumentNullException("containingScope");
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");

            try
            {
                IPattern primaryPattern = GetPrimaryPattern(codeElement) ?? defaultPrimaryPattern;
                if (primaryPattern != null)
                    primaryPattern.Consume(containingScope, codeElement, skipChildren);
            }
            catch (Exception ex)
            {
                PublishExceptionAsAnnotation(codeElement, ex);
            }
        }

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
        public void Process(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");

            try
            {
                foreach (IPattern pattern in patternResolver.GetPatterns(codeElement))
                    pattern.Process(scope, codeElement);
            }
            catch (Exception ex)
            {
                PublishExceptionAsAnnotation(codeElement, ex);
            }
        }

        /// <summary>
        /// Returns true if a code element has one or more associated patterns.
        /// </summary>
        /// <param name="codeElement">The code element</param>
        /// <returns>True if the code element has an associated pattern</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        public bool HasPatterns(ICodeElementInfo codeElement)
        {
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");

            return patternResolver.GetPatterns(codeElement).GetEnumerator().MoveNext();
        }

        /// <summary>
        /// Gets all patterns associated with a code element.
        /// </summary>
        /// <param name="codeElement">The code element</param>
        /// <returns>The enumeration of patterns, or null if none</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        public IEnumerable<IPattern> GetPatterns(ICodeElementInfo codeElement)
        {
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");

            return patternResolver.GetPatterns(codeElement);
        }

        /// <summary>
        /// Gets the primary pattern associated with a code element, or null if none.
        /// </summary>
        /// <param name="codeElement">The code element</param>
        /// <returns>The primary pattern, or null if none</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        /// <exception cref="PatternUsageErrorException">Thrown if there are multiple primary patterns associated with the code element</exception>
        public IPattern GetPrimaryPattern(ICodeElementInfo codeElement)
        {
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");

            IPattern primaryPattern = null;
            foreach (IPattern pattern in patternResolver.GetPatterns(codeElement))
            {
                if (pattern.IsPrimary)
                {
                    if (primaryPattern != null)
                        throw new PatternUsageErrorException(String.Format("There are multiple primary patterns associated with the same code element.  Perhaps it has inappropriate attributes."));
                    primaryPattern = pattern;
                }
            }

            return primaryPattern;
        }

        /// <summary>
        /// Registers a deferred action to be performed when <see cref="FinishModel" /> is called.
        /// </summary>
        /// <param name="codeElement">The associated code element, use to report errors if the action throws an exception</param>
        /// <param name="action">The action to perform</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/>
        /// or <paramref name="action"/> is null</exception>
        /// <seealso cref="FinishModel"/>
        public void AddFinishModelAction(ICodeElementInfo codeElement, Action action)
        {
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");
            if (action == null)
                throw new ArgumentNullException("action");

            finishModel += delegate
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    PublishExceptionAsAnnotation(codeElement, ex);
                }
            };
        }

        /// <summary>
        /// Applies all pending decorators and clears the list of pending decorators.
        /// </summary>
        public void FinishModel()
        {
            if (finishModel != null)
            {
                finishModel();
                finishModel = null;
            }
        }

        private void PublishExceptionAsAnnotation(ICodeElementInfo codeElement, Exception ex)
        {
            if (ex is PatternUsageErrorException)
            {
                testModel.AddAnnotation(new Annotation(AnnotationType.Error, codeElement,
                    ex.Message,
                    ex.InnerException != null ? ExceptionUtils.SafeToString(ex) : null));
            }
            else
            {
                testModel.AddAnnotation(new Annotation(AnnotationType.Error, codeElement,
                    "An exception occurred while evaluating a pattern.",
                    ExceptionUtils.SafeToString(ex)));
            }
        }
    }
}
