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
using System.Text;
using MbUnit.Framework.Kernel.Harness;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Builds a test tree for a given test project.  The builder retains
    /// context information used during the construction of a test tree.
    /// </summary>
    public class TestTreeBuilder
    {
        private ITestHarness harness;
        private ITest root;

        /// <summary>
        /// Creates a test tree builder.
        /// </summary>
        /// <param name="harness">The test project</param>
        public TestTreeBuilder(ITestHarness harness)
        {
            if (harness == null)
                throw new ArgumentNullException("harness");

            this.harness = harness;
            root = CreateRoot();
        }

        /// <summary>
        /// Gets the test harness.
        /// </summary>
        public ITestHarness Harness
        {
            get { return harness; }
        }

        /// <summary>
        /// Gets the root of the test tree.
        /// </summary>
        public ITest Root
        {
            get { return root; }
        }

        private static ITest CreateRoot()
        {
            return null;
        }
    }
}
