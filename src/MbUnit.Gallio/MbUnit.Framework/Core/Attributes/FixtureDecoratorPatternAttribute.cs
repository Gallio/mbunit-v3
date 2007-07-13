using System;
using System.Reflection;
using MbUnit.Framework.Model;
using MbUnit.Framework.Core.Model;

namespace MbUnit.Framework.Core.Attributes
{
    /// <summary>
    /// A fixture decorator pattern attribute applies various contributions to an
    /// existing fixture template model object.
    /// </summary>
    /// <seealso cref="FixturePatternAttribute"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class FixtureDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <summary>
        /// Applies contributions to an fixture template.
        /// This method is called after the contributions of the <see cref="FixturePatternAttribute"/>
        /// have been applied but before any further processing takes place.
        /// </summary>
        /// <remarks>
        /// A typical use of this method is to apply additional metadata to model
        /// objects in the test template tree and to further expand the tree using
        /// declarative metadata derived via reflection.
        /// </remarks>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="fixtureTemplate">The fixture template</param>
        public virtual void Apply(TestTemplateTreeBuilder builder, MbUnitTestFixtureTemplate fixtureTemplate)
        {
        }

        /// <summary>
        /// Processes all fixture decorators via reflection.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="fixtureTemplate">The fixture template</param>
        /// <param name="attributeProvider">The attribute provider to scan</param>
        public static void ProcessDecorators(TestTemplateTreeBuilder builder, MbUnitTestFixtureTemplate fixtureTemplate, ICustomAttributeProvider attributeProvider)
        {
            object[] decorators = attributeProvider.GetCustomAttributes(typeof(FixtureDecoratorPatternAttribute), true);
            Sort(decorators);

            foreach (FixtureDecoratorPatternAttribute decoratorAttribute in decorators)
            {
                decoratorAttribute.Apply(builder, fixtureTemplate);
            }
        }
    }
}
