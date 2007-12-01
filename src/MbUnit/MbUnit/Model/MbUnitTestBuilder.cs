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
using System.Text;
using Gallio.Model;
using Gallio.Model.Reflection;
using MbUnit.Attributes;

namespace MbUnit.Model
{
    /// <summary>
    /// The MbUnit test builder encapsulates the built-in MbUnit reflection model
    /// used to construct tests from declarative metadata.
    /// </summary>
    public class MbUnitTestBuilder
    {
        private readonly TemplateTreeBuilder templateTreeBuilder;

        /// <summary>
        /// Creates a test builder.
        /// </summary>
        /// <param name="templateTreeBuilder">The template tree builder</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="templateTreeBuilder"/> is null</exception>
        public MbUnitTestBuilder(TemplateTreeBuilder templateTreeBuilder)
        {
            if (templateTreeBuilder == null)
                throw new ArgumentNullException("templateTreeBuilder");

            this.templateTreeBuilder = templateTreeBuilder;
        }

        /// <summary>
        /// Gets the template tree builder.
        /// </summary>
        public TemplateTreeBuilder TemplateTreeBuilder
        {
            get { return templateTreeBuilder; }
        }

        /// <summary>
        /// Processes an assembly using reflection to build a template tree.
        /// </summary>
        /// <param name="frameworkTemplate">The containing framework template</param>
        /// <param name="assembly">The assembly to process</param>
        public virtual void ProcessAssembly(MbUnitFrameworkTemplate frameworkTemplate, IAssemblyInfo assembly)
        {
            AssemblyPatternAttribute assemblyPatternAttribute = AttributeUtils.GetAttribute<AssemblyPatternAttribute>(assembly, false);
            if (assemblyPatternAttribute == null)
                assemblyPatternAttribute = AssemblyPatternAttribute.DefaultInstance;

            MbUnitAssemblyTemplate assemblyTemplate = assemblyPatternAttribute.CreateTemplate(this, frameworkTemplate, assembly);
            frameworkTemplate.AddAssemblyTemplate(assemblyTemplate);

            assemblyPatternAttribute.Apply(this, assemblyTemplate);
        }

        /// <summary>
        /// Processes all assembly decorators via reflection.
        /// </summary>
        /// <param name="assemblyTemplate">The assembly template</param>
        public virtual void ProcessAssemblyDecorators(MbUnitAssemblyTemplate assemblyTemplate)
        {
            List<AssemblyDecoratorPatternAttribute> decorators = new List<AssemblyDecoratorPatternAttribute>();
            decorators.AddRange(AttributeUtils.GetAttributes<AssemblyDecoratorPatternAttribute>(assemblyTemplate.Assembly, false));
            decorators.Sort(DecoratorOrderComparer<AssemblyDecoratorPatternAttribute>.Instance);

            foreach (AssemblyDecoratorPatternAttribute decoratorAttribute in decorators)
            {
                decoratorAttribute.Apply(this, assemblyTemplate);
            }
        }

        /// <summary>
        /// Processes a type via reflection.
        /// </summary>
        /// <param name="assemblyTemplate">The assembly template</param>
        /// <param name="type">The type</param>
        public virtual void ProcessType(MbUnitAssemblyTemplate assemblyTemplate, ITypeInfo type)
        {
            TypePatternAttribute typePatternAttribute = AttributeUtils.GetAttribute<TypePatternAttribute>(type, true);
            if (typePatternAttribute == null || !ReflectionUtils.CanInstantiate(type))
                return;

            MbUnitTypeTemplate typeTemplate = typePatternAttribute.CreateTemplate(this, assemblyTemplate, type);
            assemblyTemplate.AddTypeTemplate(typeTemplate);

            typePatternAttribute.Apply(this, typeTemplate);
        }


        /// <summary>
        /// Processes all type decorators via reflection.
        /// </summary>
        /// <param name="typeTemplate">The type template</param>
        public virtual void ProcessTypeDecorators(MbUnitTypeTemplate typeTemplate)
        {
            List<TypeDecoratorPatternAttribute> decorators = new List<TypeDecoratorPatternAttribute>();
            decorators.AddRange(AttributeUtils.GetAttributes < TypeDecoratorPatternAttribute>(typeTemplate.Type, true));
            decorators.AddRange(AttributeUtils.GetAttributes<TypeDecoratorPatternAttribute>(typeTemplate.Type.Assembly, true));
            decorators.Sort(DecoratorOrderComparer < TypeDecoratorPatternAttribute>.Instance);

            foreach (TypeDecoratorPatternAttribute decoratorAttribute in decorators)
            {
                decoratorAttribute.Apply(this, typeTemplate);
            }
        }

