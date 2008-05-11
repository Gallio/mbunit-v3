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
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// Declares that a method represents a <see cref="PatternTest" />.
    /// Subclasses of this attribute can control what happens with the method.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given method.
    /// </para>
    /// </summary>
    /// <seealso cref="TestMethodDecoratorPatternAttribute"/>
    [AttributeUsage(PatternAttributeTargets.TestMethod, AllowMultiple=false, Inherited=true)]
    public abstract class TestMethodPatternAttribute : PatternAttribute
    {
        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool IsTest(PatternEvaluator evaluator, ICodeElementInfo codeElement)
        {
            return true;
        }

        /// <inheritdoc />
        public override void Consume(PatternEvaluationScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            IMethodInfo method = codeElement as IMethodInfo;
            Validate(containingScope, method);

            PatternTest methodTest = CreateTest(containingScope, method);
            PatternEvaluationScope methodScope = containingScope.AddChildTest(methodTest);
            InitializeTest(methodScope, method);
            SetTestSemantics(methodTest, method);

            methodScope.ApplyDecorators();
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="containingScope">The containing scope</param>
        /// <param name="method">The method</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly</exception>
        protected virtual void Validate(PatternEvaluationScope containingScope, IMethodInfo method)
        {
            if (!containingScope.CanAddChildTest || method == null)
                ThrowUsageErrorException("This attribute can only be used on a test method within a test type.");
            if (method.IsAbstract)
                ThrowUsageErrorException("This attribute cannot be used on an abstract method.");
        }

        /// <summary>
        /// Creates a test for a method.
        /// </summary>
        /// <param name="containingScope">The containing scope</param>
        /// <param name="method">The method</param>
        /// <returns>The test</returns>
        protected virtual PatternTest CreateTest(PatternEvaluationScope containingScope, IMethodInfo method)
        {
            PatternTest test = new PatternTest(method.Name, method, containingScope.TestDataContext.CreateChild());
            test.Kind = TestKinds.Test;
            test.IsTestCase = true;
            return test;
        }

        /// <summary>
        /// Initializes a test for a method after it has been added to the test model.
        /// </summary>
        /// <param name="methodScope">The method scope</param>
        /// <param name="method">The method</param>
        protected virtual void InitializeTest(PatternEvaluationScope methodScope, IMethodInfo method)
        {
            string xmlDocumentation = method.GetXmlDocumentation();
            if (xmlDocumentation != null)
                methodScope.Test.Metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);

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
        /// Applies semantic actions to the <see cref="PatternTest.TestActions" /> member of a 
        /// test to set the test's runtime behavior.
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
        /// <param name="test">The test</param>
        /// <param name="method">The test method</param>
        protected virtual void SetTestSemantics(PatternTest test, IMethodInfo method)
        {
            test.TestInstanceActions.BeforeTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    MethodInvocationSpec spec = testInstanceState.GetTestMethodInvocationSpec(method);

                    testInstanceState.TestMethod = spec.ResolvedMethod;
                    testInstanceState.TestArguments = spec.ResolvedArguments;

                    if (!testInstanceState.IsReusingPrimaryTestStep)
                        testInstanceState.TestStep.Name = spec.Format(testInstanceState.TestStep.Name, testInstanceState.Formatter);
                });

            test.TestInstanceActions.ExecuteTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    testInstanceState.InvokeTestMethod();
                });
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