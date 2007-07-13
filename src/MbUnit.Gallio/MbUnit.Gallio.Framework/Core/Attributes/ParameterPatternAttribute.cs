using System;
using System.Reflection;
using MbUnit.Framework.Model;
using MbUnit.Framework.Core.Model;
using MbUnit.Framework.Utilities;

namespace MbUnit.Framework.Core.Attributes
{
    /// <summary>
    /// Declares that a property, field or parameter is a test parameter and
    /// specifies its properties.  At most one attribute of this type may appear on
    /// any given test fixture property or field.  If the attribute is omitted from
    /// test method parameters and test fixture constructor parameters the parameter
    /// will be declared with default values (which are usually just fine).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter,
        AllowMultiple = false, Inherited = true)]
    public abstract class ParameterPatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Gets a default instance of the parameter pattern attribute to use
        /// when none was specified.
        /// </summary>
        public static readonly ParameterPatternAttribute DefaultInstance = new DefaultImpl();

        /// <summary>
        /// Creates a test parameter.
        /// This method is called when a parameter is discovered via reflection to
        /// create a new model object to represent it.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="parameterSet">The parameter set</param>
        /// <param name="slot">The slot</param>
        /// <returns>The test parameter</returns>
        public virtual MbUnitTestParameter CreateParameter(TestTemplateTreeBuilder builder,
            MbUnitTestParameterSet parameterSet, Slot slot)
        {
            return new MbUnitTestParameter(parameterSet, slot);
        }

        /// <summary>
        /// Applies contributions to a parameter.
        /// This method is called after the parameter is linked to the template tree.
        /// </summary>
        /// <remarks>
        /// A typical use of this method is to apply additional metadata to model
        /// objects in the test template tree and to further expand the tree using
        /// declarative metadata derived via reflection.
        /// </remarks>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="parameter">The parameter</param>
        public virtual void Apply(TestTemplateTreeBuilder builder, MbUnitTestParameter parameter)
        {
            ICustomAttributeProvider attributeProvider = parameter.Slot.AttributeProvider;
            ParameterDecoratorPatternAttribute.ProcessDecorators(builder, parameter, attributeProvider);
            MetadataPatternAttribute.ProcessMetadata(builder, parameter, attributeProvider);

            // TODO: Look for data attributes.
        }

        /// <summary>
        /// Processes a slot using reflection and attaches a test parameter to the template if applicable.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="parameterSet">The default parameter set</param>
        /// <param name="slot">The slot to process</param>
        public static void ProcessSlot(TestTemplateTreeBuilder builder, MbUnitTestParameterSet parameterSet, Slot slot)
        {
            ParameterPatternAttribute parameterPatternAttribute = ReflectionUtils.GetAttribute<ParameterPatternAttribute>(slot.AttributeProvider);

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

            MbUnitTestParameter parameter = parameterPatternAttribute.CreateParameter(builder, parameterSet, slot);
            parameter.ParameterSet.Parameters.Add(parameter);
            parameterPatternAttribute.Apply(builder, parameter);
        }

        private class DefaultImpl : ParameterPatternAttribute
        {
        }
    }
}
