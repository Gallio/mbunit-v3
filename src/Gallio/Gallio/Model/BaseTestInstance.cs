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

namespace Gallio.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITestInstance" />.
    /// </summary>
    public class BaseTestInstance : BaseTestComponent, ITestInstance
    {
        private readonly ITest test;
        private string id;

        /// <summary>
        /// Initializes a test instance with the same name as its test.
        /// </summary>
        /// <param name="test">The test of which this is an instance</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null</exception>
        public BaseTestInstance(ITest test)
            : this(test, test.Name)
        {
        }

        /// <summary>
        /// Initializes a test instance.
        /// </summary>
        /// <param name="test">The test of which this is an instance</param>
        /// <param name="name">The name of the test instance</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name" /> or <paramref name="test"/> is null</exception>
        public BaseTestInstance(ITest test, string name)
            : base(name, test.CodeElement)
        {
            if (test == null)
                throw new ArgumentNullException(@"test");

            this.test = test;
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
        public ITest Test
        {
            get { return test; }
        }

        /// <inheritdoc />
        public virtual bool IsDynamic
        {
            get { return false; }
        }

        /// <inheritdoc />
        public object GetParameterValue(ITestParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");
            if (parameter.Owner != test)
                throw new ArgumentException("Parameter is not owned by this test.", "parameter");

            return InternalGetParameterValue(parameter);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("Instance of {0}", test);
        }

        /// <summary>
        /// <para>
        /// Internal implementation of <see cref="GetParameterValue" /> after
        /// validation has been performed.
        /// </para>
        /// <para>
        /// The default implementation throws <see cref="InvalidOperationException" />.
        /// </para>
        /// </summary>
        /// <param name="parameter">The test parameter</param>
        /// <returns>The parameter value</returns>
        protected virtual object InternalGetParameterValue(ITestParameter parameter)
        {
            throw new InvalidOperationException("The test parameter value is not available at this time.");
        }
    }
}