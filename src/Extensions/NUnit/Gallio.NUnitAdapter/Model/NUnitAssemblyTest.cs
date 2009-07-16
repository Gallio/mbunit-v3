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
using Gallio.Model;
using Gallio.Model.Commands;
using Gallio.Common.Reflection;
using NUnitTestRunner = NUnit.Core.TestRunner;
using NUnitTestName = NUnit.Core.TestName;
using NUnitITest = NUnit.Core.ITest;

namespace Gallio.NUnitAdapter.Model
{
    /// <summary>
    /// Represents an NUnit assembly-level test.
    /// </summary>
    internal class NUnitAssemblyTest : NUnitTest
    {
        private readonly NUnitTestRunner runner;

        /// <summary>
        /// Creates an NUnit assembly-level test.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="runner">The NUnit test runner.</param>
        public NUnitAssemblyTest(IAssemblyInfo assembly, NUnitTestRunner runner)
            : base(assembly.Name, assembly, runner.Test)
        {
            Kind = TestKinds.Assembly;

            this.runner = runner;
        }

        /// <summary>
        /// Gets the NUnit test runner.
        /// </summary>
        public NUnitTestRunner Runner
        {
            get { return runner; }
        }

        /// <inheritdoc />
        public override void ProcessTestNames(Action<NUnitTestName> action)
        {
            foreach (NUnitITest test in Test.Tests)
                action(test.TestName);
        }
    }
}
