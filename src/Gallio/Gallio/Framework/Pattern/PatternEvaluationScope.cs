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
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A pattern evaluation scope defines the scope within which a pattern
    /// is being evaluated.  It provides references to declared entities
    /// such as tests and test parameters that are being manipulated
    /// and extended by an <see cref="IPattern"/>.
    /// </summary>
    public class PatternEvaluationScope
    {
        private readonly PatternEvaluator evaluator;
        private readonly PatternEvaluationScope parent;
        private readonly ICodeElementInfo codeElement;
        private readonly PatternTest test;
        private readonly PatternTestParameter testParameter;
        private readonly PatternTestDataContext testDataContext;
        private readonly bool isDeclaration;

        private ActionChain<ICodeElementInfo> populateChildrenChain;
        private List<KeyValuePair<int, Action<PatternEvaluationScope>>> decorators;

        internal PatternEvaluationScope(PatternEvaluator evaluator, PatternEvaluationScope parent,
            ICodeElementInfo codeElement, PatternTest test, PatternTestParameter testParameter,
            PatternTestDataContext testDataContext, bool isDeclaration)
        {
            this.evaluator = evaluator;
            this.parent = parent;
            this.codeElement = codeElement;
            this.test = test;
            this.testParameter = testParameter;
            this.testDataContext = testDataContext;
            this.isDeclaration = isDeclaration;
        }

        /// <summary>
        /// Gets the pattern evaluator.
        /// </summary>
        public PatternEvaluator Evaluator
        {
            get { return evaluator; }
        }

        /// <summary>
        /// Gets the parent scope, or null if none.
        /// </summary>
        public PatternEvaluationScope Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Gets the code element associated with the scope, or null if none.
        /// </summary>
        public ICodeElementInfo CodeElement
        {
            get { return codeElement; }
        }

        /// <summary>
        /// <para>
        /// Returns true if the scope represents a test declaration.
        /// </para>
        /// <para>
        /// An arbitrary scope nested within a test declaration scope is not itself considered a
        /// test declaration scope unless it also represents a test declaration.
        /// </para>
        /// </summary>
        public bool IsTestDeclaration
        {
            get { return isDeclaration && testParameter == null; }
        }

        /// <summary>
        /// <para>
        /// Returns true if the scope represents a test parameter declaration.
        /// </para>
        /// <para>
        /// An arbitrary scope nested within a test parameter declaration scope is not itself considered a
        /// test parameter declaration scope unless it also represents a test parameter declaration.
        /// </para>
        /// </summary>
        public bool IsTestParameterDeclaration
        {
            get { return isDeclaration && testParameter != null; }
        }

        /// <summary>
        /// Returns true if a child test can be added to the test within scope.
        /// </summary>
        public bool CanAddChildTest
        {
            get { return test != null && testParameter == null; }
        }

        /// <summary>
        /// Returns true if a test parameter can be added to the test within scope.
        /// </summary>
        public bool CanAddTestParameter
        {
            get { return test != null && testParameter == null; }
        }

        /// <summary>
        /// Gets the innermost test declared by this scope or by one of its ancestors,
        /// or null if there is none.
        /// </summary>
        public PatternTest Test
        {
            get { return test; }
        }

        /// <summary>
        /// Gets the innermost test parameter declared by this scope of by one of its ancestors,
        /// or null if there is none.
        /// </summary>
        public PatternTestParameter TestParameter
        {
            get { return testParameter; }
        }

        /// <summary>
        /// <para>
        /// Gets the innermost test component declared by this scope or by one of its ancestors,
        /// or null if there is none.
        /// </para>
        /// <para>
        /// If the scope has a <see cref="TestParameter"/> then it is returned, otherwise
        /// the <see cref="Test"/> is returned if there is one.
        /// </para>
        /// </summary>
        public IPatternTestComponent TestComponent
        {
            get
            {
                if (testParameter != null)
                    return testParameter;
                return test;
            }
        }

        /// <summary>
        /// Gets the test data context for this scope.
        /// </summary>
        public PatternTestDataContext TestDataContext
        {
            get { return testDataContext; }
        }

        /// <summary>
        /// <para>
        /// Gets a chain of actions that are used to lazily populate child tests.
        /// The chain should do nothing if the children have already been populated.
        /// </para>
        /// <para>
        /// The action's parameter specified the code element whose patterns
        /// will declare the children to be populated.  If its value is null, then
        /// all children should be populated.
        /// </para>
        /// </summary>
        public ActionChain<ICodeElementInfo> PopulateChildrenChain
        {
            get
            {
                if (populateChildrenChain == null)
                    populateChildrenChain = new ActionChain<ICodeElementInfo>();
                return populateChildrenChain;
            }
        }

        /// <summary>
        /// Registers a deferred decorator action to be applied to the scope later,
        /// pending the addition of all decorators so that they may be applied in order.
        /// </summary>
        /// <param name="order">The order in which the decorator should be evaluated,
        /// decorators with lower order indices are evaluated before those with
        /// higher ones</param>
        /// <param name="decorator">The decorator action</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="decorator"/> is null</exception>
        /// <seealso cref="ApplyDecorators"/>
        public void AddDecorator(int order, Action<PatternEvaluationScope> decorator)
        {
            if (decorator == null)
                throw new ArgumentNullException("decorator");

            if (decorators == null)
                decorators = new List<KeyValuePair<int, Action<PatternEvaluationScope>>>();
            decorators.Add(new KeyValuePair<int, Action<PatternEvaluationScope>>(order, decorator));
        }

        /// <summary>
        /// Applies all pending decorators and clears the list of pending decorators.
        /// </summary>
        public void ApplyDecorators()
        {
            if (decorators == null)
                return;

            decorators.Sort(delegate(KeyValuePair<int, Action<PatternEvaluationScope>> x,
                KeyValuePair<int, Action<PatternEvaluationScope>> y)
            {
                return x.Key.CompareTo(y.Key);
            });

            foreach (KeyValuePair<int, Action<PatternEvaluationScope>> entry in decorators)
                entry.Value(this);

            decorators = null;
        }

        /// <summary>
        /// Adds a test as a child of the scoped test.
        /// </summary>
        /// <param name="test">The test to add</param>
        /// <returns>The child test's scope</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="CanAddChildTest"/> is false</exception>
        public PatternEvaluationScope AddChildTest(PatternTest test)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            if (!CanAddChildTest)
                throw new InvalidOperationException("Cannot add child tests to the test within this scope.");

            PatternEvaluationScope scope = new PatternEvaluationScope(evaluator, this, test.CodeElement, test, null, test.DataContext, true);
            evaluator.RegisterScope(scope);
            this.test.AddChild(test);
            return scope;
        }

        /// <summary>
        /// Adds a test parameter to the scoped test.
        /// </summary>
        /// <param name="testParameter">The test parameter to add</param>
        /// <returns>The test parameter's scope</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testParameter"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="CanAddTestParameter"/> is false</exception>
        public PatternEvaluationScope AddTestParameter(PatternTestParameter testParameter)
        {
            if (testParameter == null)
                throw new ArgumentNullException("testParameter");
            if (!CanAddTestParameter)
                throw new InvalidOperationException("Cannot add test parameters to the test within this scope.");

            PatternEvaluationScope scope = new PatternEvaluationScope(evaluator, this, testParameter.CodeElement, test, testParameter, testParameter.DataContext, true);
            evaluator.RegisterScope(scope);
            test.AddParameter(testParameter);
            return scope;
        }

        /// <summary>
        /// Creates a child scope to represent a new test data context.
        /// </summary>
        /// <param name="testDataContext">The test data context</param>
        /// <returns>The child scope</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testDataContext"/> is null</exception>
        public PatternEvaluationScope EnterTestDataContext(PatternTestDataContext testDataContext)
        {
            if (testDataContext == null)
                throw new ArgumentNullException("testDataContext");

            PatternEvaluationScope childScope = new PatternEvaluationScope(evaluator, this, test.CodeElement, test,
                testParameter, testDataContext, false);
            evaluator.RegisterScope(childScope);
            return childScope;
        }

        /// <summary>
        /// Consumes the specified code element using this scope as the containing scope.
        /// </summary>
        /// <remarks>
        /// This is a convenience method that calls <see cref="PatternEvaluator.Consume" />.
        /// </remarks>
        /// <param name="codeElement">The code element to consume</param>
        /// <param name="skipChildren">If true, instructs the primary pattern to defer populating child tests</param>
        /// <param name="defaultPrimaryPattern">The default primary pattern to use, if none can be resolved
        /// for the code element.  May be null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        public void Consume(ICodeElementInfo codeElement, bool skipChildren, IPattern defaultPrimaryPattern)
        {
            evaluator.Consume(this, codeElement, skipChildren, defaultPrimaryPattern);
        }

        /// <summary>
        /// Processes the specified code element using this scope as the current scope.
        /// </summary>
        /// <remarks>
        /// This is a convenience method that calls <see cref="PatternEvaluator.Process" />.
        /// </remarks>
        /// <param name="codeElement">The code element to process</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        public void Process(ICodeElementInfo codeElement)
        {
            evaluator.Process(this, codeElement);
        }
    }
}
