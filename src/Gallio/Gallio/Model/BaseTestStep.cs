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
using Gallio.Properties;

namespace Gallio.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITestStep"/>.
    /// </summary>
    public class BaseTestStep : BaseTestComponent, ITestStep
    {
        private readonly string fullName;
        private readonly ITestStep parent;
        private readonly ITestInstance testInstance;
        private string id;

        /// <summary>
        /// Gets the localized name of the root step.
        /// </summary>
        public static string RootStepName
        {
            get { return Resources.BaseStep_RootStepName; }
        }

        /// <summary>
        /// Creates a root step of a test instance.
        /// </summary>
        /// <param name="testInstance">The test instance to which the step belongs</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testInstance"/>
        /// or <paramref name="testInstance"/> is null</exception>
        public BaseTestStep(ITestInstance testInstance)
            : this(testInstance, RootStepName, testInstance.CodeElement, null)
        {
        }

        /// <summary>
        /// Creates a step.
        /// </summary>
        /// <param name="testInstance">The test instance to which the step belongs</param>
        /// <param name="name">The step name</param>
        /// <param name="codeElement">The point of definition of the step, or null if unknown</param>
        /// <param name="parent">The parent step, or null if creating a root step</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="testInstance"/> is null</exception>
        public BaseTestStep(ITestInstance testInstance, string name, ICodeElementInfo codeElement, ITestStep parent)
            : base(name, codeElement)
        {
            if (testInstance == null)
                throw new ArgumentNullException("testInstance");
            if (name == null)
                throw new ArgumentNullException("name");

            this.testInstance = testInstance;
            this.parent = parent;

            fullName = GenerateFullName();
        }

        /// <inheritdoc />
        public override string Id
        {
            get
            {
                if (id == null)
                    id = Guid.NewGuid().ToString();
                return id;
            }
        }

        /// <inheritdoc />
        public string FullName
        {
            get { return fullName; }
        }

        /// <inheritdoc />
        public ITestStep Parent
        {
            get { return parent; }
        }

        /// <inheritdoc />
        public ITestInstance TestInstance
        {
            get { return testInstance; }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("[Step] {0}", fullName);
        }

        private string GenerateFullName()
        {
            if (parent == null)
                return testInstance.Name;
            else if (parent.Parent == null)
                return parent.FullName + @":" + Name;
            else
                return parent.FullName + @"/" + Name;
        }
    }
}
