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
using MbUnit.Model;

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
        /// <para>
        /// Applies contributions to a type template.
        /// This method is called after the type template is linked to the template tree.
        /// </para>
        /// <para>
        /// Contributions are applied in a very specific order:
        /// <list type="bullet">
        /// <item>Type decorator attributes declared by the containing assembly</item>
        /// <item>Type decorator attributes declared by the type</item>
        /// <item>Metadata attributes declared by the type</item>
        /// <item>Fields, properties, methods and events declared by supertypes before
        /// those declared by subtypes</item>
        /// <item>Constructors</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="typeTemplate">The type template</param>
        public virtual void Apply(TemplateTreeBuilder builder, MbUnitTypeTemplate typeTemplate)
        {
            TypeDecoratorPatternAttribute.ProcessDecorators(builder, typeTemplate, typeTemplate.Type.Assembly);
            TypeDecoratorPatternAttribute.ProcessDecorators(builder, typeTemplate, typeTemplate.Type);
            MetadataPatternAttribute.ProcessMetadata(builder, typeTemplate, typeTemplate.Type);

            ProcessMembers(builder, typeTemplate);
            ProcessConstructors(builder, typeTemplate);
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
        /// Processes all public fields, properties, methods and events using reflection in order
        /// such that those declared by supertypes are processed before those declared by subtypes
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="typeTemplate">The type template</param>
        protected virtual void ProcessMembers(TemplateTreeBuilder builder, MbUnitTypeTemplate typeTemplate)
        {
            MemberInfo[] members = typeTemplate.Type.FindMembers(MemberTypes.Field | MemberTypes.Property | MemberTypes.Method | MemberTypes.Event,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, null);
            ModelUtils.SortMembersBySubTypes<MemberInfo>(members);

            foreach (MemberInfo member in members)
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        ProcessField(builder, typeTemplate, (FieldInfo)member);
                        break;

                    case MemberTypes.Property:
                        ProcessProperty(builder, typeTemplate, (PropertyInfo)member);
                        break;

                    case MemberTypes.Method:
                        ProcessMethod(builder, typeTemplate, (MethodInfo)member);
                        break;

                    case MemberTypes.Event:
                        ProcessEvent(builder, typeTemplate, (EventInfo)member);
                        break;
                }
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
        /// Processes a property using reflection to populate the
        /// type template's parameters derived from properties.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="typeTemplate">The type template</param>
        /// <param name="property">The property to process</param>
        protected virtual void ProcessProperty(TemplateTreeBuilder builder, MbUnitTypeTemplate typeTemplate, PropertyInfo property)
        {
            if (ModelUtils.CanGetAndSetNonStatic(property))
                ParameterPatternAttribute.ProcessSlot(builder, typeTemplate, new Slot(property));
        }

        /// <summary>
        /// Processes an event using reflection.  The default implementation does nothing.
        /// </summary>
        /// <todo author="jeff">
        /// What kinds of neat things can we do with events?
        /// </todo>
        /// <param name="builder">The template tree builder</param>
        /// <param name="typeTemplate">The type template</param>
        /// <param name="event">The event to process</param>
        protected virtual void ProcessEvent(TemplateTreeBuilder builder, MbUnitTypeTemplate typeTemplate, EventInfo @event)
        {
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
            TypePatternAttribute typePatternAttribute = ModelUtils.GetAttribute<TypePatternAttribute>(type);
            if (typePatternAttribute == null || ! ModelUtils.CanInstantiate(type))
                return;

            MbUnitTypeTemplate typeTemplate = typePatternAttribute.CreateTemplate(builder, assemblyTemplate, type);
            assemblyTemplate.AddTypeTemplate(typeTemplate);
            typePatternAttribute.Apply(builder, typeTemplate);
        }
    }
}