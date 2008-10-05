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
using System.Collections.Generic;
using Gallio.Model.Execution;
using Gallio.Reflection;
using Gallio.Properties;
using Gallio.Utilities;

namespace Gallio.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITest" />.
    /// </summary>
    /// <remarks>
    /// The base test implementation acts as a simple container for tests.
    /// Accordingly its kind is set to <see cref="TestKinds.Group" /> by default.
    /// </remarks>
    public class BaseTest : BaseTestComponent, ITest
    {
        private readonly List<ITestParameter> parameters;
        private readonly List<ITest> children;
        private readonly List<ITest> dependencies;
        private ITest parent;
        private bool isTestCase;
        private int order;

        private string cachedId;
        private string cachedLocalId;
        private string baselineLocalId;

        /// <summary>
        /// Initializes a test initially without a parent.
        /// </summary>
        /// <param name="name">The name of the test</param>
        /// <param name="codeElement">The point of definition of the test, or null if unknown</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public BaseTest(string name, ICodeElementInfo codeElement)
            : base(name, codeElement)
        {
            parameters = new List<ITestParameter>();
            dependencies = new List<ITest>();
            children = new List<ITest>();

            Kind = TestKinds.Group;
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="MetadataKeys.TestKind" />
        /// metadata entry.  (This is a convenience method.)
        /// </summary>
        /// <value>
        /// One of the <see cref="TestKinds" /> constants.
        /// </value>
        public string Kind
        {
            get { return Metadata.GetValue(MetadataKeys.TestKind); }
            set { Metadata.SetValue(MetadataKeys.TestKind, value); }
        }

        /// <inheritdoc />
        public override string Id
        {
            get
            {
                // We compute an id using the combined hash of the local ids
                // from this test up the tree to the root.  A hash is used to
                // keep the ids relatively short and to prevent people from 
                // making undue assumptions about the internal structure of an id.
                if (cachedId == null)
                {
                    Hash64 hash = new Hash64();
                    hash = hash.Add(LocalId);

                    for (ITest ancestor = parent; ancestor != null; ancestor = ancestor.Parent)
                        hash = hash.Add(ancestor.LocalId);

                    cachedId = hash.ToString();
                }

                return cachedId;
            }
        }

        /// <summary>
        /// Gets or sets an initial approximation of a <see cref="LocalId" />, or null if none.
        /// The value returned by this method will be checked for uniqueness and amended as necessary
        /// to produce a truly unique <see cref="LocalId" />.
        /// </summary>
        /// <value>
        /// The default value of this property is <c>null</c> which causes the <see cref="ITestComponent.Name" />
        /// property to be used as the baseline local id.
        /// </value>
        /// <returns>The local id</returns>
        public string BaselineLocalId
        {
            get { return baselineLocalId; }
            set { baselineLocalId = value; }
        }

        /// <inheritdoc />
        public string FullName
        {
            get
            {
                if (parent == null)
                    return @"";
                if (parent.Parent == null)
                    return Name;
                return String.Concat(parent.FullName, "/", Name);
            }
        }

        /// <inheritdoc />
        public ITest Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                cachedId = null;
                cachedLocalId = null;
            }
        }

        /// <inheritdoc />
        public string LocalId
        {
            get
            {
                if (cachedLocalId == null)
                {
                    string root = baselineLocalId ?? Name;
                    cachedLocalId = root;

                    int suffix = 0;
                    while (!IsLocalIdUniqueAmongSiblings(cachedLocalId))
                        cachedLocalId = root + (++suffix);
                }

                return cachedLocalId;
            }
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
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        /// <inheritdoc />
        public void AddParameter(ITestParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");
            if (parameter.Owner != null)
                throw new InvalidOperationException("The test parameter is already owned by another test.");

            parameter.Owner = this;
            parameters.Add(parameter);
        }

        /// <inheritdoc />
        public void AddChild(ITest test)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            if (test.Parent != null)
                throw new InvalidOperationException(Resources.BaseTest_TestAlreadyHasAParent);

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
        public virtual Func<ITestController> TestControllerFactory
        {
            get { return null; }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("[{0}] {1}", Kind, Name);
        }

        private bool IsLocalIdUniqueAmongSiblings(string localId)
        {
            if (parent != null)
            {
                foreach (ITest sibling in parent.Children)
                {
                    if (sibling != this && sibling.LocalId == localId)
                        return true;
                }
            }

            return true;
        }
    }
}