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
using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Model.Execution;
using Gallio.Model.Reflection;
using Gallio.Properties;

namespace Gallio.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITest" />.
    /// </summary>
    /// <remarks>
    /// The base test implementation acts as a simple container for tests.
    /// Accordingly its kind is set to <see cref="ComponentKind.Group" /> by default.
    /// </remarks>
    public class BaseTest : BaseTestComponent, ITest
    {
        private readonly List<ITestParameter> parameters;
        private readonly List<ITest> children;
        private readonly List<ITest> dependencies;
        private ITest parent;
        private bool isTestCase;

        /// <summary>
        /// Initializes a test initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeElement">The point of definition of the test, or null if unknown</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public BaseTest(string name, ICodeElementInfo codeElement)
            : base(name, codeElement)
        {
            parameters = new List<ITestParameter>();
            dependencies = new List<ITest>();
            children = new List<ITest>();

            Kind = ComponentKind.Group;
        }

        /// <inheritdoc />
        public ITest Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <inheritdoc />
        public bool IsTestCase
        {
            get { return isTestCase; }
            set { isTestCase = value; }
        }

        /// <inheritdoc />
        public IList<ITestParameter> Parameters
        {
            get { return parameters.AsReadOnly(); }
        }

        /// <inheritdoc />
        public IList<ITest> Children
        {
            get { return children.AsReadOnly(); }
        }

        /// <inheritdoc />
        public IList<ITest> Dependencies
        {
            get { return dependencies.AsReadOnly(); }
        }

        /// <inheritdoc />
        public void AddParameter(ITestParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");
            if (parameter.Owner != null)
                throw new InvalidOperationException("The test parameter is already owned by another test.");

            parameters.Add(parameter);
        }

        /// <inheritdoc />
        public void AddChild(ITest test)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            if (test.Parent != null)
                throw new InvalidOperationException(Resources.ModelUtils_NodeAlreadyHasAParent);

            test.Parent = this;
            children.Add(test);
        }

        /// <inheritdoc />
        public void AddDependency(ITest test)
        {
            if (test == null)
                throw new ArgumentNullException("test");

            if (! dependencies.Contains(test))
                dependencies.Add(test);
        }

        /// <inheritdoc />
        public virtual IEnumerable<ITestInstance> GetInstances(bool guessDynamicInstances)
        {
            if (parameters.Count == 0)
                return new ITestInstance[] { new BaseTestInstance(this) };
            else
                return EmptyArray<ITestInstance>.Instance;
        }

        /// <inheritdoc />
        public virtual Factory<ITestController> TestControllerFactory
        {
            get { return null; }
        }
    }
}