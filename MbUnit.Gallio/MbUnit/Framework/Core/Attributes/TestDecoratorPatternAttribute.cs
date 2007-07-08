using System;
using System.Reflection;
using MbUnit.Core.Model;
using MbUnit.Framework.Core.Model;

namespace MbUnit.Framework.Core.Attributes
{
    /// <summary>
    /// <para>
    /// A test decorator pattern attribute applies various contributions to an
    /// existing test method template model object.
    /// </para>
    /// <para>
    /// When the attribute is applied to a test fixture class, it affects each of the tests
    /// within that fixture individually.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class TestDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <summary>
        /// Applies contributions to a method template.
        /// This method is called after the contributions of the <see cref="TestPatternAttribute"/>
        /// have been applied but before any further processing takes place.
        /// </summary>
        /// <remarks>
        /// A typical use of this method is to apply additional metadata to model
        /// objects in the test template tree and to further expand the tree using
        /// declarative metadata derived via reflection.
        /// </remarks>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="methodTemplate">The method template</param>
        public virtual void Apply(TestTemplateTreeBuilder builder, MbUnitTestMethodTemplate methodTemplate)
        {
        }

        /// <summary>
        /// Processes all test decorators via reflection.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="methodTemplate">The method template</param>
        /// <param name="attributeProvider">The attribute provider to scan</param>
        public static void ProcessDecorators(TestTemplateTreeBuilder builder, MbUnitTestMethodTemplate methodTemplate, ICustomAttributeProvider attributeProvider)
        {
            object[] decorators = attributeProvider.GetCustomAttributes(typeof(TestDecoratorPatternAttribute), true);
            Sort(decorators);

            foreach (TestDecoratorPatternAttribute decoratorAttribute in decorators)
            {
                decoratorAttribute.Apply(builder, methodTemplate);
            }
        }
    }
}
