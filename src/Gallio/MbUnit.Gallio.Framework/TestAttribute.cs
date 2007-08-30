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
using System.Reflection;
using MbUnit.Framework.Kernel.Attributes;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// The test attribute is applied to a method that represents a single test
    /// case within a fixture.  If the method throws an unexpected exception,
    /// the test will be deemed to have failed.  Otherwise, the test will pass.
    /// Output from the test, such as text written to the console, is captured
    /// by the framework and will be included in the test report.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The method to which this attribute is applied must be declared by the
    /// fixture class and must not have any parameters.  The method may be static.
    /// </para>
    /// </remarks>
    /// <todo author="jeff">
    /// We should support explicit ordering of tests based on
    /// an Order property similar to decorators.  Then we can deprecate the
    /// TestSequence attribute.
    /// </todo>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class TestAttribute : TestPatternAttribute
    {
        /// <override />
        public override void Apply(TemplateTreeBuilder builder, MbUnitMethodTemplate methodTemplate)
        {
            base.Apply(builder, methodTemplate);

            MethodInfo method = methodTemplate.Method;
            ModelUtils.CheckMethodSignature(method);

            methodTemplate.ProcessTestChain.After(delegate(MbUnitTest test)
            {
                test.IsTestCase = true;
                test.ExecuteChain.After(MbUnitTestUtils.CreateFixtureMethodInvoker(method));
            });
        }
    }
}
