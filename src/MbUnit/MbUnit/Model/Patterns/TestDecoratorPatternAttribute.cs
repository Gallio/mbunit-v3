using System;
using Gallio.Model.Reflection;
using MbUnit.Model.Builder;

namespace MbUnit.Model.Patterns
{
    /// <summary>
    /// <para>
    /// A test decorator pattern attribute applies decorations to an
    /// existing type or method level <see cref="MbUnitTest" />.
    /// </para>
    /// </summary>
    /// <seealso cref="TestTypePatternAttribute"/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method,
        AllowMultiple = true, Inherited = true)]
    public abstract class TestDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <inheritdoc />
        public override void ProcessTest(ITestBuilder testBuilder, ICodeElementInfo codeElement)
        {
            testBuilder.AddDecorator(Order, delegate(ITestBuilder typeTestBuilder)
            {
                DecorateTest(typeTestBuilder, codeElement);
            });
        }

        /// <summary>
        /// <para>
        /// Applies decorations to a method or type-level <see cref="MbUnitTest" />.
        /// </para>
        /// <para>
        /// A typical use of this method is to augment the test with additional metadata
        /// or to add additional behaviors to the test.
        /// </para>
        /// </summary>
        /// <param name="builder">The test builder</param>
        /// <param name="codeElement">The code element</param>
        protected virtual void DecorateTest(ITestBuilder builder, ICodeElementInfo codeElement)
        {
        }
    }
}