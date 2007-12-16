// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Model.Reflection;
using MbUnit.Model;
using MbUnit.Model.Builder;
using MbUnit.Model.Patterns;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// The test fixture set up attribute is applied to a method that is to be invoked when
    /// a test fixture instance is being set up before any of its tests are executed.
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
    /// <todo author="jeff">
    /// We should support explicit ordering of set up attributes based on
    /// an Order property similar to decorators.
    /// </todo>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class TestFixtureSetUpAttribute : ContributionPatternAttribute
    {
        /// <inheritdoc />
        protected override void DecorateContainingTest(ITestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
            IMethodInfo method = (IMethodInfo) codeElement;
            ReflectionUtils.CheckMethodSignature(method);

            containingTestBuilder.Test.SetUpChain.After(MbUnitTestUtils.CreateFixtureMethodInvoker(method));
        }
    }
}
