// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Model;
using Gallio.Model.Reflection;
using MbUnit.Model.Builder;
using MbUnit.Model.Patterns;

namespace MbUnit.Model.Patterns
{
    /// <summary>
    /// <para>
    /// Declares that a method represents an <see cref="MbUnitTest" />.
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
        public override bool Consume(ITestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
            IMethodInfo method = (IMethodInfo)codeElement;

            MbUnitTest test = CreateMethodTest(containingTestBuilder, method);
            ITestBuilder testBuilder = containingTestBuilder.AddChild(test);
            InitializeMethodTest(testBuilder, method);

            testBuilder.ApplyDecorators();
            return true;
        }

        /// <summary>
        /// Creates a test for a method.
        /// </summary>
        /// <param name="containingTestBuilder">The containing test builder</param>
        /// <param name="method">The method</param>
        /// <returns>The test</returns>
        protected virtual MbUnitTest CreateMethodTest(ITestBuilder containingTestBuilder, IMethodInfo method)
        {
            MbUnitTest test = new MbUnitTest(method.Name, method);
            test.Kind = TestKinds.Test;
            test.IsTestCase = true;
            return test;
        }

        /// <summary>
        /// Initializes a test for a method after it has been added to the test model.
        /// </summary>
        /// <param name="methodTestBuilder">The test builder for the method</param>
        /// <param name="method">The method</param>
        protected virtual void InitializeMethodTest(ITestBuilder methodTestBuilder, IMethodInfo method)
        {
            string xmlDocumentation = method.GetXmlDocumentation();
            if (xmlDocumentation != null)
                methodTestBuilder.Test.Metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);

            foreach (IPattern pattern in methodTestBuilder.TestModelBuilder.PatternResolver.GetPatterns(method))
                pattern.ProcessTest(methodTestBuilder, method);

            foreach (IGenericParameterInfo parameter in method.GetGenericParameters())
                ProcessSlot(methodTestBuilder, parameter);

            foreach (IParameterInfo parameter in method.GetParameters())
                ProcessSlot(methodTestBuilder, parameter);
        }

        /// <summary>
        /// Processes a method parameter or generic parameter slot.
        /// </summary>
        /// <param name="methodTestBuilder">The test builder for the method</param>
        /// <param name="slot">The slot</param>
        /// <returns>True if the slot was consumed</returns>
        protected virtual bool ProcessSlot(ITestBuilder methodTestBuilder, ISlotInfo slot)
        {
            return PatternUtils.ConsumeWithFallback(methodTestBuilder, slot, ProcessSlotFallback);
        }

        /// <summary>
        /// Processes a slot using a default rule because no associated pattern has consumed it.
        /// </summary>
        /// <param name="methodTestBuilder">The test builder for the method</param>
        /// <param name="slot">The slot</param>
        /// <returns>True if the slot was consumed</returns>
        protected virtual bool ProcessSlotFallback(ITestBuilder methodTestBuilder, ISlotInfo slot)
        {
            return TestParameterPatternAttribute.DefaultInstance.Consume(methodTestBuilder, slot);
        }

        private sealed class DefaultImpl : TestMethodPatternAttribute
        {
        }
    }
}