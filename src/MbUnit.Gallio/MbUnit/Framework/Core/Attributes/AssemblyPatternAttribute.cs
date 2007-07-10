using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MbUnit.Core.Model;
using MbUnit.Core.Utilities;
using MbUnit.Framework.Core.Model;

namespace MbUnit.Framework.Core.Attributes
{
    /// <summary>
    /// <para>
    /// Declares that an assembly contains MbUnit tests.  Subclasses of this
    /// attribute can customize how test template enumeration takes place within
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
        /// when none was specified.
        /// </summary>
        public static readonly AssemblyPatternAttribute DefaultInstance = new DefaultImpl();

        /// <summary>
        /// Creates a test assembly template.
        /// This method is called when an assembly that appears to contain unit
        /// tests is discovered via reflection to create a new model object to represent it.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="frameworkTemplate">The containing framework template</param>
        /// <param name="assembly">The assembly</param>
        /// <returns>The test assembly template</returns>
        public virtual MbUnitTestAssemblyTemplate CreateTemplate(TestTemplateTreeBuilder builder,
            MbUnitTestFrameworkTemplate frameworkTemplate, Assembly assembly)
        {
            return new MbUnitTestAssemblyTemplate(frameworkTemplate, assembly);
        }

        /// <summary>
        /// Applies contributions to an assembly template.
        /// This method is called after the assembly template is linked to the template tree.
        /// </summary>
        /// <remarks>
        /// A typical use of this method is to apply additional metadata to model
        /// objects in the test template tree and to further expand the tree using
        /// declarative metadata derived via reflection.
        /// </remarks>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="assemblyTemplate">The assembly template</param>
        public virtual void Apply(TestTemplateTreeBuilder builder, MbUnitTestAssemblyTemplate assemblyTemplate)
        {
            AssemblyDecoratorPatternAttribute.ProcessDecorators(builder, assemblyTemplate, assemblyTemplate.Assembly);
            MetadataPatternAttribute.ProcessMetadata(builder, assemblyTemplate, assemblyTemplate.Assembly);

            ProcessPublicTypes(builder, assemblyTemplate);
        }

        /// <summary>
        /// Processes an assembly using reflection to build a test template tree.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="frameworkTemplate">The containing framework template</param>
        /// <param name="assembly">The assembly to process</param>
        public static void ProcessAssembly(TestTemplateTreeBuilder builder, MbUnitTestFrameworkTemplate frameworkTemplate,
            Assembly assembly)
        {
            AssemblyPatternAttribute assemblyPatternAttribute = ReflectionUtils.GetAttribute<AssemblyPatternAttribute>(assembly);
            if (assemblyPatternAttribute == null)
                assemblyPatternAttribute = DefaultInstance;

            MbUnitTestAssemblyTemplate assemblyTemplate = assemblyPatternAttribute.CreateTemplate(builder, frameworkTemplate, assembly);
            frameworkTemplate.AddAssemblyTemplate(assemblyTemplate);
            assemblyPatternAttribute.Apply(builder, assemblyTemplate);
        }

        /// <summary>
        /// Processes all public types within the assembly via reflection.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="assemblyTemplate">The assembly template</param>
        protected virtual void ProcessPublicTypes(TestTemplateTreeBuilder builder, MbUnitTestAssemblyTemplate assemblyTemplate)
        {
            foreach (Type type in assemblyTemplate.Assembly.GetTypes())
            {
                ProcessType(builder, assemblyTemplate, type);
            }
        }

        /// <summary>
        /// Processes a type via reflection.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="assemblyTemplate">The assembly template</param>
        /// <param name="type">The type</param>
        protected virtual void ProcessType(TestTemplateTreeBuilder builder, MbUnitTestAssemblyTemplate assemblyTemplate, Type type)
        {
            FixturePatternAttribute.ProcessType(builder, assemblyTemplate, type);
        }

        private class DefaultImpl : AssemblyPatternAttribute
        {
        }
    }
}
