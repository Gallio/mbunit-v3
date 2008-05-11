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
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// Declares that a field, property, method parameter or generic parameter
    /// represents a <see cref="PatternTestParameter" />.
    /// Subclasses of this attribute can control what happens with the parameter.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given property,
    /// field or parameter declaration.
    /// </para>
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.TestParameter, AllowMultiple = false, Inherited = true)]
    public abstract class TestParameterPatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Gets a default instance of the parameter pattern attribute to use
        /// when no other pattern consumes the parameter.
        /// </summary>
        public static readonly TestParameterPatternAttribute DefaultInstance = new DefaultImpl();

        /// <summary>
        /// Gets an instance of the parameter pattern attribute to use when no
        /// other pattern consumes the parameter but when the parameter appears to have
        /// other contributing pattern attributes associated with it.  So a test parameter
        /// is created automatically if we try to apply contributions to it, such as data items,
        /// but otherwise it is silent.  This is particularly useful with fields and properties.
        /// </summary>
        public static readonly TestParameterPatternAttribute AutomaticInstance = new AutomaticImpl();

        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override void Consume(PatternEvaluationScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            ISlotInfo slot = codeElement as ISlotInfo;
            Validate(containingScope, slot);

            PatternTestParameter testParameter = CreateTestParameter(containingScope, slot);
            PatternEvaluationScope testParameterScope = containingScope.AddTestParameter(testParameter);
            InitializeTestParameter(testParameterScope, slot);

            testParameterScope.ApplyDecorators();
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="containingScope">The containing scope</param>
        /// <param name="slot">The slot</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly</exception>
        protected virtual void Validate(PatternEvaluationScope containingScope, ISlotInfo slot)
        {
            if (!containingScope.CanAddTestParameter || slot == null)
                ThrowUsageErrorException("This attribute can only be used on a test parameter.");
        }

        /// <summary>
        /// Creates a test parameter.
        /// </summary>
        /// <param name="containingScope">The containing scope</param>
        /// <param name="slot">The slot</param>
        /// <returns>The test parameter</returns>
        protected virtual PatternTestParameter CreateTestParameter(PatternEvaluationScope containingScope, ISlotInfo slot)
        {
            PatternTestParameter testParameter = new PatternTestParameter(slot.Name, slot, containingScope.TestDataContext.CreateChild());
            return testParameter;
        }

        /// <summary>
        /// Initializes a test parameter after it has been added to the containing test.
        /// </summary>
        /// <param name="testParameterScope">The test parameter scope</param>
        /// <param name="slot">The slot</param>
        protected virtual void InitializeTestParameter(PatternEvaluationScope testParameterScope, ISlotInfo slot)
        {
            int index = slot.Position;
            IParameterInfo parameter = slot as IParameterInfo;
            if (parameter != null)
            {
                // For generic methods, we offset the position of the parameter
                // by the number of generic method parameters.  That way
                // we can assign each parameter in the method a unique index
                // by reading all parameters left to right, starting with the
                // generic parameters and moving onwards to the method parameters.
                IMethodInfo method = parameter.Member as IMethodInfo;
                if (method != null && method.IsGenericMethodDefinition)
                    index += method.GenericArguments.Count;
            }
            testParameterScope.TestDataContext.ImplicitDataBindingIndexOffset = index;

            string xmlDocumentation = slot.GetXmlDocumentation();
            if (xmlDocumentation != null)
                testParameterScope.TestParameter.Metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);

            testParameterScope.Process(slot);
        }
        
        private sealed class DefaultImpl : TestParameterPatternAttribute
        {
        }

        private sealed class AutomaticImpl : TestParameterPatternAttribute
        {
            public override void Consume(PatternEvaluationScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
            {
                if (containingScope.Evaluator.HasPatterns(codeElement))
                    base.Consume(containingScope, codeElement, skipChildren);
            }
        }
    }
}