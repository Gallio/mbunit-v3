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
using Gallio.Model.Execution;
using Gallio.Model;

namespace Gallio.Plugin.XunitAdapter.Model
{
    /// <summary>
    /// Wraps an Xunit test.
    /// </summary>
    /// <remarks author="jeff">
    /// Looks like Type will be replaced by ITypeInfo in Xunit beta 2.
    /// </remarks>
    public class XunitTest : BaseTest
    {
        private readonly Type typeInfo;
        private readonly MethodInfo methodInfo;

        /// <summary>
        /// Initializes a test initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <param name="templateBinding">The template binding that produced this test</param>
        /// <param name="typeInfo">The Xunit test type information</param>
        /// <param name="methodInfo">The Xunit test method information, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>,
        /// <paramref name="codeReference"/>, <paramref name="templateBinding"/>, or <paramref name="typeInfo"/> is null</exception>
        public XunitTest(string name, CodeReference codeReference, XunitFrameworkTemplateBinding templateBinding,
            Type typeInfo, MethodInfo methodInfo)
            : base(name, codeReference, templateBinding)
        {
            if (typeInfo == null)
                throw new ArgumentNullException(@"typeInfo");

            this.typeInfo = typeInfo;
            this.methodInfo = methodInfo;
        }

        /// <summary>
        /// Gets the Xunit test type information.
        /// </summary>
        public Type TypeInfo
        {
            get { return typeInfo; }
        }

        /// <summary>
        /// Gets the Xunit test method information.
        /// </summary>
        public MethodInfo MethodInfo
        {
            get { return methodInfo; }
        }

        /// <inheritdoc />
        public override ITestController CreateTestController()
        {
            return new XunitTestController();
        }
    }
}
