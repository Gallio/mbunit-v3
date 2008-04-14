using System;
using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Framework.Pattern;
using Gallio.Reflection;
using NBehave.Core;

namespace NBehave.Spec.Framework
{
    /// <summary>
    /// When applied to a method of a context class, declares a tear down action to be performed after
    /// evaluating each specification.
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.ContributionMethod, AllowMultiple = false, Inherited = true)]
    public class TearDownAttribute : ContributionMethodPatternAttribute
    {
        /// <inheritdoc />
        protected override void DecorateContainingScope(PatternEvaluationScope containingScope, IMethodInfo method)
        {
            containingScope.Test.TestInstanceActions.DecorateChildTestChain.After(
                delegate(PatternTestInstanceState testInstanceState, PatternTestActions decoratedChildActions)
                {
                    decoratedChildActions.TestInstanceActions.TearDownTestInstanceChain.After(delegate
                    {
                        testInstanceState.InvokeFixtureMethod(method, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
                    });
                });
        }

        /// <inheritdoc />
        protected override void Validate(PatternEvaluationScope containingScope, IMethodInfo method)
        {
            base.Validate(containingScope, method);

            if (!containingScope.IsTestDeclaration
                || containingScope.Test.Kind != NBehaveTestKinds.Context)
                throw new PatternUsageErrorException("The [TearDown] attribute can only appear on a method within a context class.");
        }
    }
}