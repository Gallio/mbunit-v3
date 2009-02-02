// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Framework.Data;
using Gallio.Model;
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Declares that a method represents a test.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Subclasses of this attribute can control what happens with the method.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given method.
    /// </para>
    /// <para>
    /// A test method has a timeout of 10 minutes by default.
    /// </para>
    /// </remarks>
    /// <seealso cref="TestMethodDecoratorPatternAttribute"/>
    [AttributeUsage(PatternAttributeTargets.TestMethod, AllowMultiple=false, Inherited=true)]
    public abstract class TestMethodPatternAttribute : PatternAttribute
    {
        /// <summary>
        /// <para>
        /// Gets or sets a number that defines an ordering for the test with respect to its siblings.
        /// </para>
        /// <para>
        /// Unless compelled otherwise by test dependencies, tests with a lower order number than
        /// their siblings will run before those siblings and tests with the same order number
        /// as their siblings with run in an arbitrary sequence with respect to those siblings.
        /// </para>
        /// </summary>
        /// <value>The test execution order with respect to siblings, initially zero.</value>
        public int Order { get; set; }

        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool IsTest(IPatternEvaluator evaluator, ICodeElementInfo codeElement)
        {
            return true;
        }

        /// <inheritdoc />
        public override void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            IMethodInfo method = codeElement as IMethodInfo;
            Validate(containingScope, method);

            IPatternScope methodScope = containingScope.CreateChildTestScope(method.Name, method);
            methodScope.TestBuilder.Kind = TestKinds.Test;
            methodScope.TestBuilder.IsTestCase = true;
            methodScope.TestBuilder.Order = Order;
            methodScope.TestBuilder.Timeout = TimeSpan.FromMinutes(10);

            InitializeTest(methodScope, method);
            SetTestSemantics(methodScope.TestBuilder, method);

            methodScope.TestBuilder.ApplyDeferredActions();
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="containingScope">The containing scope</param>
        /// <param name="method">The method</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly</exception>
        protected virtual void Validate(IPatternScope containingScope, IMethodInfo method)
        {
            if (!containingScope.CanAddChildTest || method == null)
                ThrowUsageErrorException("This attribute can only be used on a test method within a test type.");
            if (method.IsAbstract)
                ThrowUsageErrorException("This attribute cannot be used on an abstract method.");
        }

        /// <summary>
        /// Initializes a test for a method after it has been added to the test model.
        /// </summary>
        /// <param name="methodScope">The method scope</param>
        /// <param name="method">The method</param>
        protected virtual void InitializeTest(IPatternScope methodScope, IMethodInfo method)
        {
            string xmlDocumentation = method.GetXmlDocumentation();
            if (xmlDocumentation != null)
                methodScope.TestBuilder.AddMetadata(MetadataKeys.XmlDocumentation, xmlDocumentation);

            methodScope.Process(method);

            if (method.IsGenericMethodDefinition)
            {
                foreach (IGenericParameterInfo parameter in method.GenericArguments)
                    methodScope.Consume(parameter, false, DefaultGenericParameterPattern);
            }

            foreach (IParameterInfo parameter in method.Parameters)
                methodScope.Consume(parameter, false, DefaultMethodParameterPattern);
        }

        /// <summary>
        /// <para>
        /// Applies semantic actions to a test to estalish its runtime behavior.
        /// </para>
        /// <para>
        /// This method is called after <see cref="InitializeTest" />.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default behavior for a <see cref="TestMethodPatternAttribute" />
        /// is to configure the test actions as follows:
        /// <list type="bullet">
        /// <item><see cref="IPatternTestInstanceHandler.BeforeTestInstance" />: Set the
        /// test step name, <see cref="PatternTestInstanceState.TestMethod" /> and
        /// <see cref="PatternTestInstanceState.TestArguments" /> based on any values bound
        /// to the test method's generic parameter and method parameter slots.</item>
        /// <item><see cref="IPatternTestInstanceHandler.ExecuteTestInstance" />: Invoke the method.</item>
        /// </list>
        /// </para>
        /// <para>
        /// You can override this method to change the semantics as required.
        /// </para>
        /// </remarks>
        /// <param name="testBuilder">The test builder</param>
        /// <param name="method">The test method</param>
        protected virtual void SetTestSemantics(ITestBuilder testBuilder, IMethodInfo method)
        {
            testBuilder.TestInstanceActions.BeforeTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    MethodInvocationSpec spec = testInstanceState.GetTestMethodInvocationSpec(method);

                    testInstanceState.TestMethod = spec.ResolvedMethod;
                    testInstanceState.TestArguments = spec.ResolvedArguments;

                    if (!testInstanceState.IsReusingPrimaryTestStep)
                        testInstanceState.TestStep.Name = spec.Format(testInstanceState.TestStep.Name, testInstanceState.Formatter);
                });

            testBuilder.TestInstanceActions.ExecuteTestInstanceChain.After(state => Execute(state));
        }

        /// <summary>
        /// Executes the test method.
        /// </summary>
        /// <param name="state">The test instance state, not null</param>
        /// <returns>The test method result</returns>
        protected virtual object Execute(PatternTestInstanceState state)
        {
            return state.InvokeTestMethod();
        }

        /// <summary>
        /// Gets the default pattern to apply to generic parameters that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <see cref="TestParameterPatternAttribute.DefaultInstance" />.
        /// </remarks>
        protected virtual IPattern DefaultGenericParameterPattern
        {
            get { return TestParameterPatternAttribute.DefaultInstance; }
        }

        /// <summary>
        /// Gets the default pattern to apply to method parameters that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <see cref="TestParameterPatternAttribute.DefaultInstance" />.
        /// </remarks>
        protected virtual IPattern DefaultMethodParameterPattern
        {
            get { return TestParameterPatternAttribute.DefaultInstance; }
        }
    }
}