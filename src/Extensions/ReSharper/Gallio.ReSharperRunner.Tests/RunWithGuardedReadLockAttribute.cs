#if ! RESHARPER_31
using System;
using Gallio.Framework.Pattern;
using Gallio.Reflection;
using JetBrains.Application;
using JetBrains.Application.Test;
using MbUnit.Framework;

namespace Gallio.ReSharperRunner.Tests
{
    public class RunWithGuardedReadLockAttribute : TestDecoratorPatternAttribute
    {
        protected override void DecorateTest(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            scope.Test.TestInstanceActions.DecorateChildTestChain.After(delegate(PatternTestInstanceState state, PatternTestActions actions)
            {
                actions.TestInstanceActions.ExecuteTestInstanceChain.Around(delegate(PatternTestInstanceState childState, Action<PatternTestInstanceState> inner)
                {
                    TestShell.RunGuarded(delegate
                    {
                        using (ReadLockCookie.Create())
                        {
                            inner(childState);
                        }
                    });
                });
            });
        }
    }
}
#endif