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
using Gallio.Model.Reflection;

namespace MbUnit.Model.Builder
{
    /// <summary>
    /// <para>
    /// Declares that a field, property or method parameter represents an <see cref="MbUnitTestParameter" />.
    /// Subclasses of this attribute can control what happens with the parameter.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given property,
    /// field or parameter declaration.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter
        | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = true)]
    public abstract class ParameterPatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Gets a default instance of the parameter pattern attribute to use
        /// when no other pattern consumes the parameter.
        /// </summary>
        public static readonly ParameterPatternAttribute DefaultInstance = new DefaultImpl();

        /// <inheritdoc />
        public override bool Consume(ITestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
            ISlotInfo slot = (ISlotInfo)codeElement;

            MbUnitTestParameter testParameter = CreateTestParameter(containingTestBuilder, slot);
            containingTestBuilder.Test.AddParameter(testParameter);

            ITestParameterBuilder testParameterBuilder = containingTestBuilder.TestModelBuilder.CreateTestParameterBuilder(testParameter);
            InitializeTestParameter(testParameterBuilder, slot);

            testParameterBuilder.ApplyDecorators();
            return true;
        }

        /// <summary>
        /// Creates a test parameter.
        /// </summary>
        /// <param name="containingTestBuilder">The containing test builder</param>
        /// <param name="slot">The slot</param>
        /// <returns>The test parameter</returns>
        protected virtual MbUnitTestParameter CreateTestParameter(ITestBuilder containingTestBuilder, ISlotInfo slot)
        {
            MbUnitTestParameter testParameter = new MbUnitTestParameter(slot);
            return testParameter;
        }

        /// <summary>
        /// Initializes a test parameter after it has been added to the containing test.
        /// </summary>
        /// <param name="testParameterBuilder">The test parameter builer</param>
        /// <param name="slot">The slot</param>
        protected virtual void InitializeTestParameter(ITestParameterBuilder testParameterBuilder, ISlotInfo slot)
        {
            foreach (IPattern pattern in testParameterBuilder.TestModelBuilder.ReflectionPolicy.GetPatterns(slot))
                pattern.ProcessTestParameter(testParameterBuilder, slot);
        }
        
        private sealed class DefaultImpl : ParameterPatternAttribute
        {
        }
    }
}
