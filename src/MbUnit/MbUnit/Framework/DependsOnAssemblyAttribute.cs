// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Creates a dependency from this test assembly, test fixture or test method on all tests
    /// in some other test assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If any test in the other test assembly test fails then this
    /// test will not run.  Moreover, the dependency forces this test to run after those it depends upon.
    /// </para>
    /// <para>
    /// This attribute can be repeated multiple times if there are multiple dependencies.
    /// </para>
    /// </remarks>
    public class DependsOnAssemblyAttribute : TestDependencyPatternAttribute
    {
        private readonly string testAssemblyName;

        /// <summary>
        /// Creates a dependency from this test on all tests in another
        /// test assembly.
        /// </summary>
        /// <param name="testAssemblyName">The dependent test assembly name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testAssemblyName"/> is null</exception>
        public DependsOnAssemblyAttribute(string testAssemblyName)
        {
            if (testAssemblyName == null)
                throw new ArgumentNullException("testAssemblyName");

            this.testAssemblyName = testAssemblyName;
        }

        /// <summary>
        /// Gets the dependent test assembly name.
        /// </summary>
        public string TestAssemblyName
        {
            get { return testAssemblyName; }
        }

        /// <inheritdoc />
        protected override ICodeElementInfo GetDependency(IPatternScope scope, ICodeElementInfo codeElement)
        {
            try
            {
                return scope.TestModelBuilder.ReflectionPolicy.LoadAssembly(new AssemblyName(testAssemblyName));
            }
            catch (Exception ex)
            {
                throw new PatternUsageErrorException(
                    String.Format("Could not resolve dependency on test assembly '{0}' because the assembly could not be loaded.", testAssemblyName),
                    ex);
            }
        }
    }
}