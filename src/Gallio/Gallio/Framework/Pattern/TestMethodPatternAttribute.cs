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
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public abstract class TestMethodPatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Gets a default instance of the method pattern attribute to use
        /// when no other pattern consumes a method.
        /// </summary>
        public static readonly TestMethodPatternAttribute DefaultInstance = new DefaultImpl();

        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool IsTest(IPatternResolver patternResolver, ICodeElementInfo codeElement)
        {
            return true;
        }

        /// <inheritdoc />
        public override void Consume(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement, bool skipChildren)
        {
            IMethodInfo method = (IMethodInfo)codeElement;
            Validate(method);

            PatternTest test = CreateTest(containingTestBuilder, method);
            IPatternTestBuilder testBuilder = containingTestBuilder.AddChild(test);
            InitializeTest(testBuilder, method);
            SetTestSemantics(testBuilder.Test, method);

            testBuilder.ApplyDecorators();
        }

        /// <summary>
        /// Validates whether the attribute has been applied to a valid <see cref="IMethodInfo" />.
        /// Called by <see cref="Consume" />.
        /// </summary>
        /// <remarks>
        /// The default implementation throws an exception if <paramref name="method"/> is abstract.
        /// </remarks>
        /// <param name="method">The method</param>
        /// <exception cref="ModelException">Thrown if the attribute is applied to an inappropriate method</exception>
        protected virtual void Validate(IMethodInfo method)
        {
            if (method.IsAbstract)
                throw new ModelException(String.Format("The {0} attribute is not valid for use on method '{1}'.  The method must not be abstract.", GetType().Name, method));
        }

        /// <summary>
        /// Creates a test for a method.
        /// </summary>
        /// <param name="containingTestBuilder">The containing test builder</param>
        /// <param name="method">The method</param>
        /// <returns>The test</returns>
        protected virtual PatternTest CreateTest(IPatternTestBuilder containingTestBuilder, IMethodInfo method)
        {
            PatternTest test = new PatternTest(method.Name, method);
            test.Kind = TestKinds.Test;
            test.IsTestCase = true;
            return test;
        }

        /// <summary>
        /// Initializes a test for a method after it has been added to the test model.
        /// </summary>
        /// <param name="methodTestBuilder">The test builder for the method</param>
        /// <param name="method">The method</param>
        protected virtual void InitializeTest(IPatternTestBuilder methodTestBuilder, IMethodInfo method)
        {
            string xmlDocumentation = method.GetXmlDocumentation();
            if (xmlDocumentation != null)
                methodTestBuilder.Test.Metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);

            foreach (IPattern pattern in methodTestBuilder.TestModelBuilder.PatternResolver.GetPatterns(method, true))
                pattern.ProcessTest(methodTestBuilder, method);

            if (method.IsGenericMethodDefinition)
            {
                foreach (IGenericParameterInfo parameter in method.GenericArguments)
                    ProcessGenericParameter(methodTestBuilder, parameter);
            }

            foreach (IParameterInfo parameter in method.Parameters)
                ProcessMethodParameter(methodTestBuilder, parameter);
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
        /// test instance name, <see cref="PatternTestInstanceState.TestMethod" /> and
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

                    testInstanceState.TestInstance.Name += spec.Format(testInstanceState.Formatter);
                    testInstanceState.TestMethod = spec.ResolvedMethod;
                    testInstanceState.TestArguments = spec.ResolvedArguments;
                });

            test.TestInstanceActions.ExecuteTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    if (testInstanceState.TestMethod != null && testInstanceState.TestArguments != null)
                        testInstanceState.TestMethod.Invoke(testInstanceState.FixtureInstance, testInstanceState.TestArguments);
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

        /// <summary>
        /// Gets the primary pattern of a generic parameter, or null if none.
        /// </summary>
        /// <param name="patternResolver">The pattern resolver</param>
        /// <param name="genericParameter">The generic parameter</param>
        /// <returns>The primary pattern, or null if none</returns>
        protected IPattern GetPrimaryGenericParameterPattern(IPatternResolver patternResolver, IGenericParameterInfo genericParameter)
        {
            return PatternUtils.GetPrimaryPattern(patternResolver, genericParameter) ?? DefaultGenericParameterPattern;
        }

        /// <summary>
        /// Gets the primary pattern of a method parameter, or null if none.
        /// </summary>
        /// <param name="patternResolver">The pattern resolver</param>
        /// <param name="methodParameter">The method parameter</param>
        /// <returns>The primary pattern, or null if none</returns>
        protected IPattern GetPrimaryMethodParameterPattern(IPatternResolver patternResolver, IParameterInfo methodParameter)
        {
            return PatternUtils.GetPrimaryPattern(patternResolver, methodParameter) ?? DefaultMethodParameterPattern;
        }

        /// <summary>
        /// Processes a generic parameter.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="genericParameter">The generic parameter</param>
        protected virtual void ProcessGenericParameter(IPatternTestBuilder typeTestBuilder, IGenericParameterInfo genericParameter)
        {
            IPattern pattern = GetPrimaryGenericParameterPattern(typeTestBuilder.TestModelBuilder.PatternResolver, genericParameter);
            if (pattern != null)
                pattern.Consume(typeTestBuilder, genericParameter, false);
        }

        /// <summary>
        /// Processes a method parameter.
        /// </summary>
        /// <param name="typeTestBuilder">The test builder for the type</param>
        /// <param name="methodParameter">The method parameter</param>
        protected virtual void ProcessMethodParameter(IPatternTestBuilder typeTestBuilder, IParameterInfo methodParameter)
        {
            IPattern pattern = GetPrimaryMethodParameterPattern(typeTestBuilder.TestModelBuilder.PatternResolver, methodParameter);
            if (pattern != null)
                pattern.Consume(typeTestBuilder, methodParameter, false);
        }

        private sealed class DefaultImpl : TestMethodPatternAttribute
        {
        }
    }
}