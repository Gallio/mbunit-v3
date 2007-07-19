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

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Describes the scope in which a test has been instantiated including
    /// its parentage and available data providers.
    /// </summary>
    public class TestScope
    {
        private TestScope parentScope;
        private ITest containingTest;

        /// <summary>
        /// Creates a new scope.
        /// </summary>
        /// <param name="parentScope">The parent scope, or null if at the root</param>
        /// <param name="containingTest">The containing test</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="containingTest"/> is null</exception>
        public TestScope(TestScope parentScope, ITest containingTest)
        {
            this.parentScope = parentScope;
            this.containingTest = containingTest;
        }

        /// <summary>
        /// Gets the parent of this scope, or null if at the root.
        /// </summary>
        public TestScope ParentScope
        {
            get { return parentScope; }
        }

        /// <summary>
        /// Gets the test that contains this scope, never null.
        /// </summary>
        public ITest ContainingTest
        {
            get { return containingTest; }
        }
    }
}
