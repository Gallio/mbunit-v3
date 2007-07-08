using System;
using System.Reflection;
using MbUnit.Core.Model;
using MbUnit.Framework.Core.Model;

namespace MbUnit.Framework.Core.Attributes
{
    /// <summary>
    /// An assembly decorator pattern attribute applies various contributions to an
    /// existing assembly template model object.
    /// </summary>
    /// <seealso cref="AssemblyPatternAttribute"/>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true, Inherited=true)]
    public abstract class AssemblyDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <summary>
        /// Applies contributions to an assembly template.
        /// This method is called after the contributions of the <see cref="AssemblyPatternAttribute"/>
        /// have been applied but before any further processing takes place.
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
        }

        /// <summary>
        /// Processes all assembly decorators via reflection.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="assemblyTemplate">The assembly template</param>
        /// <param name="attributeProvider">The attribute provider to scan</param>
        public static void ProcessDecorators(TestTemplateTreeBuilder builder, MbUnitTestAssemblyTemplate assemblyTemplate, ICustomAttributeProvider attributeProvider)
        {
            object[] decorators = attributeProvider.GetCustomAttributes(typeof(AssemblyDecoratorPatternAttribute), true);
            Sort(decorators);

            foreach (AssemblyDecoratorPatternAttribute decoratorAttribute in decorators)
            {
                decoratorAttribute.Apply(builder, assemblyTemplate);
            }
        }
    }
}
