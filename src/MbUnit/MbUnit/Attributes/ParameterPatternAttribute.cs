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
using System.Reflection;
using MbUnit.Model;
using Gallio.Model;
using MbUnit.Model;

namespace MbUnit.Attributes
{
    /// <summary>
    /// <para>
    /// Generates a parameter template from the annotated property, field or
    /// method parameter.  Subclasses of this attribute can control how the
    /// parameter template is manipulated by the system.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given property,
    /// field or parameter declaration.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// In some cases, as with method parameters, parameter templates will be
    /// generated automatically by the system even if this attribute has
    /// not been applied.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter,
        AllowMultiple = false, Inherited = true)]
    public abstract class ParameterPatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Gets a default instance of a parameter pattern attribute to use when
        /// the attribute is elided from a method parameter declaration.
        /// </summary>
        public static readonly ParameterPatternAttribute DefaultInstance = new DefaultImpl();

        /// <summary>
        /// Creates a test parameter.
        /// This method is called when a parameter is discovered via reflection to
        /// create a new model object to represent it.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="template">The template</param>
        /// <param name="slot">The slot</param>
        /// <returns>The test parameter</returns>
        public virtual MbUnitTemplateParameter CreateParameter(TemplateTreeBuilder builder,
            MbUnitTemplate template, Slot slot)
        {
            return new MbUnitTemplateParameter(slot);
        }

        /// <summary>
        /// <para>
        /// Applies contributions to a parameter.
        /// This method is called after the parameter is linked to the template tree.
        /// </para>
        /// <para>
        /// Contributions are applied in a very specific order:
        /// <list type="bullet">
        /// <item>Parameter decorator attributes declared by the slot</item>
        /// <item>Metadata attributes declared by the slot</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <remarks>
        /// A typical use of this method is to apply additional metadata to model
        /// objects in the template tree and to further expand the tree using
        /// declarative metadata derived via reflection.
        /// </remarks>
        /// <param name="builder">The template tree builder</param>
        /// <param name="parameter">The parameter</param>
        public virtual void Apply(TemplateTreeBuilder builder, MbUnitTemplateParameter parameter)
        {
            ICustomAttributeProvider attributeProvider = parameter.Slot.AttributeProvider;

            ParameterDecoratorPatternAttribute.ProcessDecorators(builder, parameter, attributeProvider);
            MetadataPatternAttribute.ProcessMetadata(builder, parameter, attributeProvider);

            // TODO: Look for data attributes.
        }

        /// <summary>
        /// Processes a slot using reflection and attaches a test parameter to the template if applicable.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="template">The template to which the parameter should be attached</param>
        /// <param name="slot">The slot to process</param>
        public static void ProcessSlot(TemplateTreeBuilder builder, MbUnitTemplate template, Slot slot)
        {
            ParameterPatternAttribute parameterPatternAttribute = ModelUtils.GetAttribute<ParameterPatternAttribute>(slot.AttributeProvider);

            if (parameterPatternAttribute == null)
            {
                if (slot.Parameter == null)
                {
                    // Fields and properties are not automatically treated as template parameters
                    // because they might be used by the test for unrelated purposes.  To disambiguate
                    // we check if we have at least one other pattern attribute on this member
                    // then assume we just happened to elide the parameter attribute in this case.
                    // This provides a particularly nice shortcut for self-binding parameters
                    // with data attributes.
                    if (slot.AttributeProvider.GetCustomAttributes(typeof(PatternAttribute), true).Length == 0)
                        return;
                }

                parameterPatternAttribute = DefaultInstance;
            }

            MbUnitTemplateParameter parameter = parameterPatternAttribute.CreateParameter(builder, template, slot);
            template.AddParameter(parameter);
            parameterPatternAttribute.Apply(builder, parameter);
        }

        private class DefaultImpl : ParameterPatternAttribute
        {
        }
    }
}
