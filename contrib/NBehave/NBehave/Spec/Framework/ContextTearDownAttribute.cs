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
    /// evaluating all of its specifications.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public class ContextTearDownAttribute : ContributionPatternAttribute
    {
        /// <inheritdoc />
        protected override void DecorateContainingTest(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
            if (containingTestBuilder.Test.Kind != NBehaveTestKinds.Context)
                throw new ModelException("The [ContextTearDown] attribute can only appear within a context class.");

            IMethodInfo method = (IMethodInfo)codeElement;

            containingTestBuilder.Test.TestInstanceActions.TearDownTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    testInstanceState.InvokeFixtureMethod(method, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
                });
        }
    }
}