using System;
using System.Reflection;
using MbUnit.Framework.Model;
using MbUnit.Framework.Core.Model;

namespace MbUnit.Framework.Core.Attributes
{
    /// <summary>
    /// Applies declarative metadata to an MbUnit model object.  A metadata attribute is
    /// similar to a decorator but more restrictive.  Metadata does not modify the structure
    /// of a model object directly.  Instead it introduces additional properties that are
    /// useful for classification, filtering, reporting, documentation or other purposes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class
        | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field
        | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public abstract class MetadataPatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Applies metadata contributions to the specified component.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="component">The component to which metadata should be applied</param>
        public virtual void Apply(TestTemplateTreeBuilder builder, ITestComponent component)
        {
        }

        /// <summary>
        /// Scans a code element using reflection to apply metadata pattern attributes.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="component">The component to which metadata should be applied</param>
        /// <param name="attributeProvider">The attribute provider to scan</param>
        public static void ProcessMetadata(TestTemplateTreeBuilder builder, ITestComponent component, ICustomAttributeProvider attributeProvider)
        {
            foreach (MetadataPatternAttribute metadataAttribute in attributeProvider.GetCustomAttributes(typeof(MetadataPatternAttribute), true))
            {
                metadataAttribute.Apply(builder, component);
            }
        }
    }
}
