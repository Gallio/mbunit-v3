using System;
using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Reflection;
using NBehave.Core;

namespace NBehave.Spec.Framework
{
    /// <summary>
    /// When applied to a method of a context class, declares a setup action to be performed before
    /// evaluating each specification.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public class SetUpAttribute : ContributionPatternAttribute
    {
        /// <inheritdoc />
        protected override void DecorateContainingTest(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
            if (containingTestBuilder.Test.Kind != NBehaveTestKinds.Context)
                throw new ModelException("The [SetUp] attribute can only appear within a context class.");

            IMethodInfo method = (IMethodInfo)codeElement;

            containingTestBuilder.Test.TestInstanceActions.DecorateChildTestChain.After(
                delegate(PatternTestInstanceState testInstanceState, PatternTestActions decoratedChildActions)
                {
                    decoratedChildActions.TestInstanceActions.SetUpTestInstanceChain.Before(delegate
                    {
                        testInstanceState.InvokeFixtureMethod(method, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
                    });
                });
        }
    }
}
