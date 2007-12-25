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
using Gallio.Reflection;
using MbUnit.Model.Builder;

namespace MbUnit.Model.Patterns
{
    /// <summary>
    /// <para>
    /// A dependency pattern attribute creates a dependency on the tests defined
    /// by some other code element.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method
        | AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public abstract class DependencyPatternAttribute : PatternAttribute
    {
        /// <inheritdoc />
        public override void ProcessTest(ITestBuilder testBuilder, ICodeElementInfo codeElement)
        {
            testBuilder.AddDecorator(int.MaxValue, delegate
            {
                ICodeElementInfo resolvedDependency = GetDependency(testBuilder, codeElement);

                foreach (ITestBuilder dependentTestBuilder in testBuilder.TestModelBuilder.GetTestBuilders(resolvedDependency))
                    testBuilder.AddDependency(dependentTestBuilder.Test);
            });
        }

        /// <summary>
        /// Gets the code element that declares the tests on which this test should depend.
        /// </summary>
        /// <param name="testBuilder">The test builder</param>
        /// <param name="codeElemeent">The code element</param>
        /// <returns>The code element representing the dependency</returns>
        protected abstract ICodeElementInfo GetDependency(ITestBuilder testBuilder, ICodeElementInfo codeElemeent);
    }
}