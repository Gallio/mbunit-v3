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
using System.Collections.Generic;
using Gallio.Common.Collections;
using Gallio.Common;
using Gallio.Model.Execution;
using Gallio.Common.Reflection;
using Gallio.Properties;

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
        private List<ITestParameter> parameters;
        private List<ITest> children;
        private List<ITest> dependencies;
        private HashSet<string> assignedChildLocalIds;
        private ITest parent;
        private bool isTestCase;
        private int order;

        private string cachedId;
        private string cachedLocalId;
        private string localIdHint;

        /// <summary>
        /// Initializes a test initially without a parent.
        /// </summary>
        /// <param name="name">The name of the test.</param>
        /// <param name="codeElement">The point of definition of the test, or null if unknown.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
        public BaseTest(string name, ICodeElementInfo codeElement)
            : base(name, codeElement)
        {
            Kind = TestKinds.Group;
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
        /// Gets or sets a suggested <see cref="LocalId" /> hint, or null if none.
        /// The value returned by this method will be checked for uniqueness and amended as necessary
        /// to produce a truly unique <see cref="LocalId" />.
        /// </summary>
        /// <value>
        /// The default value of this property is <c>null</c> which causes the <see cref="ITestComponent.Name" />
        /// property to be used as the local id hint.
        /// </value>
        /// <returns>The local id hint.</returns>
        public string LocalIdHint
        {
            get { return localIdHint; }
            set { localIdHint = value; }
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
                    string resolvedLocalIdHint = localIdHint ?? Name;
                    if (parent != null)
                        cachedLocalId = parent.GetUniqueLocalIdForChild(resolvedLocalIdHint);
                    else
                        cachedLocalId = resolvedLocalIdHint;
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
            get { return parameters != null ? (IList<ITestParameter>) parameters.AsReadOnly() : EmptyArray<ITestParameter>.Instance; }
        }

        /// <inheritdoc />
        public IList<ITest> Children
        {
            get { return children != null ? (IList<ITest>) children.AsReadOnly() : EmptyArray<ITest>.Instance; }
        }

        /// <inheritdoc />
        public IList<ITest> Dependencies
        {
            get { return dependencies != null ? (IList<ITest>) dependencies.AsReadOnly() : EmptyArray<ITest>.Instance; }
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

            if (parameters == null)
                parameters = new List<ITestParameter>();
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

            if (children == null)
                children = new List<ITest>();
            children.Add(test);
        }

        /// <inheritdoc />
        public void AddDependency(ITest test)
        {
            if (test == null)
                throw new ArgumentNullException("test");

            if (dependencies == null)
                dependencies = new List<ITest>();
            if (! dependencies.Contains(test))
                dependencies.Add(test);
        }

        /// <inheritdoc />
        public string GetUniqueLocalIdForChild(string localIdHint)
        {
            if (localIdHint == null)
                throw new ArgumentNullException("localIdHint");

            string candidateLocalId = localIdHint;

            if (assignedChildLocalIds == null)
            {
                assignedChildLocalIds = new HashSet<string>();
            }
            else
            {
                int index = 2;
                while (assignedChildLocalIds.Contains(candidateLocalId))
                {
                    candidateLocalId = localIdHint + index;
                    index += 1;
                }
            }

            assignedChildLocalIds.Add(candidateLocalId);
            return candidateLocalId;
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
    }
}