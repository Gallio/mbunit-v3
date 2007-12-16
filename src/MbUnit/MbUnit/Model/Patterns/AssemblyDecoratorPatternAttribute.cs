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

namespace MbUnit.Model.Patterns
{
    /// <summary>
    /// A test assembly decorator pattern attribute applies decorations to an
    /// existing assembly-level test.
    /// </summary>
    /// <seealso cref="AssemblyPatternAttribute"/>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true, Inherited=true)]
    public abstract class AssemblyDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <inheritdoc />
        public override void ProcessTest(ITestBuilder testBuilder, ICodeElementInfo codeElement)
        {
            IAssemblyInfo assembly = (IAssemblyInfo)codeElement;

            testBuilder.AddDecorator(Order, delegate(ITestBuilder assemblyTestBuilder)
            {
                DecorateAssemblyTest(assemblyTestBuilder, assembly);
            });
        }

        /// <summary>
        /// <para>
        /// Applies decorations to an assembly-level <see cref="MbUnitTest" />.
        /// </para>
        /// <para>
        /// A typical use of this method is to augment the test with additional metadata
        /// or to add additional behaviors to the test.
        /// </para>
        /// </summary>
        /// <param name="builder">The test builder</param>
        /// <param name="assembly">The assembly</param>
        protected virtual void DecorateAssemblyTest(ITestBuilder builder, IAssemblyInfo assembly)
        {
        }
    }
}