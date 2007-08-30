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
    /// Generates a type template from the annotated class.
    /// Subclasses of this attribute can control what happens with the type.
    /// The type might not necessarily represent a test fixture.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given class.
    /// </para>
    /// </summary>
    /// <seealso cref="TypeDecoratorPatternAttribute"/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class TypePatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Creates a type template.
        /// This method is called when a type is discovered via reflection to
        /// create a new model object to represent it.
        /// </summary>
        /// <remarks>
        /// A typical use of this method is to apply additional metadata to model
        /// objects in the template tree and to further expand the tree using
        /// declarative metadata derived via reflection.
        /// </remarks>
        /// <param name="builder">The template tree builder</param>
        /// <param name="assemblyTemplate">The containing assembly template</param>
        /// <param name="type">The annotated type</param>
        /// <returns>The type template</returns>
        public virtual MbUnitTypeTemplate CreateTemplate(TemplateTreeBuilder builder,
            MbUnitAssemblyTemplate assemblyTemplate, Type type)
        {
            return new MbUnitTypeTemplate(assemblyTemplate, type);
        }

        /// <summary>
        /// Applies contributions to a type template.
        /// This method is called after the type template is linked to the template tree.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="typeTemplate">The type template</param>
        public virtual void Apply(TemplateTreeBuilder builder, MbUnitTypeTemplate typeTemplate)
        {
            TypeDecoratorPatternAttribute.ProcessDecorators(builder, typeTemplate, typeTemplate.Type);
            TypeDecoratorPatternAttribute.ProcessDecorators(builder, typeTemplate, typeTemplate.Type.Assembly);
            MetadataPatternAttribute.ProcessMetadata(builder, typeTemplate, typeTemplate.Type);

            ProcessConstructors(builder, typeTemplate);
            ProcessFields(builder, typeTemplate);
            ProcessProperties(builder, typeTemplate);
            ProcessMethods(builder, typeTemplate);
        }

        /// <summary>
        /// Processes all public constructors using reflection to populate the
        /// type template's parameters derived from constructor parameters.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="typeTemplate">The type template</param>
        protected virtual void ProcessConstructors(TemplateTreeBuilder builder, MbUnitTypeTemplate typeTemplate)
        {
            foreach (ConstructorInfo constructor in typeTemplate.Type.GetConstructors())
            {
                ProcessConstructor(builder, typeTemplate, constructor);

                // FIXME: Currently we arbitrarily choose the first constructor and throw away the rest.
                //        This should be replaced by a more intelligent mechanism that can
                //        handle optional or alternative dependencies.  We might benefit from
                //        using an existing inversion of control framework like Castle
                //        to handle stuff like this.
                break;
            }
        }

        /// <summary>
        /// Processes a constructor using reflection to populate the
        /// type template's parameters derived from constructor parameters.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="typeTemplate">The type template</param>
        /// <param name="constructor">The constructor to process</param>
        protected virtual void ProcessConstructor(TemplateTreeBuilder builder, MbUnitTypeTemplate typeTemplate, ConstructorInfo constructor)
        {
            foreach (ParameterInfo parameter in constructor.GetParameters())
            {
                ParameterPatternAttribute.ProcessSlot(builder, typeTemplate, new Slot(parameter));
            }
        }

        /// <summary>
        /// Processes all public fields using reflection to populate the
        /// type template's parameters derived from fields.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="typeTemplate">The type template</param>
        protected virtual void ProcessFields(TemplateTreeBuilder builder, MbUnitTypeTemplate typeTemplate)
        {
            foreach (FieldInfo field in typeTemplate.Type.GetFields())
            {
                ProcessField(builder, typeTemplate, field);
            }
        }

        /// <summary>
        /// Processes a field using reflection to populate the
        /// type template's parameters derived from fields.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="typeTemplate">The type template</param>
        /// <param name="field">The field to process</param>
        protected virtual void ProcessField(TemplateTreeBuilder builder, MbUnitTypeTemplate typeTemplate, FieldInfo field)
        {
            ParameterPatternAttribute.ProcessSlot(builder, typeTemplate, new Slot(field));
        }

        /// <summary>
        /// Processes all public properties using reflection to populate the
        /// type template's parameters derived from properties.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="typeTemplate">The type template</param>
        protected virtual void ProcessProperties(TemplateTreeBuilder builder, MbUnitTypeTemplate typeTemplate)
        {
            foreach (PropertyInfo property in typeTemplate.Type.GetProperties())
            {
                ProcessProperty(builder, typeTemplate, property);
            }
        }

        /// <summary>
        /// Processes a property using reflection to populate the
        /// type template's parameters derived from properties.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="typeTemplate">The type template</param>
        /// <param name="property">The property to process</param>
        protected virtual void ProcessProperty(TemplateTreeBuilder builder, MbUnitTypeTemplate typeTemplate, PropertyInfo property)
        {
            if (ReflectionUtils.CanGetAndSetNonStatic(property))
                ParameterPatternAttribute.ProcessSlot(builder, typeTemplate, new Slot(property));
        }

        /// <summary>
        /// Processes all public methods using reflection to populate tests and other
        /// executable components.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="typeTemplate">The type template</param>
        protected virtual void ProcessMethods(TemplateTreeBuilder builder, MbUnitTypeTemplate typeTemplate)
        {
            foreach (MethodInfo method in typeTemplate.Type.GetMethods())
            {
                ProcessMethod(builder, typeTemplate, method);
            }
        }

        /// <summary>
        /// Processes a method using reflection to populate tests and other executable components.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="typeTemplate">The type template</param>
        /// <param name="method">The method to process</param>
        protected virtual void ProcessMethod(TemplateTreeBuilder builder, MbUnitTypeTemplate typeTemplate, MethodInfo method)
        {
            MethodPatternAttribute.ProcessMethod(builder, typeTemplate, method);
        }

        /// <summary>
        /// Scans a type using reflection to build a template tree.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="assemblyTemplate">The containing assembly template</param>
        /// <param name="type">The type to process</param>
        public static void ProcessType(TemplateTreeBuilder builder, MbUnitAssemblyTemplate assemblyTemplate, Type type)
        {
            TypePatternAttribute typePatternAttribute = ReflectionUtils.GetAttribute<TypePatternAttribute>(type);
            if (typePatternAttribute == null || ! ReflectionUtils.CanInstantiate(type))
                return;

            MbUnitTypeTemplate typeTemplate = typePatternAttribute.CreateTemplate(builder, assemblyTemplate, type);
            assemblyTemplate.AddTypeTemplate(typeTemplate);
            typePatternAttribute.Apply(builder, typeTemplate);
        }
    }
}