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
using Gallio.Model;
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A pattern scope describes the environment in which the pattern is being evaluated.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A pattern is always evaluated in some scope which determines its ultimate effect
    /// on declarations of tests, test parameters and test data contexts.
    /// </para>
    /// </remarks>
    public interface IPatternScope
    {
        /// <summary>
        /// Gets the pattern evaluator.
        /// </summary>
        IPatternEvaluator Evaluator { get; }

        /// <summary>
        /// Gets the code element associated with the scope, or null if none.
        /// </summary>
        ICodeElementInfo CodeElement { get; }

        /// <summary>
        /// <para>
        /// Returns true if the scope represents a test declaration.
        /// </para>
        /// <para>
        /// An arbitrary scope nested within a test declaration scope is not itself considered a
        /// test declaration scope unless it also represents a test declaration.
        /// </para>
        /// </summary>
        bool IsTestDeclaration { get; }

        /// <summary>
        /// <para>
        /// Returns true if the scope represents a test parameter declaration.
        /// </para>
        /// <para>
        /// An arbitrary scope nested within a test parameter declaration scope is not itself considered a
        /// test parameter declaration scope unless it also represents a test parameter declaration.
        /// </para>
        /// </summary>
        bool IsTestParameterDeclaration { get; }

        /// <summary>
        /// Returns true if a child test can be added to the test within scope.
        /// </summary>
        bool CanAddChildTest { get; }

        /// <summary>
        /// Returns true if a test parameter can be added to the test within scope.
        /// </summary>
        bool CanAddTestParameter { get; }

        /// <summary>
        /// Gets a builder for applying contributions to the test currently being
        /// constructed within this scope.  Never null.
        /// </summary>
        ITestBuilder TestBuilder { get; }

        /// <summary>
        /// Gets a builder for applying contributions to the test parameter currently being
        /// constructed within this scope, if there is one, or null if there is no
        /// test parameter in scope.
        /// </summary>
        ITestParameterBuilder TestParameterBuilder { get; }

        /// <summary>
        /// Gets a builder for applying contributions to the test component currently being
        /// constructed within this scope.
        /// </summary>
        /// <remarks>
        /// The component may be a test or a test parameter.  It will never be null.
        /// </remarks>
        ITestComponentBuilder TestComponentBuilder { get; }

        /// <summary>
        /// Gets a builder for applying contributions to the current test data context within the scope.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A test data context is essentially a lexically scoped environment for the declaration of
        /// data sources used by parameterized tests.
        /// </para>
        /// <para>
        /// Typically a new data context will be introduced at the point of declaration of a test
        /// or test parameter, but it may also be introduced by other lexical scopes.  For example,
        /// a test fixture constructor may declare a new test data context which provides data sources
        /// that can be used for data binding by the constructor's parameters (but are not visible
        /// to any other part of the fixture).
        /// </para>
        /// <para>
        /// Since there is always a test component in scope, the test data context builder can never be null.
        /// </para>
        /// </remarks>
        ITestDataContextBuilder TestDataContextBuilder { get; }

        /// <summary>
        /// Gets a builder for applying contributions to the test model.
        /// </summary>
        ITestModelBuilder TestModelBuilder { get; }

        /// <summary>
        /// Creates and registers a scope for a test component.
        /// </summary>
        /// <param name="codeElement">The code element to associate with the scope</param>
        /// <param name="testBuilder">The test builder</param>
        /// <param name="testParameterBuilder">The test parameter builder, or null if the scope
        /// is to be associated to the test only</param>
        /// <param name="testDataContextBuilder">The test data context builder</param>
        /// <param name="isDeclaration">If true, the scope represents the point of declaration
        /// of the test component with which it is associated</param>
        /// <returns>The new scope</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/>,
        /// <paramref name="testBuilder"/>, <paramref name="testParameterBuilder"/>
        /// or <paramref name="testDataContextBuilder"/> is null</exception>
        IPatternScope CreateScope(ICodeElementInfo codeElement,
            ITestBuilder testBuilder, ITestParameterBuilder testParameterBuilder,
            ITestDataContextBuilder testDataContextBuilder, bool isDeclaration);

        /// <summary>
        /// Creates a child test with its own child data context derived from the
        /// builders of this scope.  Returns the scope of the newly created child test.
        /// </summary>
        /// <param name="name">The test name</param>
        /// <param name="codeElement">The associated code element, or null if none</param>
        /// <returns>The scope of the newly created child test</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        /// <exception cref="PatternUsageErrorException">Thrown if <see cref="CanAddChildTest"/> is null</exception>
        IPatternScope CreateChildTestScope(string name, ICodeElementInfo codeElement);

        /// <summary>
        /// Creates a test parameter for the test in scope with its own
        /// child data context derived from the builders of this scope.  Returns
        /// the scope of the newly created test parameter.
        /// </summary>
        /// <param name="name">The test parameter name</param>
        /// <param name="codeElement">The associated code element, or null if none</param>
        /// <returns>The scope of the newly created test parameter</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        /// <exception cref="PatternUsageErrorException">Thrown if <see cref="CanAddTestParameter"/> is null</exception>
        IPatternScope CreateTestParameterScope(string name, ICodeElementInfo codeElement);

        /// <summary>
        /// Creates a child test data context derived from the builders of this scope.
        /// Returns the scope of the newly created child test data context.
        /// </summary>
        /// <param name="codeElement">The associated code element, or null if none</param>
        /// <returns>The scope of the newly created child test data context</returns>
        IPatternScope CreateChildTestDataContextScope(ICodeElementInfo codeElement);

        /// <summary>
        /// Adds an action to the scope that enables a pattern to lazily populate the test model
        /// with components generated in nested scopes.  This is used when
        /// <see cref="IPattern.Consume" /> is called with the option to skip children.
        /// </summary>
        /// <param name="populator">A <see cref="DeferredComponentPopulator" /> supplied by the pattern to populate
        /// its components lazily</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="populator"/> is null</exception>
        /// <seealso cref="PopulateDeferredComponents"/>
        void AddDeferredComponentPopulator(DeferredComponentPopulator populator);

        /// <summary>
        /// Asks all registered deferred component populators to build components whose
        /// construction had previously been deferred.
        /// </summary>
        /// <param name="codeElementHint">The code element hint to identify the location of the
        /// particular components to populate, or null to populate them all</param>
        /// <seealso cref="AddDeferredComponentPopulator"/>
        void PopulateDeferredComponents(ICodeElementInfo codeElementHint);

        /// <summary>
        /// Consumes the specified code element using this scope as the containing scope.
        /// </summary>
        /// <remarks>
        /// This is a convenience method that calls <see cref="IPatternEvaluator.Consume" />.
        /// </remarks>
        /// <param name="codeElement">The code element to consume</param>
        /// <param name="skipChildren">If true, instructs the primary pattern to defer populating child tests</param>
        /// <param name="defaultPrimaryPattern">The default primary pattern to use, if none can be resolved
        /// for the code element.  May be null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        void Consume(ICodeElementInfo codeElement, bool skipChildren, IPattern defaultPrimaryPattern);

        /// <summary>
        /// Processes the specified code element using this scope as the current scope.
        /// </summary>
        /// <remarks>
        /// This is a convenience method that calls <see cref="IPatternEvaluator.Process" />.
        /// </remarks>
        /// <param name="codeElement">The code element to process</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        void Process(ICodeElementInfo codeElement);
    }
}
