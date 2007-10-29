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
using Gallio.Model;

namespace Gallio.Model
{
    /// <summary>
    /// The builder for a test tree.
    /// </summary>
    public class TestTreeBuilder : ModelTreeBuilder<ITest>
    {
        private readonly TestEnumerationOptions options;

        /// <summary>
        /// Creates a test tree builder initially populated with
        /// a root test.
        /// </summary>
        /// <param name="rootTest">The root test</param>
        /// <param name="options">The test enumeration options</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rootTest"/> or <paramref name="options" /> is null</exception>
        public TestTreeBuilder(RootTest rootTest, TestEnumerationOptions options)
            : base(rootTest)
        {
            if (options == null)
                throw new ArgumentNullException(@"options");

            this.options = options;
        }

        /// <summary>
        /// Gets the root test.
        /// </summary>
        new public RootTest Root
        {
            get { return (RootTest)base.Root; }
        }

        /// <summary>
        /// Gets the test enumeration options.
        /// </summary>
        public TestEnumerationOptions Options
        {
            get { return options; }
        }
    }
}