        /// <summary>
        /// Processes a method using reflection.
        /// </summary>
        /// <param name="typeTemplate">The type template</param>
        /// <param name="method">The method to process</param>
        public virtual void ProcessMethod(MbUnitTypeTemplate typeTemplate, IMethodInfo method)
        {
            MethodPatternAttribute methodPatternAttribute = AttributeUtils.GetAttribute<MethodPatternAttribute>(method, true);
            if (methodPatternAttribute == null)
                return;

            MbUnitMethodTemplate methodTemplate = methodPatternAttribute.CreateTemplate(this, typeTemplate, method);
            typeTemplate.AddMethodTemplate(methodTemplate);

            methodPatternAttribute.Apply(this, methodTemplate);
        }

        /// <summary>
        /// Processes all method decorators via reflection.
        /// </summary>
        /// <param name="methodTemplate">The method template</param>
        public virtual void ProcessMethodDecorators(MbUnitMethodTemplate methodTemplate)
        {
            List<MethodDecoratorPatternAttribute> decorators = new List<MethodDecoratorPatternAttribute>();
            decorators.AddRange(AttributeUtils.GetAttributes<MethodDecoratorPatternAttribute>(methodTemplate.Method, true));
            decorators.AddRange(AttributeUtils.GetAttributes<MethodDecoratorPatternAttribute>(methodTemplate.Method.DeclaringType, true));
            decorators.Sort(DecoratorOrderComparer<MethodDecoratorPatternAttribute>.Instance);

            foreach (MethodDecoratorPatternAttribute decoratorAttribute in decorators)
            {
                decoratorAttribute.Apply(this, methodTemplate);
            }
        }

        /// <summary>
        /// Processes a slot using reflection and attaches a test parameter to the template if applicable.
        /// </summary>
        /// <param name="template">The template to which the parameter should be attached</param>
        /// <param name="slot">The slot to process</param>
        public virtual void ProcessParameter(MbUnitTemplate template, ISlotInfo slot)
        {
            ParameterPatternAttribute parameterPatternAttribute = AttributeUtils.GetAttribute<ParameterPatternAttribute>(slot, true);

            if (parameterPatternAttribute == null)
            {
                if (slot is IPropertyInfo | slot is IFieldInfo)
                {
                    // Fields and properties are not automatically treated as template parameters
                    // because they might be used by the test for unrelated purposes.  To disambiguate
                    // we check if we have at least one other pattern attribute on this member
                    // then assume we just happened to elide the parameter attribute in this case.
                    // This provides a particularly nice shortcut for self-binding parameters
                    // with data attributes.
                    if (slot.HasAttribute(typeof(PatternAttribute), true))
                        return;
                }

                parameterPatternAttribute = ParameterPatternAttribute.DefaultInstance;
            }

            MbUnitTemplateParameter parameter = parameterPatternAttribute.CreateParameter(this, template, slot);
            template.AddParameter(parameter);

            parameterPatternAttribute.Apply(this, parameter);
        }

        /// <summary>
        /// Processes a slot using reflection and attaches a test parameter to the template if applicable.
        /// </summary>
        /// <param name="parameter">The template parameter to decorate</param>
        public virtual void ProcessParameterDecorators(MbUnitTemplateParameter parameter)
        {
            List<ParameterDecoratorPatternAttribute> decorators = new List<ParameterDecoratorPatternAttribute>();
            decorators.AddRange(AttributeUtils.GetAttributes<ParameterDecoratorPatternAttribute>(parameter.Slot, true));
            decorators.Sort(DecoratorOrderComparer<ParameterDecoratorPatternAttribute>.Instance);

            foreach (ParameterDecoratorPatternAttribute decoratorAttribute in decorators)
            {
                decoratorAttribute.Apply(this, parameter);
            }
        }

        /// <summary>
        /// Scans a code element using reflection to apply metadata pattern attributes.
        /// </summary>
        /// <param name="component">The component to which metadata should be applied</param>
        /// <param name="codeElement">The code element bearing the metadata</param>
        public virtual void ProcessMetadata(ITemplateComponent component, ICodeElementInfo codeElement)
        {
            foreach (MetadataPatternAttribute metadataAttribute in AttributeUtils.GetAttributes<MetadataPatternAttribute>(codeElement, true))
            {
                metadataAttribute.Apply(this, component);
            }

            IAssemblyInfo assembly = codeElement as IAssemblyInfo;
            if (assembly != null)
                ModelUtils.PopulateMetadataFromAssembly(assembly, component.Metadata);

            string xmlDocumentation = codeElement.GetXmlDocumentation();
            if (xmlDocumentation != null)
                component.Metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);
        }
    }
}
