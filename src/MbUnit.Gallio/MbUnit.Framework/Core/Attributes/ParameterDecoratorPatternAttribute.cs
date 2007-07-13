using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MbUnit.Framework.Model;
using MbUnit.Framework.Core.Model;

namespace MbUnit.Framework.Core.Attributes
{
    /// <summary>
    /// A parameter decorator pattern attribute applies various contributions to an
    /// existing parameter model object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = true)]
    public abstract class ParameterDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <summary>
        /// Applies contributions to a parameter.
        /// This method is called after the contributions of the <see cref="ParameterPatternAttribute"/>
        /// have been applied but before any further processing takes place.
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
        }

        /// <summary>
        /// Processes all parameter decorators via reflection.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="attributeProvider">The attribute provider to scan</param>
        public static void ProcessDecorators(TestTemplateTreeBuilder builder, MbUnitTestParameter parameter, ICustomAttributeProvider attributeProvider)
        {
            object[] decorators = attributeProvider.GetCustomAttributes(typeof(ParameterDecoratorPatternAttribute), true);
            Sort(decorators);

            foreach (ParameterDecoratorPatternAttribute decoratorAttribute in decorators)
            {
                decoratorAttribute.Apply(builder, parameter);
            }
        }
    }
}
