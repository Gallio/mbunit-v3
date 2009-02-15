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
using Gallio.Model;
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Default implementation of a pattern scope.
    /// </summary>
    public class DefaultPatternScope : IPatternScope
    {
        private readonly IPatternEvaluator evaluator;
        private readonly ICodeElementInfo codeElement;
        private readonly ITestBuilder testBuilder;
        private readonly ITestParameterBuilder testParameterBuilder;
        private readonly ITestDataContextBuilder testDataContextBuilder;
        private readonly bool isDeclaration;

        private List<DeferredComponentPopulator> populators;

        /// <summary>
        /// Creates a new scope.
        /// </summary>
        /// <param name="evaluator">The pattern evaluator</param>
        /// <param name="codeElement">The code element associated with the scope, or null if none</param>
        /// <param name="testBuilder">The test builder in scope</param>
        /// <param name="testParameterBuilder">The test parameter builder in scope, or null if none</param>
        /// <param name="testDataContextBuilder">The test data context builder</param>
        /// <param name="isDeclaration">True if the scope represents the initial point of declaration
        /// of a given test component</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="evaluator"/>,
        /// <paramref name="testBuilder"/> or <paramref name="testDataContextBuilder"/> is null</exception>
        public DefaultPatternScope(IPatternEvaluator evaluator, ICodeElementInfo codeElement,
            ITestBuilder testBuilder, ITestParameterBuilder testParameterBuilder,
            ITestDataContextBuilder testDataContextBuilder, bool isDeclaration)
        {
            if (evaluator == null)
                throw new ArgumentNullException("evaluator");
            if (testBuilder == null)
                throw new ArgumentNullException("testBuilder");
            if (testDataContextBuilder == null)
                throw new ArgumentNullException("testDataContextBuilder");

            this.evaluator = evaluator;
            this.codeElement = codeElement;
            this.testBuilder = testBuilder;
            this.testParameterBuilder = testParameterBuilder;
            this.testDataContextBuilder = testDataContextBuilder;
            this.isDeclaration = isDeclaration;
        }

        /// <inheritdoc />
        public IPatternEvaluator Evaluator
        {
            get { return evaluator; }
        }

        /// <inheritdoc />
        public ICodeElementInfo CodeElement
        {
            get { return codeElement; }
        }

        /// <inheritdoc />
        public bool IsTestDeclaration
        {
            get { return isDeclaration && testParameterBuilder == null; }
        }

        /// <inheritdoc />
        public bool IsTestParameterDeclaration
        {
            get { return isDeclaration && testParameterBuilder != null; }
        }

        /// <inheritdoc />
        public bool CanAddChildTest
        {
            get { return testParameterBuilder == null; }
        }

        /// <inheritdoc />
        public bool CanAddTestParameter
        {
            get { return testParameterBuilder == null; }
        }

        /// <inheritdoc />
        public ITestBuilder TestBuilder
        {
            get { return testBuilder; }
        }

        /// <inheritdoc />
        public ITestParameterBuilder TestParameterBuilder
        {
            get { return testParameterBuilder; }
        }

        /// <inheritdoc />
        public ITestComponentBuilder TestComponentBuilder
        {
            get { return (ITestComponentBuilder)testParameterBuilder ?? testBuilder; }
        }

        /// <inheritdoc />
        public ITestDataContextBuilder TestDataContextBuilder
        {
            get { return testDataContextBuilder; }
        }

        /// <inheritdoc />
        public ITestModelBuilder TestModelBuilder
        {
            get { return evaluator.TestModelBuilder; }
        }

        /// <inheritdoc />
        public IPatternScope CreateScope(ICodeElementInfo codeElement,
            ITestBuilder testBuilder, ITestParameterBuilder testParameterBuilder, ITestDataContextBuilder testDataContextBuilder, bool isDeclaration)
        {
            var scope = new DefaultPatternScope(evaluator, codeElement, testBuilder, testParameterBuilder, testDataContextBuilder, isDeclaration);
            evaluator.RegisterScope(scope);
            return scope;
        }

        /// <inheritdoc />
        public IPatternScope CreateChildTestScope(string name, ICodeElementInfo codeElement)
        {
            if (!CanAddChildTest)
                throw new PatternUsageErrorException("Cannot add child tests to the test within this scope.");
            
            ITestDataContextBuilder childTestDataContextBuilder = testDataContextBuilder.CreateChild();
            ITestBuilder childTestBuilder = testBuilder.CreateChild(name, codeElement, childTestDataContextBuilder);
            return CreateScope(codeElement, childTestBuilder, null, childTestDataContextBuilder, true);
        }

        /// <inheritdoc />
        public IPatternScope CreateTestParameterScope(string name, ICodeElementInfo codeElement)
        {
            if (!CanAddTestParameter)
                throw new InvalidOperationException("Cannot add test parameters to the test within this scope.");

            ITestDataContextBuilder childTestDataContextBuilder = testDataContextBuilder.CreateChild();
            ITestParameterBuilder testParameterBuilder = testBuilder.CreateParameter(name, codeElement, childTestDataContextBuilder);
            return CreateScope(codeElement, testBuilder, testParameterBuilder, childTestDataContextBuilder, true);
        }

        /// <inheritdoc />
        public IPatternScope CreateChildTestDataContextScope(ICodeElementInfo codeElement)
        {
            ITestDataContextBuilder childTestDataContextBuilder = testDataContextBuilder.CreateChild();
            return CreateScope(codeElement, testBuilder, testParameterBuilder, childTestDataContextBuilder, false);
        }

        /// <inheritdoc />
        public void AddDeferredComponentPopulator(DeferredComponentPopulator populator)
        {
            if (populator == null)
                throw new ArgumentNullException("populator");

            if (populators == null)
                populators = new List<DeferredComponentPopulator>();

            populators.Add(populator);
        }

        /// <inheritdoc />
        public void PopulateDeferredComponents(ICodeElementInfo codeElementHint)
        {
            if (populators != null)
            {
                foreach (DeferredComponentPopulator populator in populators)
                    populator(codeElementHint);
            }
        }

        /// <inheritdoc />
        public void Consume(ICodeElementInfo codeElement, bool skipChildren, IPattern defaultPrimaryPattern)
        {
            evaluator.Consume(this, codeElement, skipChildren, defaultPrimaryPattern);
        }

        /// <inheritdoc />
        public void Process(ICodeElementInfo codeElement)
        {
            evaluator.Process(this, codeElement);
        }
    }
}
