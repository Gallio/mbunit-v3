// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Utilities;

namespace Gallio.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITestInstance" />.
    /// </summary>
    public class BaseTestInstance : BaseTestComponent, ITestInstance
    {
        private readonly ITest test;
        private readonly ITestInstance parent;
        private string id;

        /// <summary>
        /// Initializes a test instance with the same name as its test.
        /// </summary>
        /// <param name="test">The test of which this is an instance</param>
        /// <param name="parent">The parent test instance, or null if this is to be the
        /// root test instance</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null</exception>
        public BaseTestInstance(ITest test, ITestInstance parent)
            : this(test, parent, test.Name)
        {
        }

        /// <summary>
        /// Initializes a test instance.
        /// </summary>
        /// <param name="test">The test of which this is an instance</param>
        /// <param name="parent">The parent test instance, or null if this is to be the
        /// root test instance</param>
        /// <param name="name">The name of the test instance</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name" /> or <paramref name="test"/> is null</exception>
        public BaseTestInstance(ITest test, ITestInstance parent, string name)
            : base(name, test.CodeElement)
        {
            if (test == null)
                throw new ArgumentNullException(@"test");

            this.test = test;
            this.parent = parent;
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
        public ITest Test
        {
            get { return test; }
        }

        /// <inheritdoc />
        public ITestInstance Parent
        {
            get { return parent; }
        }

        /// <inheritdoc />
        public string FullName
        {
            get
            {
                if (parent == null)
                    return Name;
                return String.Concat(parent.FullName, "/", Name);
            }
        }

        /// <inheritdoc />
        public virtual bool IsDynamic
        {
            get { return false; }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("Instance of {0}", test);
        }
    }
}