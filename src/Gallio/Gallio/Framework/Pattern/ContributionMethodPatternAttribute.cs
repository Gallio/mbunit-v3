using System;
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// A contribution method pattern attribute applies decorations to a containing scope
    /// such as by introducing a new setup or teardown action to a test.
    /// </para>
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.ContributionMethod, AllowMultiple=false, Inherited=true)]
    public abstract class ContributionMethodPatternAttribute : DecoratorPatternAttribute
    {
        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override void Consume(PatternEvaluationScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            IMethodInfo method = codeElement as IMethodInfo;
            Validate(containingScope, method);

            containingScope.AddDecorator(Order, delegate
            {
                DecorateContainingScope(containingScope, method);
            });
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="containingScope">The containing scope</param>
        /// <param name="method">The method</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly</exception>
        protected virtual void Validate(PatternEvaluationScope containingScope, IMethodInfo method)
        {
        }

        /// <summary>
        /// <para>
        /// Applies decorations to the containing <see cref="PatternTest" />.
        /// </para>
        /// </summary>
        /// <param name="containingScope">The containing scope</param>
        /// <param name="method">The method to process</param>
        protected virtual void DecorateContainingScope(PatternEvaluationScope containingScope, IMethodInfo method)
        {
            if (containingScope.IsTestDeclaration || method == null)
                ThrowUsageErrorException(String.Format("This attribute can only be used on a method within a test type."));
        }
    }
}