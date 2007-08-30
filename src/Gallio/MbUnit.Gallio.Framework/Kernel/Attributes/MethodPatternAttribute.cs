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
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Framework.Kernel.Attributes
{
    /// <summary>
    /// <para>
    /// Generates a method template from the annotated method.
    /// Subclasses of this attribute can control what happens with the method.
    /// The method might not necessarily represent a test.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given method.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public abstract class MethodPatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Creates a method template.
        /// This method is called when a method is discovered via reflection to
        /// create a new model object to represent it.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="typeTemplate">The containing type template</param>
        /// <param name="methodInfo">The test method</param>
        /// <returns>The test method template</returns>
        public virtual MbUnitMethodTemplate CreateTemplate(TemplateTreeBuilder builder,
            MbUnitTypeTemplate typeTemplate, MethodInfo methodInfo)
        {
            return new MbUnitMethodTemplate(typeTemplate, methodInfo);
        }

        /// <summary>
        /// Applies contributions to a method template.
        /// This method is called after the method template is linked to the template tree.
        /// </summary>
        /// <remarks>
        /// A typical use of this method is to apply additional metadata to model
        /// objects in the template tree and to further expand the tree using
        /// declarative metadata derived via reflection.
        /// </remarks>
        /// <param name="builder">The template tree builder</param>
        /// <param name="methodTemplate">The method template</param>
        public virtual void Apply(TemplateTreeBuilder builder, MbUnitMethodTemplate methodTemplate)
        {
            // FIXME: Should we apply decorators in two separate sorted batches or merge them
            //        together to produce a single ordered collection?  I think it's easier
            //        to reason about decorators locally if they are only ordered based on the
            //        site to which they are applied.
            MethodDecoratorPatternAttribute.ProcessDecorators(builder, methodTemplate, methodTemplate.Method);
            MethodDecoratorPatternAttribute.ProcessDecorators(builder, methodTemplate, methodTemplate.Method.ReflectedType);
            MetadataPatternAttribute.ProcessMetadata(builder, methodTemplate, methodTemplate.Method);

            ProcessParameters(builder, methodTemplate);
        }

        /// <summary>
        /// Processes a method using reflection to populate the template tree.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="typeTemplate">The type template</param>
        /// <param name="method">The method to process</param>
        public static void ProcessMethod(TemplateTreeBuilder builder, MbUnitTypeTemplate typeTemplate, MethodInfo method)
        {
            MethodPatternAttribute methodPatternAttribute = ReflectionUtils.GetAttribute<MethodPatternAttribute>(method);
            if (methodPatternAttribute == null)
                return;

            MbUnitMethodTemplate methodTemplate = methodPatternAttribute.CreateTemplate(builder, typeTemplate, method);
            typeTemplate.AddMethodTemplate(methodTemplate);
            methodPatternAttribute.Apply(builder, methodTemplate);
        }

        /// <summary>
        /// Processes all parameters using reflection to populate method parameters.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="methodTemplate">The method template</param>
        protected virtual void ProcessParameters(TemplateTreeBuilder builder, MbUnitMethodTemplate methodTemplate)
        {
            foreach (ParameterInfo parameter in methodTemplate.Method.GetParameters())
            {
                ProcessParameter(builder, methodTemplate, parameter);
            }
        }

        /// <summary>
        /// Processes a parameter using reflection to populate method parameters.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="methodTemplate">The method template</param>
        /// <param name="parameter">The parameter</param>
        protected virtual void ProcessParameter(TemplateTreeBuilder builder, MbUnitMethodTemplate methodTemplate,
            ParameterInfo parameter)
        {
            ParameterPatternAttribute.ProcessSlot(builder, methodTemplate, new Slot(parameter));
        }
    }
}
