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

using System;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Model.Diagnostics;
using Gallio.Reflection;
using Gallio;

namespace MbUnit.Framework
{
    /// <summary>
    /// Runs the test as if it were surrounded by <see cref="Assert.Multiple(Action)" /> so that
    /// multiple assertion failures within the test are tolerated.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When an assertion failure occurs, it is reported but the test is allowed to proceed
    /// until it completes or throws an unhandled exception.  When the test finishes, it will
    /// still be marked as failed, as usual.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    [TestFrameworkInternal]
    public class MultipleAssertsAttribute : TestMethodDecoratorPatternAttribute
    {
        /// <inheritdoc />
        protected override void DecorateMethodTest(IPatternScope methodScope, IMethodInfo method)
        {
            methodScope.TestBuilder.TestInstanceActions.ExecuteTestInstanceChain.Around(WithMultiple);
        }

        [TestEntryPoint]
        private void WithMultiple(PatternTestInstanceState state, Action<PatternTestInstanceState> action)
        {
            Assert.Multiple(() => action(state));
        }
    }
}
