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
using Gallio.Common;
using Gallio.Model.Execution;
using Gallio.Model;
using Gallio.Common.Reflection;

namespace Gallio.XunitAdapter.Model
{
    /// <summary>
    /// Wraps an Xunit test.
    /// </summary>
    internal class XunitTest : BaseTest
    {
        private readonly XunitTypeInfoAdapter typeInfo;
        private readonly XunitMethodInfoAdapter methodInfo;

        /// <summary>
        /// Initializes a test initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeElement">The point of definition, or null if none</param>
        /// <param name="typeInfo">The Xunit test type information</param>
        /// <param name="methodInfo">The Xunit test method information, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="typeInfo"/> is null</exception>
        public XunitTest(string name, ICodeElementInfo codeElement, XunitTypeInfoAdapter typeInfo, XunitMethodInfoAdapter methodInfo)
            : base(name, codeElement)
        {
            if (typeInfo == null)
                throw new ArgumentNullException(@"typeInfo");

            this.typeInfo = typeInfo;
            this.methodInfo = methodInfo;
        }

        /// <summary>
        /// Gets the Xunit test type information.
        /// </summary>
        public XunitTypeInfoAdapter TypeInfo
        {
            get { return typeInfo; }
        }

        /// <summary>
        /// Gets the Xunit test method information.
        /// </summary>
        public XunitMethodInfoAdapter MethodInfo
        {
            get { return methodInfo; }
        }

        /// <inheritdoc />
        public override Func<ITestController> TestControllerFactory
        {
            get { return CreateTestController; }
        }

        private static ITestController CreateTestController()
        {
            return new XunitTestController();
        }
    }
}
