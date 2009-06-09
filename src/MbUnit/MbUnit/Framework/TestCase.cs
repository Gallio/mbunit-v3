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
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Common.Diagnostics;

namespace MbUnit.Framework
{
    /// <summary>
    /// Describes a test case generated either at test exploration time or at test
    /// execution time by a test factory.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A test case has a timeout of 10 minutes by default.  This may be changed
    /// setting the <see cref="TestDefinition.Timeout" /> property.
    /// </para>
    /// <para>
    /// Refer to the examples on the <see cref="Test" /> class for more information.
    /// </para>
    /// </remarks>
    /// <seealso cref="Test"/>
    public class TestCase : TestDefinition
    {
        /// <summary>
        /// Creates a test case with a delegate to execute as its main body.
        /// </summary>
        /// <param name="name">The test case name.</param>
        /// <param name="execute">The main body of the test case.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="execute"/>
        /// is null.</exception>
        public TestCase(string name, Action execute)
            : base(name)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            Execute = execute;
            Timeout = TestAssemblyExecutionParameters.DefaultTestCaseTimeout;
        }

        /// <summary>
        /// Gets the delegate to run as the main body of the test case.
        /// </summary>
        public Action Execute { get; private set; }

        /// <inheritdoc />
        protected override bool IsTestCase
        {
            get { return true; }
        }

        /// <inheritdoc />
        protected override string Kind
        {
            get { return TestKinds.Test; }
        }

        /// <inheritdoc />
        [UserCodeEntryPoint]
        protected override void OnExecuteSelf()
        {
            Execute();
        }
    }
}
