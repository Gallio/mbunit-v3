// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// The fixture tear down attribute is applied to a method that is to be invoked
    /// when a fixture instance is being torn down after all of its tests have been executed.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The attribute may be applied to multiple methods within a fixture, however
    /// the order in which they are processed is undefined.
    /// </para>
    /// <para>
    /// The method to which this attribute is applied must be declared by the
    /// fixture class and must not have any parameters.  The method may be static.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class FixtureTearDownAttribute : ContributionPatternAttribute
    {
        /// <inheritdoc />
        protected override void DecorateContainingTest(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
            IMethodInfo method = (IMethodInfo)codeElement;

            containingTestBuilder.Test.TestInstanceActions.TearDownTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    testInstanceState.InvokeFixtureMethod(method, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
                });
        }
    }
}
