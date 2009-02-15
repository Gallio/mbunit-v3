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
using System.Reflection;
using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Creates a dependency from this test assembly, test fixture or test method on some other test
    /// fixture or test method.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the other test fixture or test method fails then this test
    /// will not run.  Moreover, the dependency forces this test to run after those
    /// it depends upon.
    /// </para>
    /// <para>
    /// This attribute can be repeated multiple times if there are multiple dependencies.
    /// </para>
    /// </remarks>
    public class DependsOnAttribute : TestDependencyPatternAttribute
    {
        private readonly Type testFixtureType;
        private readonly string testMethodName;

        /// <summary>
        /// Creates a dependency from this test on another test fixture.
        /// </summary>
        /// <param name="testFixtureType">The dependent test fixture type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testFixtureType"/> is null</exception>
        public DependsOnAttribute(Type testFixtureType)
        {
            if (testFixtureType == null)
                throw new ArgumentNullException("testFixtureType");

            this.testFixtureType = testFixtureType;
        }

        /// <summary>
        /// Creates a dependency from this test on a particular test method within another test fixture.
        /// </summary>
        /// <param name="testFixtureType">The dependent test fixture type</param>
        /// <param name="testMethodName">The dependent test method name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testFixtureType"/> or <paramref name="testMethodName"/> is null</exception>
        public DependsOnAttribute(Type testFixtureType, string testMethodName)
            : this(testFixtureType)
        {
            if (testMethodName == null)
                throw new ArgumentNullException("testMethodName");

            this.testMethodName = testMethodName;
        }

        /// <summary>
        /// Creates a dependency from this test on another test method within this test fixture.
        /// </summary>
        /// <param name="testMethodName">The dependent test method name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testMethodName"/> is null</exception>
        public DependsOnAttribute(string testMethodName)
        {
            if (testMethodName == null)
                throw new ArgumentNullException("testMethodName");

            this.testMethodName = testMethodName;
        }

        /// <summary>
        /// Gets the dependent test fixture type, or null if the dependency is on another
        /// test method of this test fixture.
        /// </summary>
        public Type TestFixtureType
        {
            get { return testFixtureType; }
        }

        /// <summary>
        /// Gets the dependent test method name, or null if the dependency
        /// is on the whole test fixture.
        /// </summary>
        public string TestMethodName
        {
            get { return testMethodName; }
        }

        /// <inheritdoc />
        protected override ICodeElementInfo GetDependency(IPatternScope scope, ICodeElementInfo codeElement)
        {
            ITypeInfo resolvedFixtureType;
            if (testFixtureType != null)
            {
                resolvedFixtureType = Reflector.Wrap(testFixtureType);
            }
            else
            {
                resolvedFixtureType = ReflectionUtils.GetType(codeElement);
                if (resolvedFixtureType == null)
                    ThrowUsageErrorException(String.Format("Could not resolve dependency on test method '{0}' because the declaring fixture type is not known.", testMethodName));
            }

            if (testMethodName == null)
                return resolvedFixtureType;

            IMethodInfo resolvedMethod = resolvedFixtureType.GetMethod(testMethodName, BindingFlags.Public | BindingFlags.Instance);
            if (resolvedMethod == null)
                ThrowUsageErrorException(String.Format("Could not resolve dependency on test method '{0}' of test fixture type '{1}' because the method was not found.",
                    testMethodName, resolvedFixtureType));
            return resolvedMethod;
        }
    }
}