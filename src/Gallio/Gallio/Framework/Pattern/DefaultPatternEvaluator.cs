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
using Gallio.Collections;
using Gallio.Model;
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Default implementation of a pattern evaluator.
    /// </summary>
    public class DefaultPatternEvaluator : IPatternEvaluator
    {
        private readonly ITestModelBuilder testModelBuilder;
        private readonly IPatternResolver patternResolver;
        private readonly MultiMap<ICodeElementInfo, IPatternScope> registeredScopes;
        private readonly ITestDataContextBuilder rootDataContextBuilder;

        /// <summary>
        /// Creates a pattern evaluator.
        /// </summary>
        /// <param name="testModelBuilder">The test model builder</param>
        /// <param name="patternResolver">The pattern resolver</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModelBuilder"/> or
        /// <paramref name="patternResolver"/> is null</exception>
        public DefaultPatternEvaluator(ITestModelBuilder testModelBuilder, IPatternResolver patternResolver)
        {
            if (testModelBuilder == null)
                throw new ArgumentNullException("testModelBuilder");
            if (patternResolver == null)
                throw new ArgumentNullException("patternResolver");

            this.testModelBuilder = testModelBuilder;
            this.patternResolver = patternResolver;

            registeredScopes = new MultiMap<ICodeElementInfo, IPatternScope>();

            // Define an empty root data context so that all anonymous data source lookups terminate.
            rootDataContextBuilder = new DefaultTestDataContextBuilder(testModelBuilder, new PatternTestDataContext(null));
            rootDataContextBuilder.DefineDataSource("");
        }

        /// <inheritdoc />
        public ITestModelBuilder TestModelBuilder
        {
            get { return testModelBuilder; }
        }

        /// <inheritdoc />
        public IPatternScope CreateTopLevelTestScope(string name, ICodeElementInfo codeElement)
        {
            ITestDataContextBuilder testDataContextBuilder = rootDataContextBuilder.CreateChild();
            ITestBuilder testBuilder = testModelBuilder.CreateTopLevelTest(name, codeElement, testDataContextBuilder);

            var scope = new DefaultPatternScope(this, codeElement, testBuilder, null, testDataContextBuilder, true);
            RegisterScope(scope);
            return scope;
        }

        /// <inheritdoc />
        public void RegisterScope(IPatternScope scope)
        {
            if (scope == null)
                throw new ArgumentNullException("scope");

            ICodeElementInfo codeElement = scope.CodeElement;
            if (codeElement != null)
                registeredScopes.Add(codeElement, scope);
        }

        /// <inheritdoc />
        public IEnumerable<IPatternScope> GetScopes(ICodeElementInfo codeElement)
        {
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");

            return registeredScopes[codeElement];
        }

        /// <inheritdoc />
        public IEnumerable<PatternTest> GetDeclaredTests(ICodeElementInfo codeElement)
        {
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");

            foreach (IPatternScope scope in GetScopes(codeElement))
                if (scope.IsTestDeclaration)
                    yield return scope.TestBuilder.ToTest();
        }

        /// <inheritdoc />
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
                testModelBuilder.PublishExceptionAsAnnotation(codeElement, ex);
            }

            return false;
        }

        /// <inheritdoc />
        public void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren, IPattern defaultPrimaryPattern)
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
                testModelBuilder.PublishExceptionAsAnnotation(codeElement, ex);
            }
        }

        /// <inheritdoc />
        public void Process(IPatternScope scope, ICodeElementInfo codeElement)
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
                testModelBuilder.PublishExceptionAsAnnotation(codeElement, ex);
            }
        }

        /// <inheritdoc />
        public bool HasPatterns(ICodeElementInfo codeElement)
        {
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");

            return patternResolver.GetPatterns(codeElement).GetEnumerator().MoveNext();
        }

        /// <inheritdoc />
        public IEnumerable<IPattern> GetPatterns(ICodeElementInfo codeElement)
        {
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");

            return patternResolver.GetPatterns(codeElement);
        }

        /// <inheritdoc />
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
    }
}
