// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Reflection;
using Gallio.Framework.Explorer;
using Gallio.Framework.Patterns;

namespace Gallio.Framework.Patterns
{
    /// <summary>
    /// <para>
    /// Declares that a field, property, method parameter or generic parameter
    /// represents an <see cref="PatternTestParameter" />.
    /// Subclasses of this attribute can control what happens with the parameter.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given property,
    /// field or parameter declaration.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter
        | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = true)]
    public abstract class TestParameterPatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Gets a default instance of the parameter pattern attribute to use
        /// when no other pattern consumes the parameter.
        /// </summary>
        public static readonly TestParameterPatternAttribute DefaultInstance = new DefaultImpl();

        /// <inheritdoc />
        public override bool Consume(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
            ISlotInfo slot = (ISlotInfo)codeElement;

            PatternTestParameter testParameter = CreateTestParameter(containingTestBuilder, slot);
            IPatternTestParameterBuilder testParameterBuilder = containingTestBuilder.AddParameter(testParameter);
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
        protected virtual PatternTestParameter CreateTestParameter(IPatternTestBuilder containingTestBuilder, ISlotInfo slot)
        {
            PatternTestParameter testParameter = new PatternTestParameter(slot);
            return testParameter;
        }

        /// <summary>
        /// Initializes a test parameter after it has been added to the containing test.
        /// </summary>
        /// <param name="testParameterBuilder">The test parameter builer</param>
        /// <param name="slot">The slot</param>
        protected virtual void InitializeTestParameter(IPatternTestParameterBuilder testParameterBuilder, ISlotInfo slot)
        {
            string xmlDocumentation = slot.GetXmlDocumentation();
            if (xmlDocumentation != null)
                testParameterBuilder.TestParameter.Metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);

            foreach (IPattern pattern in testParameterBuilder.TestModelBuilder.PatternResolver.GetPatterns(slot))
                pattern.ProcessTestParameter(testParameterBuilder, slot);
        }
        
        private sealed class DefaultImpl : TestParameterPatternAttribute
        {
        }
    }
}