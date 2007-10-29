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
    /// Declares that an assembly contains MbUnit tests.  Subclasses of this
    /// attribute can customize how template enumeration takes place within
    /// an assembly.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear in any given assembly.
    /// If the attribute is omitted, the assembly is scanned using the default
    /// reflection algorithm.
    /// </para>
    /// </summary>
    /// <seealso cref="AssemblyDecoratorPatternAttribute"/>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=false, Inherited=true)]
    public abstract class AssemblyPatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Gets a default instance of the assembly pattern attribute to use
        /// when the attribute is elided from a test assembly.
        /// </summary>
        public static readonly AssemblyPatternAttribute DefaultInstance = new DefaultImpl();

        /// <summary>
        /// Creates a test assembly template.
        /// This method is called when an assembly that appears to contain unit
        /// tests is discovered via reflection to create a new model object to represent it.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="frameworkTemplate">The containing framework template</param>
        /// <param name="assembly">The assembly</param>
        /// <returns>The test assembly template</returns>
        public virtual MbUnitAssemblyTemplate CreateTemplate(TemplateTreeBuilder builder,
            MbUnitFrameworkTemplate frameworkTemplate, Assembly assembly)
        {
            return new MbUnitAssemblyTemplate(frameworkTemplate, assembly);
        }

        /// <summary>
        /// <para>
        /// Applies contributions to an assembly template.
        /// This method is called after the assembly template is linked to the template tree.
        /// </para>
        /// <para>
        /// Contributions are applied in a very specific order:
        /// <list type="bullet">
        /// <item>Assembly decorator attributes declared by the assembly</item>
        /// <item>Metadata attributes declared by the assembly</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <remarks>
        /// A typical use of this method is to apply additional metadata to model
        /// objects in the template tree and to further expand the tree using
        /// declarative metadata derived via reflection.
        /// </remarks>
        /// <param name="builder">The template tree builder</param>
        /// <param name="assemblyTemplate">The assembly template</param>
        public virtual void Apply(TemplateTreeBuilder builder, MbUnitAssemblyTemplate assemblyTemplate)
        {
            AssemblyDecoratorPatternAttribute.ProcessDecorators(builder, assemblyTemplate, assemblyTemplate.Assembly);
            MetadataPatternAttribute.ProcessMetadata(builder, assemblyTemplate, assemblyTemplate.Assembly);

            ProcessPublicTypes(builder, assemblyTemplate);
        }

        /// <summary>
        /// Processes an assembly using reflection to build a template tree.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="frameworkTemplate">The containing framework template</param>
        /// <param name="assembly">The assembly to process</param>
        public static void ProcessAssembly(TemplateTreeBuilder builder, MbUnitFrameworkTemplate frameworkTemplate,
            Assembly assembly)
        {
            AssemblyPatternAttribute assemblyPatternAttribute = ModelUtils.GetAttribute<AssemblyPatternAttribute>(assembly);
            if (assemblyPatternAttribute == null)
                assemblyPatternAttribute = DefaultInstance;

            MbUnitAssemblyTemplate assemblyTemplate = assemblyPatternAttribute.CreateTemplate(builder, frameworkTemplate, assembly);
            frameworkTemplate.AddAssemblyTemplate(assemblyTemplate);
            assemblyPatternAttribute.Apply(builder, assemblyTemplate);

            // Add assembly-level metadata.
            ModelUtils.PopulateMetadataFromAssembly(assembly, assemblyTemplate.Metadata);
        }

        /// <summary>
        /// Processes all public types within the assembly via reflection.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="assemblyTemplate">The assembly template</param>
        protected virtual void ProcessPublicTypes(TemplateTreeBuilder builder, MbUnitAssemblyTemplate assemblyTemplate)
        {
            foreach (Type type in assemblyTemplate.Assembly.GetExportedTypes())
            {
                ProcessType(builder, assemblyTemplate, type);
            }
        }

        /// <summary>
        /// Processes a type via reflection.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="assemblyTemplate">The assembly template</param>
        /// <param name="type">The type</param>
        protected virtual void ProcessType(TemplateTreeBuilder builder, MbUnitAssemblyTemplate assemblyTemplate, Type type)
        {
            TypePatternAttribute.ProcessType(builder, assemblyTemplate, type);
        }

        private class DefaultImpl : AssemblyPatternAttribute
        {
        }
    }
}
