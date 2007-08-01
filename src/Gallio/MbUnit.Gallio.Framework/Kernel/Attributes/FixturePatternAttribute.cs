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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Framework.Kernel.Attributes
{
    /// <summary>
    /// <para>
    /// Declares that a class represents an MbUnit test fixture.  Subclasses of this
    /// attribute can customize how template enumeration takes place within
    /// a fixture.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given class.
    /// </para>
    /// </summary>
    /// <seealso cref="FixtureDecoratorPatternAttribute"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public abstract class FixturePatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Gets a default instance of the fixture pattern attribute to use
        /// when none was specified.
        /// </summary>
        public static readonly FixturePatternAttribute DefaultInstance = new DefaultImpl();

        /// <summary>
        /// Creates a test fixture template.
        /// This method is called when a fixture is discovered via reflection to
        /// create a new model object to represent it.
        /// </summary>
        /// <remarks>
        /// A typical use of this method is to apply additional metadata to model
        /// objects in the template tree and to further expand the tree using
        /// declarative metadata derived via reflection.
        /// </remarks>
        /// <param name="builder">The template tree builder</param>
        /// <param name="assemblyTemplate">The containing assembly template</param>
        /// <param name="fixtureType">The fixture type</param>
        /// <returns>The test fixture template</returns>
        public virtual MbUnitFixtureTemplate CreateTemplate(TemplateTreeBuilder builder,
            MbUnitAssemblyTemplate assemblyTemplate, Type fixtureType)
        {
            return new MbUnitFixtureTemplate(assemblyTemplate, fixtureType);
        }

        /// <summary>
        /// Applies contributions to a fixture template.
        /// This method is called after the fixture template is linked to the template tree.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="fixtureTemplate">The fixture template</param>
        public virtual void Apply(TemplateTreeBuilder builder, MbUnitFixtureTemplate fixtureTemplate)
        {
            FixtureDecoratorPatternAttribute.ProcessDecorators(builder, fixtureTemplate, fixtureTemplate.FixtureType);
            MetadataPatternAttribute.ProcessMetadata(builder, fixtureTemplate, fixtureTemplate.FixtureType);

            ProcessConstructors(builder, fixtureTemplate);
            ProcessFields(builder, fixtureTemplate);
            ProcessProperties(builder, fixtureTemplate);
            ProcessMethods(builder, fixtureTemplate);
        }

        /// <summary>
        /// Processes all public constructors using reflection to populate fixture parameters.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="fixtureTemplate">The fixture template</param>
        protected virtual void ProcessConstructors(TemplateTreeBuilder builder, MbUnitFixtureTemplate fixtureTemplate)
        {
            foreach (ConstructorInfo constructor in fixtureTemplate.FixtureType.GetConstructors())
            {
                ProcessConstructor(builder, fixtureTemplate, constructor);

                // FIXME: Currently we arbitrarily choose the first constructor and throw away the rest.
                //        This should be replaced by a more intelligent mechanism that can
                //        handle optional or alternative dependencies.  We might benefit from
                //        using an existing inversion of control framework like Castle
                //        to handle stuff like this.
                break;
            }
        }

        /// <summary>
        /// Processes a constructor using reflection to populate fixture parameters.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="fixtureTemplate">The fixture template</param>
        /// <param name="constructor">The constructor to process</param>
        protected virtual void ProcessConstructor(TemplateTreeBuilder builder, MbUnitFixtureTemplate fixtureTemplate, ConstructorInfo constructor)
        {
            MbUnitTemplateParameterSet parameterSet = fixtureTemplate.CreateAnonymousParameterSet();
            foreach (ParameterInfo parameter in constructor.GetParameters())
            {
                ParameterPatternAttribute.ProcessSlot(builder, parameterSet, new Slot(parameter));
            }
        }

        /// <summary>
        /// Processes all public fields using reflection to populate fixture parameters.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="fixtureTemplate">The fixture template</param>
        protected virtual void ProcessFields(TemplateTreeBuilder builder, MbUnitFixtureTemplate fixtureTemplate)
        {
            foreach (FieldInfo field in fixtureTemplate.FixtureType.GetFields())
            {
                ProcessField(builder, fixtureTemplate, field);
            }
        }

        /// <summary>
        /// Processes a field using reflection to populate fixture parameters.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="fixtureTemplate">The fixture template</param>
        /// <param name="field">The field to process</param>
        protected virtual void ProcessField(TemplateTreeBuilder builder, MbUnitFixtureTemplate fixtureTemplate, FieldInfo field)
        {
            ParameterPatternAttribute.ProcessSlot(builder, fixtureTemplate.CreateAnonymousParameterSet(), new Slot(field));
        }

        /// <summary>
        /// Processes all public properties using reflection to populate fixture parameters.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="fixtureTemplate">The fixture template</param>
        protected virtual void ProcessProperties(TemplateTreeBuilder builder, MbUnitFixtureTemplate fixtureTemplate)
        {
            foreach (PropertyInfo property in fixtureTemplate.FixtureType.GetProperties())
            {
                ProcessProperty(builder, fixtureTemplate, property);
            }
        }

        /// <summary>
        /// Processes a property using reflection to populate fixture parameters.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="fixtureTemplate">The fixture template</param>
        /// <param name="property">The property to process</param>
        protected virtual void ProcessProperty(TemplateTreeBuilder builder, MbUnitFixtureTemplate fixtureTemplate, PropertyInfo property)
        {
            if (ReflectionUtils.CanGetAndSetNonStatic(property))
                ParameterPatternAttribute.ProcessSlot(builder, fixtureTemplate.CreateAnonymousParameterSet(), new Slot(property));
        }

        /// <summary>
        /// Processes all public methods using reflection to populate tests and other
        /// executable components.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="fixtureTemplate">The fixture template</param>
        protected virtual void ProcessMethods(TemplateTreeBuilder builder, MbUnitFixtureTemplate fixtureTemplate)
        {
            foreach (MethodInfo method in fixtureTemplate.FixtureType.GetMethods())
            {
                ProcessMethod(builder, fixtureTemplate, method);
            }
        }

        /// <summary>
        /// Processes a method using reflection to populate tests and other executable components.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="fixtureTemplate">The fixture template</param>
        /// <param name="method">The method to process</param>
        protected virtual void ProcessMethod(TemplateTreeBuilder builder, MbUnitFixtureTemplate fixtureTemplate, MethodInfo method)
        {
            if (ReflectionUtils.CanInvokeNonStatic(method))
                MethodPatternAttribute.ProcessMethod(builder, fixtureTemplate, method);
        }

        /// <summary>
        /// Scans a type using reflection to build a template tree.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="assemblyTemplate">The containing assembly template</param>
        /// <param name="type">The type to process</param>
        public static void ProcessType(TemplateTreeBuilder builder, MbUnitAssemblyTemplate assemblyTemplate, Type type)
        {
            FixturePatternAttribute fixturePatternAttribute = ReflectionUtils.GetAttribute<FixturePatternAttribute>(type);
            if (fixturePatternAttribute == null || ! ReflectionUtils.CanInstantiate(type))
                return;

            MbUnitFixtureTemplate fixtureTemplate = fixturePatternAttribute.CreateTemplate(builder, assemblyTemplate, type);
            assemblyTemplate.AddFixtureTemplate(fixtureTemplate);
            fixturePatternAttribute.Apply(builder, fixtureTemplate);
        }

        private class DefaultImpl : FixturePatternAttribute
        {
        }
    }
}
