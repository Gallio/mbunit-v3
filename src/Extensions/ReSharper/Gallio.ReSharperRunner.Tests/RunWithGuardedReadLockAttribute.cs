// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if ! RESHARPER_31
using System;
using Gallio.Framework.Pattern;
using Gallio.Reflection;
using JetBrains.Application;
using JetBrains.Application.Test;
using MbUnit.Framework;

namespace Gallio.ReSharperRunner.Tests
{
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public class RunWithGuardedReadLockAttribute : TestDecoratorPatternAttribute
    {
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            scope.TestBuilder.TestInstanceActions.DecorateChildTestChain.After(delegate(PatternTestInstanceState state, PatternTestActions actions)
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
