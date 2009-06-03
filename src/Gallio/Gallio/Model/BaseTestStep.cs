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
using Gallio.Common.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITestStep"/>.
    /// </summary>
    public class BaseTestStep : BaseTestComponent, ITestStep
    {
        private readonly ITestStep parent;
        private readonly ITest test;
        private readonly bool isPrimary;
        private string id;
        private bool isDynamic;
        private bool isTestCase;

        /// <summary>
        /// Creates a primary step using the same name, code element and metadata
        /// as the test to which it belongs.
        /// </summary>
        /// <param name="test">The test to which the step belongs.</param>
        /// <param name="parent">The parent test step, or null if creating the root step.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null.</exception>
        public BaseTestStep(ITest test, ITestStep parent)
            : this(test, parent, test.Name, test.CodeElement, true)
        {
        }

        /// <summary>
        /// Creates a step.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If <paramref name="isPrimary"/> is true, then all metadata from the <paramref name="test"/>
        /// is copied to the step.  Otherwise the new step will have no metadata initially.
        /// </para>
        /// </remarks>
        /// <param name="test">The test to which the step belongs.</param>
        /// <param name="parent">The parent step, or null if creating a root step.</param>
        /// <param name="name">The step name.</param>
        /// <param name="codeElement">The point of definition of the step, or null if unknown.</param>
        /// <param name="isPrimary">True if the test step is primary.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="test"/> is null.</exception>
        public BaseTestStep(ITest test, ITestStep parent, string name, ICodeElementInfo codeElement, bool isPrimary)
            : base(name, codeElement)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            if (name == null)
                throw new ArgumentNullException("name");

            this.test = test;
            this.parent = parent;
            this.isPrimary = isPrimary;

            isTestCase = test.IsTestCase;

            if (isPrimary)
                Metadata.AddAll(test.Metadata);
        }

        /// <inheritdoc />
        public override string Id
        {
            get
            {
                if (id == null)
                    id = Hash64.CreateUniqueHash().ToString();
                return id;
            }
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
        public ITestStep Parent
        {
            get { return parent; }
        }

        /// <inheritdoc />
        public ITest Test
        {
            get { return test; }
        }

        /// <inheritdoc />
        public bool IsPrimary
        {
            get { return isPrimary; }
        }

        /// <inheritdoc />
        public bool IsDynamic
        {
            get { return isDynamic; }
            set { isDynamic = value; }
        }

        /// <inheritdoc />
        public bool IsTestCase
        {
            get { return isTestCase; }
            set { isTestCase = value; }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("[Step] {0}", Name);
        }
    }
}
