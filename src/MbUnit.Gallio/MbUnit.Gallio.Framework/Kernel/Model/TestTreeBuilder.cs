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
using MbUnit.Framework.Kernel.Metadata;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// The builder for a test tree.
    /// </summary>
    public class TestTreeBuilder : ModelTreeBuilder<ITest>
    {
        /// <summary>
        /// Creates a test tree builder initially populated with
        /// a root test.
        /// </summary>
        /// <param name="harness">The test harness</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="harness"/> is null</exception>
        public TestTreeBuilder(ITestHarness harness)
            : base(harness, new RootTest())
        {
        }

        /// <summary>
        /// Gets the root test.
        /// </summary>
        new public RootTest Root
        {
            get { return (RootTest)base.Root; }
        }
    }
}
