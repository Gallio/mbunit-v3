// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// Specifies that a test fixture or a test method is abstract and should not run.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The attribute hides the test methods in the current test fixture or a test method itself, 
    /// so they can be consumed properly in the desired context from any derived concrete test fixtures.
    /// </para>
    /// <para>
    /// This attribute is not inherited.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = false)]
    public class DisableAttribute : TestDecoratorPatternAttribute
    {
        /// <inheritdoc />
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            scope.TestBuilder.TestActions.BeforeTestChain.Before(state =>
            {
                 throw new SilentTestException(TestOutcome.Skipped, "Abstract test method/fixture.");
            });
        }
    }
}
