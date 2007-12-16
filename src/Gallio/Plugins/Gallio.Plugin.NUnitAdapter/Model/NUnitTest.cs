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
using Gallio.Model;
using Gallio.Model.Reflection;

using NUnitITest = NUnit.Core.ITest;
using NUnitTestName = NUnit.Core.TestName;

namespace Gallio.Plugin.NUnitAdapter.Model
{
    /// <summary>
    /// Wraps an NUnit test.
    /// </summary>
    internal class NUnitTest : BaseTest
    {
        private NUnitITest test;

        /// <summary>
        /// Initializes a test initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeElement">The point of definition, or null if none</param>
        /// <param name="test">The NUnit test, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public NUnitTest(string name, ICodeElementInfo codeElement, NUnitITest test)
            : base(name, codeElement)
        {
            this.test = test;
        }

        /// <summary>
        /// Gets or sets the NUnit test.
        /// </summary>
        public NUnitITest Test
        {
            get { return test; }
            set { test = value; }
        }

        /// <summary>
        /// Processes all of the test names associated with the test.
        /// </summary>
        /// <param name="action">The action to apply</param>
        public virtual void ProcessTestNames(Action<NUnitTestName> action)
        {
            action(test.TestName);
        }
    }
}
